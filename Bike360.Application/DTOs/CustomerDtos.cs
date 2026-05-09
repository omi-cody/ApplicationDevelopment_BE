using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bike360.Application.DTOs
{
    // Registration & Profile 

    public class CustomerRegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }

    public class CustomerUpdateProfileDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }

    public class CustomerProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<VehicleDto> Vehicles { get; set; } = new();
    }

    // Vehicle 

    public class AddVehicleDto
    {
        [Required]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        public string Make { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        public string VehicleType { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        public DateTime LastServiceDate { get; set; }
    }

    public class UpdateVehicleDto
    {
        public string? VehicleNumber { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? VehicleType { get; set; }
        public int? Mileage { get; set; }
        public DateTime? LastServiceDate { get; set; }
    }

    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public int Mileage { get; set; }
        public DateTime LastServiceDate { get; set; }
    }

    // Purchase / Service History 

    public class SaleInvoiceSummaryDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCreditSale { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public List<SaleInvoiceItemDto> Items { get; set; } = new();
    }

    public class SaleInvoiceItemDto
    {
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    //  Appointments 

    public class BookAppointmentDto
    {
        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }

    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // ── Part Requests 

    public class PartRequestCreateDto
    {
        [Required]
        [StringLength(200)]
        public string PartName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 100)]
        public int QuantityNeeded { get; set; } = 1;
    }

    public class PartRequestDto
    {
        public Guid Id { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuantityNeeded { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    //  Service Reviews 

    public class SubmitReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }

    public class ServiceReviewDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
    }

    // AI Part Failure Prediction 

    public class PartFailurePredictionDto
    {
        public Guid VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public List<PredictedPartAlertDto> Alerts { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class PredictedPartAlertDto
    {
        public string PartName { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
    }

    // Common API response wrapper 

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Success") =>
            new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string error) =>
            new() { Success = false, Message = error, Errors = new List<string> { error } };

        public static ApiResponse<T> Fail(List<string> errors) =>
            new() { Success = false, Message = "One or more errors occurred.", Errors = errors };
    }
}