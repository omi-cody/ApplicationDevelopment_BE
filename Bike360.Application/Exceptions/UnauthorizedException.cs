using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Exceptions
{
    public class UnauthorizedException(string message)   : Exception(message);
}
