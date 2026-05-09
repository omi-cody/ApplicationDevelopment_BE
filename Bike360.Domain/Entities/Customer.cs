using System;
using System.Collections.Generic;

namespace Bike360.Domain.Entities
{
    
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        
        public string ApplicationUserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public decimal OutstandingCredit { get; set; } = 0;

        // Navigation properties
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<SaleInvoice> SaleInvoices { get; set; } = new List<SaleInvoice>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
        public ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();
    }
}