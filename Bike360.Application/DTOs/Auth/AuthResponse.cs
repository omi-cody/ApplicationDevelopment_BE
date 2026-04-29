using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.DTOs.Auth
{
    public  class AuthResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = [];
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

    }
}
