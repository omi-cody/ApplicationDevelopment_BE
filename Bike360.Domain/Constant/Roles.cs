using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Domain.Constant
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "Staff";
        public const string Customer = "Customer";

        public static readonly string[] AllRoles = new[] { Admin, User, Customer };
    }
}
