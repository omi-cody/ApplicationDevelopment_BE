using Bike360.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
         Task<AuthResponse> RegisterAsync(RegisterRequest request);
    }
}
