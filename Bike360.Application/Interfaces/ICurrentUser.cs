using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Interfaces
{
    public interface ICurrentUser
    {

        string? UserId { get; }
        string? Email { get; }

        bool isAuthenticated { get; }
        bool IsInRole(string role);

    }
}
