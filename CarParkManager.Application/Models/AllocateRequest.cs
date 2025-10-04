using CarParkManager.Domain;

namespace CarParkManager.Application.Models;

public class AllocateRequest
{
    public required string VehicleReg { get; init; }
    public required VehicleType VehicleType { get; init; }
}
