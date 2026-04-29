using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Exceptions
{
    public class BadRequestException(string message) : Exception(message);
    
}
