using System;
using System.Collections.Generic;

namespace Bike360.Domain.Entities
{
    
    public class SaleInvoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string InvoiceNumber { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public Guid? VehicleId { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public decimal SubTotal { get; set; }
        
        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsCreditSale { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation
        public Customer Customer { get; set; } = null!;
        public Vehicle? Vehicle { get; set; }
        public ICollection<SaleInvoiceItem> Items { get; set; } = new List<SaleInvoiceItem>();
    }
}