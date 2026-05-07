using Bike360.Application.DTOs.Customer;
using Bike360.Application.Exceptions;
using Bike360.Application.Interfaces;
using Bike360.Domain.Entities;
using Bike360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bike360.Infrastructure.Services
{
    public class CustomerService(AppDbContext context) : ICustomerService
    {
        public async Task<CustomerResponse> RegisterCustomerAsync(CreateCustomerRequest request)
        {
            // Check if email already exists
            var exists = await context.Customers
                .AnyAsync(c => c.Email == request.Email);

            if (exists)
                throw new BadRequestException("A customer with this email already exists.");

            // Create customer
            var customer = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                RegisteredAt = DateTime.UtcNow
            };

            // Add vehicles to the customer
            foreach (var v in request.Vehicles)
            {
                customer.Vehicles.Add(new CustomerVehicle
                {
                    VehicleNumber = v.VehicleNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    Year = v.Year
                });
            }

            // Save to database
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return MapToResponse(customer);
        }

        public async Task<CustomerResponse> GetCustomerByIdAsync(int id)
        {
            var customer = await context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException($"Customer with ID {id} not found.");

            return MapToResponse(customer);
        }

        // Converts Customer to CustomerResponse
        private static CustomerResponse MapToResponse(Customer customer) => new()
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            RegisteredAt = customer.RegisteredAt,
            Vehicles = customer.Vehicles.Select(v => new VehicleResponse
            {
                Id = v.Id,
                VehicleNumber = v.VehicleNumber,
                Brand = v.Brand,
                Model = v.Model,
                Year = v.Year
            }).ToList()
        };
    }
}
