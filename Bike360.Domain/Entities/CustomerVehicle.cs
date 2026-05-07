using System;
using System.Collections.Generic;
using System.Text;

namespace Bike360.Domain.Entities
{
    public class CustomerVehicle
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
