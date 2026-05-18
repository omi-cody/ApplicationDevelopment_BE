using Bike360.Application.DTOs.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Interfaces
{
    public interface ISalesService
    {
        Task<SalesInvoiceResponse> CreateInvoiceAsync(CreateSalesInvoiceRequest request);
        Task SendInvoiceEmailAsync(int invoiceId);
    }
}
