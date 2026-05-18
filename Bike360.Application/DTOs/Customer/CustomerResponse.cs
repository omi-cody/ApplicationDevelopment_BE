using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.DTOs.Customer
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }  // ← Guid not int
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty; 
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<VehicleResponse> Vehicles { get; set; } = new();
    }

    public class VehicleResponse
    {
        public Guid Id { get; set; }  
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}