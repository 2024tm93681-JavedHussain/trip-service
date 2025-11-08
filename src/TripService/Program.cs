using Microsoft.EntityFrameworkCore;
using TripService.Data;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read connection string from environment or appsettings
var conn = builder.Configuration.GetConnectionString("DefaultConnection") 
           ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
           ?? "Host=localhost;Port=5432;Database=tripdb;Username=tripuser;Password=tripsecret";

builder.Services.AddDbContext<TripDbContext>(options =>
    options.UseNpgsql(conn));

builder.WebHost.UseUrls("http://0.0.0.0:8080");
var app = builder.Build();

// Run migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TripDbContext>();
    db.Database.Migrate();
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseRouting();
app.UseHttpMetrics(); // ✅ adds metrics middleware

app.MapControllers();


app.MapMetrics(); // 🔹 Exposes /metrics for Prometheus
app.MapGet("/", () => Results.Ok("Trip Service is running ✅"));
app.Run();









