using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.DTOs.Sales
{
    public class SalesInvoiceResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public bool IsEmailSent { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemResponse> Items { get; set; } = new();
    }

    public class InvoiceItemResponse
    {
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}
