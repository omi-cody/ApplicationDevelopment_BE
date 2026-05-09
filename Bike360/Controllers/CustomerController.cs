using Bike360.Application.DTOs;
using Bike360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bike360.Controllers
{
    /// <summary>
    /// All customer-facing REST endpoints.
    /// Layer : API (Presentation) → Controllers → CustomerController.cs
    ///
    /// Login/token-issuance is handled by your existing AuthService + TokenService.
    /// This controller only handles post-login customer operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Reads the authenticated customer's ID from the JWT "sub" claim
        private Guid GetCurrentCustomerId()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub")
                   ?? throw new UnauthorizedAccessException("Customer ID not in token.");
            return Guid.Parse(sub);
        }

        // =====================================================================
        // REGISTRATION  — Feature 12
        // =====================================================================

        /// <summary>
        /// POST api/customer/register
        /// Customer self-registers. Returns profile (token issued separately via /auth/login).
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var result = await _customerService.RegisterAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // =====================================================================
        // PROFILE  — Feature 12
        // =====================================================================

        /// <summary>GET api/customer/profile — returns logged-in customer's profile.</summary>
        [HttpGet("profile")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _customerService.GetProfileAsync(GetCurrentCustomerId());
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>PUT api/customer/profile — update name, phone, or address.</summary>
        [HttpPut("profile")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateProfile([FromBody] CustomerUpdateProfileDto dto)
        {
            var result = await _customerService.UpdateProfileAsync(GetCurrentCustomerId(), dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // =====================================================================
        // VEHICLES  — Feature 12
        // =====================================================================

        /// <summary>GET api/customer/vehicles — list my vehicles.</summary>
        [HttpGet("vehicles")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyVehicles()
        {
            var result = await _customerService.GetMyVehiclesAsync(GetCurrentCustomerId());
            return Ok(result);
        }

        /// <summary>POST api/customer/vehicles — add a vehicle.</summary>
        [HttpPost("vehicles")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var result = await _customerService.AddVehicleAsync(GetCurrentCustomerId(), dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>PUT api/customer/vehicles/{vehicleId} — update a vehicle.</summary>
        [HttpPut("vehicles/{vehicleId:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateVehicle(Guid vehicleId, [FromBody] UpdateVehicleDto dto)
        {
            var result = await _customerService.UpdateVehicleAsync(GetCurrentCustomerId(), vehicleId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>DELETE api/customer/vehicles/{vehicleId} — remove a vehicle.</summary>
        [HttpDelete("vehicles/{vehicleId:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveVehicle(Guid vehicleId)
        {
            var result = await _customerService.RemoveVehicleAsync(GetCurrentCustomerId(), vehicleId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // =====================================================================
        // PURCHASE & SERVICE HISTORY  — Feature 14
        // =====================================================================

        /// <summary>
        /// GET api/customer/history
        /// Returns the customer's complete purchase and service invoice history.
        /// </summary>
        [HttpGet("history")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPurchaseHistory()
        {
            var result = await _customerService.GetPurchaseHistoryAsync(GetCurrentCustomerId());
            return Ok(result);
        }

        // =====================================================================
        // APPOINTMENTS  — Feature 13
        // =====================================================================

        /// <summary>POST api/customer/appointments — book a service appointment.</summary>
        [HttpPost("appointments")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var result = await _customerService.BookAppointmentAsync(GetCurrentCustomerId(), dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>GET api/customer/appointments — list my appointments.</summary>
        [HttpGet("appointments")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var result = await _customerService.GetMyAppointmentsAsync(GetCurrentCustomerId());
            return Ok(result);
        }

        /// <summary>DELETE api/customer/appointments/{id} — cancel an appointment.</summary>
        [HttpDelete("appointments/{appointmentId:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            var result = await _customerService.CancelAppointmentAsync(GetCurrentCustomerId(), appointmentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // =====================================================================
        // PART REQUESTS  — Feature 13
        // =====================================================================

        /// <summary>POST api/customer/part-requests — request an out-of-stock part.</summary>
        [HttpPost("part-requests")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RequestPart([FromBody] PartRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var result = await _customerService.RequestUnavailablePartAsync(GetCurrentCustomerId(), dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>GET api/customer/part-requests — view my part request statuses.</summary>
        [HttpGet("part-requests")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPartRequests()
        {
            var result = await _customerService.GetMyPartRequestsAsync(GetCurrentCustomerId());
            return Ok(result);
        }

        // =====================================================================
        // SERVICE REVIEWS  — Feature 13
        // =====================================================================

        /// <summary>POST api/customer/reviews — submit a review (pending admin approval).</summary>
        [HttpPost("reviews")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var result = await _customerService.SubmitReviewAsync(GetCurrentCustomerId(), dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>GET api/customer/reviews — public list of approved reviews.</summary>
        [HttpGet("reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApprovedReviews()
        {
            var result = await _customerService.GetApprovedReviewsAsync();
            return Ok(result);
        }

        // =====================================================================
        // AI PART FAILURE PREDICTION  — Scenario requirement
        // =====================================================================

        /// <summary>
        /// GET api/customer/vehicles/{vehicleId}/predictions
        /// Analyses vehicle age, mileage, and service history to predict part failures.
        /// </summary>
        [HttpGet("vehicles/{vehicleId:guid}/predictions")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPartFailurePredictions(Guid vehicleId)
        {
            var result = await _customerService.GetPartFailurePredictionsAsync(
                GetCurrentCustomerId(), vehicleId);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}