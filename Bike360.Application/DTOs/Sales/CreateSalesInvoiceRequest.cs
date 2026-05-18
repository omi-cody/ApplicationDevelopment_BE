using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.DTOs.Sales
{
    public class CreateSalesInvoiceRequest
    {
        public int CustomerId { get; set; }
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    public class SaleItemRequest
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }
    }
}
