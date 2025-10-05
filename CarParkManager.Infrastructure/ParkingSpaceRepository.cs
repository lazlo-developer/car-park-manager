using CarParkManager.Application.Repositories;
using CarParkManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarParkManager.Infrastructure;

public class ParkingSpaceRepository(CarParkDbContext db) : IParkingSpaceRepository
{
    public async Task<ParkingSpace?> GetFirstAvailableSpaceAsync() =>
        await db.ParkingSpaces.FirstOrDefaultAsync(s => !s.IsOccupied);

    public async Task<ParkingSpace?> GetSpaceByVehicleRegAsync(string vehicleReg) =>
        await db.ParkingSpaces
        .Include(x => x.CurrentSession)
        .Include(x => x.ParkedVehicle)
        .FirstOrDefaultAsync(s => s.ParkedVehicle != null && s.ParkedVehicle.VehicleReg == vehicleReg);

    public async Task<List<ParkingSpace>> GetAllSpacesAsync() =>
        await db.ParkingSpaces.ToListAsync();

    public async Task UpdateSpaceAsync(ParkingSpace space)
    {
        await db.SaveChangesAsync();
    }
}
