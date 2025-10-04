using CarParkManager.Domain;

namespace CarParkManager.Application.Repositories;

public interface IParkingSessionRepository
{
    Task<ParkingSession?> GetActiveSessionAsync(string vehicleReg);
    Task AddSessionAsync(ParkingSession session);
    Task UpdateSessionAsync(ParkingSession session);
}

