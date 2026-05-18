using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Domain.Entities
{
    public class SalesInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public bool IsEmailSent { get; set; } = false;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public List<SalesInvoiceItem> Items { get; set; } = new();
    }
}
