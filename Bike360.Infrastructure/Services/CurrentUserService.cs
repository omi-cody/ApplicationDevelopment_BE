using Bike360.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Infrastructure.Services
{
    public class CurrentUserService(IHttpContextAccessor   httpContextAccessor) : ICurrentUser
    {
        public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        public string? Email => httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        public bool isAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}
