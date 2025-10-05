using CarParkManager.Application.Models;
using CarParkManager.Application.Repositories;
using CarParkManager.Domain;

namespace CarParkManager.Application.Services;

public class ParkingService(
    IParkingSpaceRepository spaceRepo,
    IParkingSessionRepository sessionRepo,
    IChargeCalculator chargeCalculator) : IParkingService
{
    public async Task<AllocateResponse?> AllocateAsync(AllocateRequest request)
    {
        var existingSession = await sessionRepo.GetActiveSessionAsync(request.VehicleReg);
        if (existingSession != null)
            throw new InvalidOperationException("Vehicle is already parked.");

        var space = await spaceRepo.GetFirstAvailableSpaceAsync();
        if (space is null)
            return null;

        var vehicle = new Vehicle { VehicleReg = request.VehicleReg, VehicleType = request.VehicleType };
        var session = new ParkingSession
        {
            VehicleReg = request.VehicleReg,
            SpaceNumber = space.SpaceNumber,
            TimeIn = DateTime.UtcNow
        };
        await sessionRepo.AddSessionAsync(session);

        space.IsOccupied = true;
        space.ParkedVehicle = vehicle;
        space.CurrentSession = session;

        await spaceRepo.UpdateSpaceAsync(space);

        return new AllocateResponse
        {
            VehicleReg = request.VehicleReg,
            SpaceNumber = space.SpaceNumber,
            TimeIn = session.TimeIn
        };
    }

    public async Task<StatusResponse> GetStatusAsync()
    {
        var allSpaces = await spaceRepo.GetAllSpacesAsync();
        int available = allSpaces.Count(s => !s.IsOccupied);
        int occupied = allSpaces.Count(s => s.IsOccupied);
        return new StatusResponse
        {
            AvailableSpaces = available,
            OccupiedSpaces = occupied
        };
    }

    public async Task<ExitResponse?> ExitAsync(ExitRequest request)
    {
        var space = await spaceRepo.GetSpaceByVehicleRegAsync(request.VehicleReg);
        if (space is null || space.CurrentSession is null)
            return null;

        var session = space.CurrentSession;
        session.TimeOut = DateTime.UtcNow;
        session.VehicleCharge = chargeCalculator.Calculate(
            space.ParkedVehicle?.VehicleType ?? throw new ArgumentException("Parked vehicle is missing."),
            session.TimeIn,
            session.TimeOut.Value);

        space.IsOccupied = false;
        space.ParkedVehicle = null;
        space.CurrentSession = null;

        await spaceRepo.UpdateSpaceAsync(space);
        await sessionRepo.UpdateSessionAsync(session);

        return new ExitResponse
        {
            VehicleReg = request.VehicleReg,
            VehicleCharge = session.VehicleCharge ?? 0,
            TimeIn = session.TimeIn,
            TimeOut = session.TimeOut!.Value
        };
    }
}
