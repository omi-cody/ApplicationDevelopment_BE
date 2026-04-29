using Bike360.Application.DTOs.Auth;
using Bike360.Application.Exceptions;
using Bike360.Application.Interfaces;
using Bike360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService) : IAuthService
    {

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                throw new ValidationException(error);
            }

            await userManager.AddToRoleAsync(user, request.Role);

            return await GenerateAuthResponseAsync(user);

        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }
            return await GenerateAuthResponseAsync(user);
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var (token, expiresAt) = tokenService.GenerateToken(user, roles);
            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Roles = roles,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
    }
