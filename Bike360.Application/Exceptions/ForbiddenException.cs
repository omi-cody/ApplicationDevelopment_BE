using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Exceptions
{
    public class ForbiddenException(string message) : Exception(message);
}
