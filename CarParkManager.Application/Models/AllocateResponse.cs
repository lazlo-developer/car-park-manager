namespace CarParkManager.Application.Models;

public class AllocateResponse
{
    public required string VehicleReg { get; init; }
    public required int SpaceNumber { get; init; }
    public required DateTime TimeIn { get; init; }
}
