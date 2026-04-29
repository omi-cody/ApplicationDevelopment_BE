using Bike360.Domain.Constant;
using Bike360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Interfaces
{
    public interface ITokenService
    {
        (string token, DateTime expiresAt) GenerateToken(ApplicationUser user, IEnumerable<string> roles);

    }
}
