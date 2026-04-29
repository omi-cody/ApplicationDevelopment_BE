using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Domain.Entities
{
    public  class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string? ProfilePictureUrl { get; set; }


        public bool IsActive { get; set; } = true;
    }
}
