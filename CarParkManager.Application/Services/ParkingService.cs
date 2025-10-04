using CarParkManager.Application.Models;
using CarParkManager.Application.Repositories;
using CarParkManager.Domain;

namespace CarParkManager.Application.Services;

public class ParkingService(
    IParkingSpaceRepository spaceRepo,
    IParkingSessionRepository sessionRepo,
    ChargeCalculator chargeCalculator) : IParkingService
{
    private readonly IParkingSpaceRepository _spaceRepo = spaceRepo;
    private readonly IParkingSessionRepository _sessionRepo = sessionRepo;
    private readonly ChargeCalculator _chargeCalculator = chargeCalculator;

    public async Task<AllocateResponse?> AllocateAsync(AllocateRequest request)
    {
        var existingSession = await _sessionRepo.GetActiveSessionAsync(request.VehicleReg);
        if (existingSession != null)
            throw new InvalidOperationException("Vehicle is already parked.");

        var space = await _spaceRepo.GetFirstAvailableSpaceAsync();
        if (space is null)
            return null;

        var vehicle = new Vehicle { VehicleReg = request.VehicleReg, VehicleType = request.VehicleType };
        var session = new ParkingSession
        {
            VehicleReg = request.VehicleReg,
            SpaceNumber = space.SpaceNumber,
            TimeIn = DateTime.UtcNow
        };

        space.IsOccupied = true;
        space.ParkedVehicle = vehicle;
        space.CurrentSession = session;

        await _spaceRepo.UpdateSpaceAsync(space);

        return new AllocateResponse
        {
            VehicleReg = request.VehicleReg,
            SpaceNumber = space.SpaceNumber,
            TimeIn = session.TimeIn
        };
    }

    public async Task<StatusResponse> GetStatusAsync()
    {
        var allSpaces = await _spaceRepo.GetAllSpacesAsync();
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
        var space = await _spaceRepo.GetSpaceByVehicleRegAsync(request.VehicleReg);
        if (space is null || space.CurrentSession is null)
            return null;

        var session = space.CurrentSession;
        session.TimeOut = DateTime.UtcNow;
        session.VehicleCharge = _chargeCalculator.Calculate(
            space.ParkedVehicle?.VehicleType ?? throw new ArgumentException("Parked vehicle is missing."),
            session.TimeIn,
            session.TimeOut.Value);

        space.IsOccupied = false;
        space.ParkedVehicle = null;
        space.CurrentSession = null;

        await _spaceRepo.UpdateSpaceAsync(space);
        await _sessionRepo.UpdateSessionAsync(session);

        return new ExitResponse
        {
            VehicleReg = request.VehicleReg,
            VehicleCharge = session.VehicleCharge ?? 0,
            TimeIn = session.TimeIn,
            TimeOut = session.TimeOut!.Value
        };
    }
}
