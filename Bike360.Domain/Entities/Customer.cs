using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public List<CustomerVehicle> Vehicles { get; set; } = new();
    }
  }

