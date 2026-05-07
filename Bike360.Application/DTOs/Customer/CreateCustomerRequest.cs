using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.DTOs.Customer
{
    public class CreateCustomerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<VehicleRequest> Vehicles { get; set; } = new();
    }

    public class VehicleRequest
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
