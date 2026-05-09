using Bike360.Application.Interfaces;
using Bike360.Domain.Entities;
using Bike360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bike360.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of ICustomerRepository.
    /// Layer : Infrastructure → Repositories → CustomerRepository.cs
    /// Uses your existing AppDbContext.
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _db;

        public CustomerRepository(AppDbContext db)
        {
            _db = db;
        }

        // Customer 

        public async Task<Customer?> GetByIdAsync(Guid id) =>
            await _db.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Customer?> GetByEmailAsync(string email) =>
            await _db.Customers
                .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant());

        public async Task<Customer> AddAsync(Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        // Vehicles 

        public async Task<List<Vehicle>> GetVehiclesByCustomerAsync(Guid customerId) =>
            await _db.Vehicles
                .Where(v => v.CustomerId == customerId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

        public async Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId) =>
            await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
        {
            _db.Vehicles.Update(vehicle);
            await _db.SaveChangesAsync();
            return vehicle;
        }

        public async Task DeleteVehicleAsync(Vehicle vehicle)
        {
            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
        }

        // Sale Invoices / History 

        public async Task<List<SaleInvoice>> GetInvoicesByCustomerAsync(Guid customerId) =>
            await _db.SaleInvoices
                .Include(i => i.Items)
                .Include(i => i.Vehicle)
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();

        //  Appointments 

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid id) =>
            await _db.Appointments
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Appointment>> GetAppointmentsByCustomerAsync(Guid customerId) =>
            await _db.Appointments
                .Include(a => a.Vehicle)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

        public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
        {
            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        //  Part Requests 

        public async Task<List<PartRequest>> GetPartRequestsByCustomerAsync(Guid customerId) =>
            await _db.PartRequests
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<PartRequest> AddPartRequestAsync(PartRequest request)
        {
            _db.PartRequests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        // Reviews 
        public async Task<ServiceReview> AddReviewAsync(ServiceReview review)
        {
            _db.ServiceReviews.Add(review);
            await _db.SaveChangesAsync();
            return review;
        }

        public async Task<List<ServiceReview>> GetApprovedReviewsAsync() =>
            await _db.ServiceReviews
                .Include(r => r.Customer)
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
    }
}