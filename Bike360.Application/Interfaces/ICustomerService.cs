using Bike360.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bike360.Application.Interfaces
{
    /// <summary>
    /// Contract for all customer-facing business operations.
    /// Layer : Application → Interfaces → ICustomerService.cs
    /// </summary>
    public interface ICustomerService
    {
        // Registration & Profile
        Task<ApiResponse<CustomerProfileDto>> RegisterAsync(CustomerRegisterDto dto);
        Task<ApiResponse<CustomerProfileDto>> GetProfileAsync(Guid customerId);
        Task<ApiResponse<CustomerProfileDto>> UpdateProfileAsync(Guid customerId, CustomerUpdateProfileDto dto);

        // Vehicles
        Task<ApiResponse<List<VehicleDto>>> GetMyVehiclesAsync(Guid customerId);
        Task<ApiResponse<VehicleDto>> AddVehicleAsync(Guid customerId, AddVehicleDto dto);
        Task<ApiResponse<VehicleDto>> UpdateVehicleAsync(Guid customerId, Guid vehicleId, UpdateVehicleDto dto);
        Task<ApiResponse<bool>> RemoveVehicleAsync(Guid customerId, Guid vehicleId);

        // Purchase & Service History
        Task<ApiResponse<List<SaleInvoiceSummaryDto>>> GetPurchaseHistoryAsync(Guid customerId);

        // Appointments
        Task<ApiResponse<AppointmentDto>> BookAppointmentAsync(Guid customerId, BookAppointmentDto dto);
        Task<ApiResponse<List<AppointmentDto>>> GetMyAppointmentsAsync(Guid customerId);
        Task<ApiResponse<bool>> CancelAppointmentAsync(Guid customerId, Guid appointmentId);

        // Part Requests
        Task<ApiResponse<PartRequestDto>> RequestUnavailablePartAsync(Guid customerId, PartRequestCreateDto dto);
        Task<ApiResponse<List<PartRequestDto>>> GetMyPartRequestsAsync(Guid customerId);

        // Service Reviews
        Task<ApiResponse<ServiceReviewDto>> SubmitReviewAsync(Guid customerId, SubmitReviewDto dto);
        Task<ApiResponse<List<ServiceReviewDto>>> GetApprovedReviewsAsync();

        // AI Part Failure Prediction
        Task<ApiResponse<PartFailurePredictionDto>> GetPartFailurePredictionsAsync(Guid customerId, Guid vehicleId);
    }
}