using System;

namespace Bike360.Domain.Entities
{
    
    public class SaleInvoiceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SaleInvoiceId { get; set; }

        public Guid PartId { get; set; }

        public string PartName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;

        // Navigation
        public SaleInvoice SaleInvoice { get; set; } = null!;
    }
}