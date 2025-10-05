using CarParkManager.Domain;

namespace CarParkManager.Application.Models;

using System.Text.Json.Serialization;

public class AllocateRequest
{
    public required string VehicleReg { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required VehicleType VehicleType { get; init; }
}
