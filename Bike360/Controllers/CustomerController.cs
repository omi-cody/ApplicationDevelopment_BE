using Bike360.Application.DTOs.Customer;
using Bike360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bike360.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Staff,Admin")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        // POST /api/customer/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CreateCustomerRequest request)
        {
            var result = await customerService.RegisterCustomerAsync(request);
            return Ok(result);
        }

        // GET /api/customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var result = await customerService.GetCustomerByIdAsync(id);
            return Ok(result);
        }
    }
}
