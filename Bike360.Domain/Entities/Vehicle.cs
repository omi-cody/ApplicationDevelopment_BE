using System;
using System.Collections.Generic;

namespace Bike360.Domain.Entities
{
    
    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CustomerId { get; set; }

        public string VehicleNumber { get; set; } = string.Empty;

        public string Make { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public string VehicleType { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public DateTime LastServiceDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Customer Customer { get; set; } = null!;
        public ICollection<SaleInvoice> SaleInvoices { get; set; } = new List<SaleInvoice>();
    }
}