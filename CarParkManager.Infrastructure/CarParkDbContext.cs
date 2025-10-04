using Microsoft.EntityFrameworkCore;
using CarParkManager.Domain;

namespace CarParkManager.Infrastructure;

public class CarParkDbContext(DbContextOptions<CarParkDbContext> options) : DbContext(options)
{
    public DbSet<ParkingSpace> ParkingSpaces => Set<ParkingSpace>();
    public DbSet<ParkingSession> ParkingSessions => Set<ParkingSession>();
}
