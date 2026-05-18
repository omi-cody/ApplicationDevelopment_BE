using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Domain.Entities
{
    public class SalesInvoiceItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }

        public int SalesInvoiceId { get; set; }
        public SalesInvoice SalesInvoice { get; set; } = null!;

        public int PartId { get; set; }
        public Part Part { get; set; } = null!;
    }
}
