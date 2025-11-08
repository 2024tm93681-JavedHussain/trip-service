using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripService.Data;
using TripService.Models;
using Prometheus;

namespace TripService.Controllers
{
    [ApiController]
    [Route("api/v1/trips")]
    public class TripsController : ControllerBase
    {
        private readonly TripDbContext _context;

        private static readonly Counter TripRequests = Metrics.CreateCounter(
            "trip_requests_total", "Number of trip API requests",
            new CounterConfiguration { LabelNames = new[] { "endpoint", "method", "status" } });

        private static readonly Histogram RequestDuration = Metrics.CreateHistogram(
            "trip_request_duration_seconds", "Request duration in seconds",
            new HistogramConfiguration { LabelNames = new[] { "endpoint", "method" } });

        public TripsController(TripDbContext context)
        {
            _context = context;
        }

        // ✅ GET all trips
        [HttpGet]
        public async Task<IActionResult> GetAllTrips()
        {
            using (RequestDuration.WithLabels("get_all_trips", "GET").NewTimer())
            {
                var trips = await _context.Trips.ToListAsync();
                TripRequests.WithLabels("get_all_trips", "GET", "200").Inc();
                return Ok(trips);
            }
        }

        // ✅ POST create trip
        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            var trip = new Trip
            {
                PickupZone = request.PickupZone,
                DropZone = request.DropZone,
                BaseFare = request.BaseFare ?? 0m,
                DistanceKm = request.DistanceKm ?? 0m,
                Status = "REQUESTED",
                RequestedAt = DateTime.UtcNow,
                RiderId = request.RiderId
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllTrips), new { id = trip.TripId }, trip);
        }

        // ✅ POST accept trip
        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptTrip(int id, [FromBody] DriverActionRequest request)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound(new { message = "Trip not found" });

            if (trip.Status != "ASSIGNED" && trip.Status != "REQUESTED")
                return Conflict(new { message = "Trip cannot be accepted" });

            trip.DriverId = request.DriverId;
            trip.Status = "ACCEPTED";

            await _context.SaveChangesAsync();
            return Ok(trip);
        }

        // ✅ POST complete trip
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTrip(int id, [FromBody] CompleteTripRequest request)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound();

            if (trip.Status != "ONGOING")
                return Conflict(new { message = "Trip not in progress" });

            trip.DistanceKm = request.DistanceKm ?? trip.DistanceKm;
            trip.Status = "COMPLETED";

            var perKmRate = 10m;
            var total = trip.BaseFare + (trip.DistanceKm * perKmRate * trip.SurgeMultiplier);
            trip.TotalFare = total;

            await _context.SaveChangesAsync();
            return Ok(trip);
        }
    }

    // Helper request models
    public class CreateTripRequest
    {
        public string PickupZone { get; set; } = string.Empty;
        public string DropZone { get; set; } = string.Empty;
        public decimal? BaseFare { get; set; }
        public decimal? DistanceKm { get; set; }
        public int RiderId { get; set; }
    }

    public class DriverActionRequest
    {
        public int DriverId { get; set; }
    }

    public class CompleteTripRequest
    {
        public decimal? DistanceKm { get; set; }
    }
}
