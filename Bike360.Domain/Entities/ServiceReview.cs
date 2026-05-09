using System;

namespace Bike360.Domain.Entities
{
    
    public class ServiceReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CustomerId { get; set; }
        
        public int Rating { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = false;

        // Navigation
        public Customer Customer { get; set; } = null!;
    }
}