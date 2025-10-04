using CarParkManager.Application.Repositories;
using CarParkManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarParkManager.Infrastructure;

public class ParkingSessionRepository(CarParkDbContext db) : IParkingSessionRepository
{
    public async Task<ParkingSession?> GetActiveSessionAsync(string vehicleReg) =>
        await db.ParkingSessions.FirstOrDefaultAsync(s => s.VehicleReg == vehicleReg && s.TimeOut == null);

    public async Task AddSessionAsync(ParkingSession session)
    {
        db.ParkingSessions.Add(session);
        await db.SaveChangesAsync();
    }

    public async Task UpdateSessionAsync(ParkingSession session)
    {
        await db.SaveChangesAsync();
    }
}
