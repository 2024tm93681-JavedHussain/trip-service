using Microsoft.EntityFrameworkCore;
using TripService.Models;

namespace TripService.Data
{
    public class TripDbContext : DbContext
    {
        public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }

        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("trips");
                entity.HasKey(t => t.TripId);
                entity.Property(t => t.TripId).HasColumnName("trip_id").ValueGeneratedOnAdd();
                entity.Property(t => t.RiderId).HasColumnName("rider_id").IsRequired();
                entity.Property(t => t.DriverId).HasColumnName("driver_id");
                entity.Property(t => t.PickupZone).HasColumnName("pickup_zone").HasMaxLength(100).IsRequired();
                entity.Property(t => t.DropZone).HasColumnName("drop_zone").HasMaxLength(100).IsRequired();
                entity.Property(t => t.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("REQUESTED");
                entity.Property(t => t.RequestedAt).HasColumnName("requested_at").IsRequired();
                entity.Property(t => t.DistanceKm).HasColumnName("distance_km").IsRequired();
                entity.Property(t => t.BaseFare).HasColumnName("base_fare").IsRequired();
                entity.Property(t => t.SurgeMultiplier).HasColumnName("surge_multiplier").HasDefaultValue(1.0m);
                entity.Property(t => t.TotalFare).HasColumnName("total_fare").IsRequired();
            });
        }
    }
}
