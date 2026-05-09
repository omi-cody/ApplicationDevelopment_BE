using Bike360.Application.DTOs;
using Bike360.Application.Interfaces;
using Bike360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bike360.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<CustomerService> _logger;

        private const decimal LoyaltyThreshold    = 5_000m;
        private const decimal LoyaltyDiscountRate = 0.10m;

        public CustomerService(
            ICustomerRepository repo,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<CustomerService> logger)
        {
            _repo         = repo;
            _userManager  = userManager;
            _emailService = emailService;
            _logger       = logger;
        }

        
        // Registration
        
        public async Task<ApiResponse<CustomerProfileDto>> RegisterAsync(CustomerRegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null)
                return ApiResponse<CustomerProfileDto>.Fail("An account with this email already exists.");

            var user = new ApplicationUser
            {
                UserName    = dto.Email.ToLowerInvariant().Trim(),
                Email       = dto.Email.ToLowerInvariant().Trim(),
                FullName    = dto.FullName.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim()
            };

            var identityResult = await _userManager.CreateAsync(user, dto.Password);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("Customer registration failed for {Email}: {Errors}",
                    dto.Email, string.Join(", ", errors));
                return ApiResponse<CustomerProfileDto>.Fail(errors);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                FullName          = dto.FullName.Trim(),
                Email             = dto.Email.ToLowerInvariant().Trim(),
                PhoneNumber       = dto.PhoneNumber.Trim(),
                Address           = dto.Address.Trim()
            };

            var saved = await _repo.AddAsync(customer);
            _logger.LogInformation("New customer registered: {Email}", dto.Email);

            return ApiResponse<CustomerProfileDto>.Ok(
                MapToProfileDto(saved, new List<Vehicle>()),
                "Registration successful. Welcome!");
        }

        
        // Profile
        
        public async Task<ApiResponse<CustomerProfileDto>> GetProfileAsync(Guid customerId)
        {
            var customer = await _repo.GetByIdAsync(customerId);
            if (customer is null)
                return ApiResponse<CustomerProfileDto>.Fail("Customer not found.");

            var vehicles = await _repo.GetVehiclesByCustomerAsync(customerId);
            return ApiResponse<CustomerProfileDto>.Ok(MapToProfileDto(customer, vehicles));
        }

        public async Task<ApiResponse<CustomerProfileDto>> UpdateProfileAsync(
            Guid customerId, CustomerUpdateProfileDto dto)
        {
            var customer = await _repo.GetByIdAsync(customerId);
            if (customer is null)
                return ApiResponse<CustomerProfileDto>.Fail("Customer not found.");

            if (dto.FullName    is not null) customer.FullName    = dto.FullName.Trim();
            if (dto.PhoneNumber is not null) customer.PhoneNumber = dto.PhoneNumber.Trim();
            if (dto.Address     is not null) customer.Address     = dto.Address.Trim();

            var updated  = await _repo.UpdateAsync(customer);
            var vehicles = await _repo.GetVehiclesByCustomerAsync(customerId);

            return ApiResponse<CustomerProfileDto>.Ok(
                MapToProfileDto(updated, vehicles), "Profile updated successfully.");
        }

        
        // Vehicles
        
        public async Task<ApiResponse<List<VehicleDto>>> GetMyVehiclesAsync(Guid customerId)
        {
            var vehicles = await _repo.GetVehiclesByCustomerAsync(customerId);
            return ApiResponse<List<VehicleDto>>.Ok(vehicles.Select(MapToVehicleDto).ToList());
        }

        public async Task<ApiResponse<VehicleDto>> AddVehicleAsync(Guid customerId, AddVehicleDto dto)
        {
            var customer = await _repo.GetByIdAsync(customerId);
            if (customer is null)
                return ApiResponse<VehicleDto>.Fail("Customer not found.");

            var vehicle = new Vehicle
            {
                CustomerId      = customerId,
                VehicleNumber   = dto.VehicleNumber.ToUpper().Trim(),
                Make            = dto.Make.Trim(),
                Model           = dto.Model.Trim(),
                Year            = dto.Year,
                VehicleType     = dto.VehicleType.Trim(),
                Mileage         = dto.Mileage,
                LastServiceDate = dto.LastServiceDate
            };

            var saved = await _repo.AddVehicleAsync(vehicle);
            _logger.LogInformation("Vehicle {VehicleNumber} added for customer {CustomerId}",
                vehicle.VehicleNumber, customerId);

            return ApiResponse<VehicleDto>.Ok(MapToVehicleDto(saved), "Vehicle added successfully.");
        }

        public async Task<ApiResponse<VehicleDto>> UpdateVehicleAsync(
            Guid customerId, Guid vehicleId, UpdateVehicleDto dto)
        {
            var vehicle = await _repo.GetVehicleByIdAsync(vehicleId);
            if (vehicle is null || vehicle.CustomerId != customerId)
                return ApiResponse<VehicleDto>.Fail("Vehicle not found or access denied.");

            if (dto.VehicleNumber   is not null) vehicle.VehicleNumber   = dto.VehicleNumber.ToUpper().Trim();
            if (dto.Make            is not null) vehicle.Make            = dto.Make.Trim();
            if (dto.Model           is not null) vehicle.Model           = dto.Model.Trim();
            if (dto.Year.HasValue)               vehicle.Year            = dto.Year.Value;
            if (dto.VehicleType     is not null) vehicle.VehicleType     = dto.VehicleType.Trim();
            if (dto.Mileage.HasValue)            vehicle.Mileage         = dto.Mileage.Value;
            if (dto.LastServiceDate.HasValue)    vehicle.LastServiceDate = dto.LastServiceDate.Value;

            var updated = await _repo.UpdateVehicleAsync(vehicle);
            return ApiResponse<VehicleDto>.Ok(MapToVehicleDto(updated), "Vehicle updated.");
        }

        public async Task<ApiResponse<bool>> RemoveVehicleAsync(Guid customerId, Guid vehicleId)
        {
            var vehicle = await _repo.GetVehicleByIdAsync(vehicleId);
            if (vehicle is null || vehicle.CustomerId != customerId)
                return ApiResponse<bool>.Fail("Vehicle not found or access denied.");

            await _repo.DeleteVehicleAsync(vehicle);
            return ApiResponse<bool>.Ok(true, "Vehicle removed.");
        }

        
        // Purchase & Service History

        public async Task<ApiResponse<List<SaleInvoiceSummaryDto>>> GetPurchaseHistoryAsync(Guid customerId)
        {
            var invoices = await _repo.GetInvoicesByCustomerAsync(customerId);

            var result = invoices.Select(inv => new SaleInvoiceSummaryDto
            {
                Id             = inv.Id,
                InvoiceNumber  = inv.InvoiceNumber,
                InvoiceDate    = inv.InvoiceDate,
                TotalAmount    = inv.TotalAmount,
                DiscountAmount = inv.DiscountAmount,
                IsPaid         = inv.IsPaid,
                IsCreditSale   = inv.IsCreditSale,
                VehicleNumber  = inv.Vehicle?.VehicleNumber ?? "N/A",
                Items = inv.Items.Select(i => new SaleInvoiceItemDto
                {
                    PartName  = i.PartName,
                    Quantity  = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            }).ToList();

            return ApiResponse<List<SaleInvoiceSummaryDto>>.Ok(result);
        }

        
        // Appointments

        public async Task<ApiResponse<AppointmentDto>> BookAppointmentAsync(
            Guid customerId, BookAppointmentDto dto)
        {
            if (dto.AppointmentDate <= DateTime.UtcNow)
                return ApiResponse<AppointmentDto>.Fail("Appointment date must be in the future.");

            var vehicle = await _repo.GetVehicleByIdAsync(dto.VehicleId);
            if (vehicle is null || vehicle.CustomerId != customerId)
                return ApiResponse<AppointmentDto>.Fail("Vehicle not found or does not belong to you.");

            var appointment = new Appointment
            {
                CustomerId      = customerId,
                VehicleId       = dto.VehicleId,
                AppointmentDate = dto.AppointmentDate,
                ServiceType     = dto.ServiceType.Trim(),
                Notes           = dto.Notes.Trim(),
                Status          = AppointmentStatus.Pending
            };

            var saved    = await _repo.AddAppointmentAsync(appointment);
            var customer = await _repo.GetByIdAsync(customerId);

            if (customer is not null)
            {
                await _emailService.SendAsync(
                    customer.Email,
                    "Appointment Confirmation",
                    $"Dear {customer.FullName},\n\n" +
                    $"Your appointment for '{dto.ServiceType}' is confirmed for " +
                    $"{dto.AppointmentDate:dddd, MMMM dd yyyy 'at' HH:mm}.\n\n" +
                    "Vehicle Parts Service Center");
            }

            _logger.LogInformation("Appointment booked for customer {CustomerId} on {Date}",
                customerId, dto.AppointmentDate);

            return ApiResponse<AppointmentDto>.Ok(
                MapToAppointmentDto(saved, vehicle.VehicleNumber),
                "Appointment booked. A confirmation email has been sent.");
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetMyAppointmentsAsync(Guid customerId)
        {
            var appointments = await _repo.GetAppointmentsByCustomerAsync(customerId);
            var result = appointments
                .Select(a => MapToAppointmentDto(a, a.Vehicle?.VehicleNumber ?? "N/A"))
                .ToList();
            return ApiResponse<List<AppointmentDto>>.Ok(result);
        }

        public async Task<ApiResponse<bool>> CancelAppointmentAsync(Guid customerId, Guid appointmentId)
        {
            var appointment = await _repo.GetAppointmentByIdAsync(appointmentId);
            if (appointment is null || appointment.CustomerId != customerId)
                return ApiResponse<bool>.Fail("Appointment not found or access denied.");

            if (appointment.Status == AppointmentStatus.Completed)
                return ApiResponse<bool>.Fail("Cannot cancel a completed appointment.");

            appointment.Status = AppointmentStatus.Cancelled;
            await _repo.UpdateAppointmentAsync(appointment);
            return ApiResponse<bool>.Ok(true, "Appointment cancelled.");
        }

        
        // Part Requests
        
        public async Task<ApiResponse<PartRequestDto>> RequestUnavailablePartAsync(
            Guid customerId, PartRequestCreateDto dto)
        {
            var request = new PartRequest
            {
                CustomerId     = customerId,
                PartName       = dto.PartName.Trim(),
                Description    = dto.Description.Trim(),
                QuantityNeeded = dto.QuantityNeeded,
                Status         = PartRequestStatus.Pending
            };

            var saved = await _repo.AddPartRequestAsync(request);
            _logger.LogInformation("Part request '{PartName}' submitted by customer {CustomerId}",
                dto.PartName, customerId);

            return ApiResponse<PartRequestDto>.Ok(
                MapToPartRequestDto(saved),
                "Part request submitted. You will be notified when available.");
        }

        public async Task<ApiResponse<List<PartRequestDto>>> GetMyPartRequestsAsync(Guid customerId)
        {
            var requests = await _repo.GetPartRequestsByCustomerAsync(customerId);
            return ApiResponse<List<PartRequestDto>>.Ok(requests.Select(MapToPartRequestDto).ToList());
        }

        
        // Service Reviews

        public async Task<ApiResponse<ServiceReviewDto>> SubmitReviewAsync(
            Guid customerId, SubmitReviewDto dto)
        {
            var customer = await _repo.GetByIdAsync(customerId);
            if (customer is null)
                return ApiResponse<ServiceReviewDto>.Fail("Customer not found.");

            var review = new ServiceReview
            {
                CustomerId = customerId,
                Rating     = dto.Rating,
                Title      = dto.Title.Trim(),
                Comment    = dto.Comment.Trim(),
                IsApproved = false
            };

            var saved = await _repo.AddReviewAsync(review);
            _logger.LogInformation("Review submitted by customer {CustomerId}, rating {Rating}",
                customerId, dto.Rating);

            return ApiResponse<ServiceReviewDto>.Ok(new ServiceReviewDto
            {
                Id           = saved.Id,
                CustomerName = customer.FullName,
                Rating       = saved.Rating,
                Title        = saved.Title,
                Comment      = saved.Comment,
                CreatedAt    = saved.CreatedAt,
                IsApproved   = saved.IsApproved
            }, "Thank you! Your review will appear after admin approval.");
        }

        public async Task<ApiResponse<List<ServiceReviewDto>>> GetApprovedReviewsAsync()
        {
            var reviews = await _repo.GetApprovedReviewsAsync();
            var result  = reviews.Select(r => new ServiceReviewDto
            {
                Id           = r.Id,
                CustomerName = r.Customer.FullName,
                Rating       = r.Rating,
                Title        = r.Title,
                Comment      = r.Comment,
                CreatedAt    = r.CreatedAt,
                IsApproved   = r.IsApproved
            }).ToList();

            return ApiResponse<List<ServiceReviewDto>>.Ok(result);
        }

       
        // AI Part Failure Prediction

        public async Task<ApiResponse<PartFailurePredictionDto>> GetPartFailurePredictionsAsync(
            Guid customerId, Guid vehicleId)
        {
            var vehicle = await _repo.GetVehicleByIdAsync(vehicleId);
            if (vehicle is null || vehicle.CustomerId != customerId)
                return ApiResponse<PartFailurePredictionDto>.Fail("Vehicle not found or access denied.");

            var alerts           = new List<PredictedPartAlertDto>();
            var vehicleAge       = DateTime.UtcNow.Year - vehicle.Year;
            var daysSinceService = (DateTime.UtcNow - vehicle.LastServiceDate).TotalDays;

            if (vehicle.Mileage > 5_000 || daysSinceService > 90)
                alerts.Add(new PredictedPartAlertDto
                {
                    PartName       = "Oil Filter",
                    RiskLevel      = vehicle.Mileage > 7_500 ? "High" : "Medium",
                    Reason         = $"Mileage: {vehicle.Mileage:N0} km | Days since service: {(int)daysSinceService}",
                    Recommendation = "Replace oil and oil filter at next service."
                });

            if (vehicle.Mileage > 30_000)
                alerts.Add(new PredictedPartAlertDto
                {
                    PartName       = "Brake Pads",
                    RiskLevel      = vehicle.Mileage > 50_000 ? "High" : "Medium",
                    Reason         = $"Brake pads typically last 30 000–50 000 km. Current: {vehicle.Mileage:N0} km.",
                    Recommendation = "Schedule a brake inspection."
                });

            if (vehicleAge >= 3)
                alerts.Add(new PredictedPartAlertDto
                {
                    PartName       = "Battery",
                    RiskLevel      = vehicleAge >= 5 ? "High" : "Low",
                    Reason         = $"Vehicle is {vehicleAge} years old. Battery lifespan: 3–5 years.",
                    Recommendation = "Have the battery tested at your next visit."
                });

            if (vehicle.Mileage > 15_000)
                alerts.Add(new PredictedPartAlertDto
                {
                    PartName       = "Air Filter",
                    RiskLevel      = "Low",
                    Reason         = $"Air filters replaced every 15 000–20 000 km. Current: {vehicle.Mileage:N0} km.",
                    Recommendation = "Replace air filter to maintain engine efficiency."
                });

            if (vehicle.Mileage > 60_000 || vehicleAge >= 6)
                alerts.Add(new PredictedPartAlertDto
                {
                    PartName       = "Timing Belt",
                    RiskLevel      = "High",
                    Reason         = "Critical at 60 000 km or 6 years — failure causes severe engine damage.",
                    Recommendation = "Book timing belt replacement as soon as possible."
                });

            if (!alerts.Any())
                alerts.Add(new PredictedPartAlertDto
                {
                    PartName       = "No Issues Predicted",
                    RiskLevel      = "None",
                    Reason         = "Mileage and service history look healthy.",
                    Recommendation = "Continue with your regular maintenance schedule."
                });

            return ApiResponse<PartFailurePredictionDto>.Ok(new PartFailurePredictionDto
            {
                VehicleId     = vehicle.Id,
                VehicleNumber = vehicle.VehicleNumber,
                Alerts        = alerts
            });
        }

        
        // Private mapping helpers

        private static CustomerProfileDto MapToProfileDto(Customer c, List<Vehicle> vehicles) => new()
        {
            Id          = c.Id,
            FullName    = c.FullName,
            Email       = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address     = c.Address,
            CreatedAt   = c.CreatedAt,
            Vehicles    = vehicles.Select(MapToVehicleDto).ToList()
        };

        private static VehicleDto MapToVehicleDto(Vehicle v) => new()
        {
            Id              = v.Id,
            VehicleNumber   = v.VehicleNumber,
            Make            = v.Make,
            Model           = v.Model,
            Year            = v.Year,
            VehicleType     = v.VehicleType,
            Mileage         = v.Mileage,
            LastServiceDate = v.LastServiceDate
        };

        private static AppointmentDto MapToAppointmentDto(Appointment a, string vehicleNumber) => new()
        {
            Id              = a.Id,
            VehicleId       = a.VehicleId,
            VehicleNumber   = vehicleNumber,
            AppointmentDate = a.AppointmentDate,
            ServiceType     = a.ServiceType,
            Notes           = a.Notes,
            Status          = a.Status.ToString(),
            CreatedAt       = a.CreatedAt
        };

        private static PartRequestDto MapToPartRequestDto(PartRequest r) => new()
        {
            Id             = r.Id,
            PartName       = r.PartName,
            Description    = r.Description,
            QuantityNeeded = r.QuantityNeeded,
            Status         = r.Status.ToString(),
            CreatedAt      = r.CreatedAt
        };
    }
}