using System;
using System.Collections.Generic;
using System.Text;
using Bike360.Application.DTOs.Customer;

namespace Bike360.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> RegisterCustomerAsync(CreateCustomerRequest request);
        Task<CustomerResponse> GetCustomerByIdAsync(int id);
    }
}
