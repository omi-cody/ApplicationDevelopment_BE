using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Bike360.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException(IDictionary<string, string[]> errors) : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }

        public ValidationException(string field, string errorMessage) : base("One or more validation errors occurred.")
        {
            Errors = new Dictionary<string, string[]>
            {
                [field] = [errorMessage]
            };
        }
    }
}
