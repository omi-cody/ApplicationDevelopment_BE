using Bike360.Domain.Constant;
using Bike360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Infrastructure.Data
{
    public static class DbSeeder

    {
        private const string DefaultAdminEmail = "admin@bike360.com";
        private const string DefaultAdminPassword = "Admin@1234!";
        private const string DefaultAdminName = "System Admin";
        private const string DeafultAdminPhone = "1234567890";
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            //seed roles
            foreach (var role in Roles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }


            var exisitingAdmin = await userManager.FindByEmailAsync(DefaultAdminEmail);
            if (exisitingAdmin is null) {
                var adminUser = new ApplicationUser
                {
                    Email = DefaultAdminEmail,
                    FullName = DefaultAdminName,
                    PhoneNumber = DeafultAdminPhone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                
                };

                var result = await userManager.CreateAsync(adminUser, DefaultAdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }
        }
    }
}
