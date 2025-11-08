namespace TripService.Models
{
    public class CreateTripRequest
    {
        public int RiderId { get; set; }
        public string PickupZone { get; set; } = string.Empty;
        public string DropZone { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }    // nullable
        public decimal? BaseFare { get; set; }      // nullable
    }
}
