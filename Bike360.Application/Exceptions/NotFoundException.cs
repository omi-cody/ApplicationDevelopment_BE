using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Application.Exceptions
{
    public class NotFoundException(string message) : Exception(message);
}
