using Bike360.Application.DTOs.Auth;
using Bike360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bike360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await authService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await authService.LoginAsync(request);
            return Ok(response);
        }


        [HttpGet("me")]
        [Authorize]
        public IActionResult Me([FromServices] ICurrentUser currentUser)

        {
            return Ok(new
            {
                userId = currentUser.UserId,
                email = currentUser.Email,
                isCustomer = currentUser.IsInRole("Customer"),
                isAdmin = currentUser.IsInRole("Admin"),
                isStaff = currentUser.IsInRole("Staff")

            });


        }
    }
}
