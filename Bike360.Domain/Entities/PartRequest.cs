using System;

namespace Bike360.Domain.Entities
{
    
    public class PartRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CustomerId { get; set; }

        public string PartName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int QuantityNeeded { get; set; } = 1;

        public PartRequestStatus Status { get; set; } = PartRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Customer Customer { get; set; } = null!;
    }

    public enum PartRequestStatus
    {
        Pending,
        Sourced,
        Available,
        Rejected
    }
}