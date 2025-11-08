using System;

namespace TripService.Models
{
    public class Trip
    {
        public int TripId { get; set; }
        public int RiderId { get; set; }
        public int? DriverId { get; set; }
        public string PickupZone { get; set; } = string.Empty;
        public string DropZone { get; set; } = string.Empty;
        public string Status { get; set; } = "REQUESTED";
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public decimal DistanceKm { get; set; }
        public decimal BaseFare { get; set; }
        public decimal SurgeMultiplier { get; set; } = 1.0m;
        public decimal TotalFare { get; set; }
      
    }
}





