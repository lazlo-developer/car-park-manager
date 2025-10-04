using CarParkManager.Domain;

namespace CarParkManager.Application.Repositories;

public interface IParkingSpaceRepository
{
    Task<ParkingSpace?> GetFirstAvailableSpaceAsync();
    Task<ParkingSpace?> GetSpaceByVehicleRegAsync(string vehicleReg);
    Task<List<ParkingSpace>> GetAllSpacesAsync();
    Task UpdateSpaceAsync(ParkingSpace space);
}

