using CarParkManager.Domain;
using Microsoft.Extensions.Options;
using Shouldly;

namespace CarParkManager.Tests;

public class ChargeCalculatorTests
{
    [Theory]
    // 10 minutes: 2 surcharges (at 5 and 10), Small: (10*0.10)+2*1=3.0, Medium: (10*0.20)+2*1=4.0, Large: (10*0.40)+2*1=6.0
    [InlineData(VehicleType.Small, 10, 3.0)]
    [InlineData(VehicleType.Medium, 10, 4.0)]
    [InlineData(VehicleType.Large, 10, 6.0)]
    public void CalculatesChargeCorrectly(VehicleType type, int minutes, double expected)
    {
        var settings = new ParkingRateSettings
        {
            SmallCarRatePerMinute = 0.10,
            MediumCarRatePerMinute = 0.20,
            LargeCarRatePerMinute = 0.40,
            SurchargeEveryMinutes = 5,
            SurchargeAmount = 1.0
        };
        var calc = new ChargeCalculator(Options.Create(settings));
        var timeIn = DateTime.UtcNow;
        var timeOut = timeIn.AddMinutes(minutes);
        var charge = calc.Calculate(type, timeIn, timeOut);
        charge.ShouldBe(expected, 0.01);
    }
}
