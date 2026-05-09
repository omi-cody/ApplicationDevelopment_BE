using Bike360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bike360.Application.Interfaces
{
   
    public interface ICustomerRepository
    {
        // Customer
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);

        // Vehicles
        Task<List<Vehicle>> GetVehiclesByCustomerAsync(Guid customerId);
        Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(Vehicle vehicle);

        // Sale Invoices
        Task<List<SaleInvoice>> GetInvoicesByCustomerAsync(Guid customerId);

        // Appointments
        Task<Appointment?> GetAppointmentByIdAsync(Guid id);
        Task<List<Appointment>> GetAppointmentsByCustomerAsync(Guid customerId);
        Task<Appointment> AddAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);

        // Part Requests
        Task<List<PartRequest>> GetPartRequestsByCustomerAsync(Guid customerId);
        Task<PartRequest> AddPartRequestAsync(PartRequest request);

        // Reviews
        Task<ServiceReview> AddReviewAsync(ServiceReview review);
        Task<List<ServiceReview>> GetApprovedReviewsAsync();
    }
}