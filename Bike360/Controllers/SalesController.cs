using Bike360.Application.DTOs.Sales;
using Bike360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bike360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Staff,Admin")]
    public class SalesController(ISalesService salesService) : ControllerBase
    {
        // FEATURE 2: POST /api/sales/invoice
        [HttpPost("invoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateSalesInvoiceRequest request)
        {
            var result = await salesService.CreateInvoiceAsync(request);
            return Ok(result);
        }

        // FEATURE 3: POST /api/sales/invoice/{id}/send-email
        [HttpPost("invoice/{id}/send-email")]
        public async Task<IActionResult> SendInvoiceEmail(int id)
        {
            await salesService.SendInvoiceEmailAsync(id);
            return Ok(new { message = "Invoice emailed successfully!" });
        }
    }
}