using Microsoft.Extensions.Options;

namespace CarParkManager.Domain;

public class ChargeCalculator(IOptions<ParkingRateSettings> settings) : IChargeCalculator
{
    private readonly ParkingRateSettings _settings = settings.Value;

    public double Calculate(VehicleType vehicleType, DateTime timeIn, DateTime timeOut)
    {
        var minutes = Math.Max(1, (int)Math.Ceiling((timeOut - timeIn).TotalMinutes));
        double rate = vehicleType switch
        {
            VehicleType.Small => _settings.SmallCarRatePerMinute,
            VehicleType.Medium => _settings.MediumCarRatePerMinute,
            VehicleType.Large => _settings.LargeCarRatePerMinute,
            _ => _settings.SmallCarRatePerMinute
        };
        var charge = minutes * rate;
        var extra = Math.Floor(minutes / _settings.SurchargeEveryMinutes) * _settings.SurchargeAmount;
        return Math.Round(charge + extra, 2);
    }
}
