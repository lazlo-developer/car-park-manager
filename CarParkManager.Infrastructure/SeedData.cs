using CarParkManager.Domain;
using Microsoft.Extensions.Configuration;

namespace CarParkManager.Infrastructure;

public static class SeedData
{
    public static void Seed(CarParkDbContext db, IConfiguration configuration)
    {
        var initialSpaces = configuration.GetValue<int>("DefaultParkingCapacity", 50);
        if (!db.ParkingSpaces.Any())
        {
            for (int i = 1; i <= initialSpaces; i++)
            {
                db.ParkingSpaces.Add(new ParkingSpace
                {
                    SpaceNumber = i,
                    IsOccupied = false
                });
            }
            db.SaveChanges();
        }
    }
}
