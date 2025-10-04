namespace CarParkManager.Domain;

public class ParkingRateSettings
{
    public double SmallCarRatePerMinute { get; set; }
    public double MediumCarRatePerMinute { get; set; }
    public double LargeCarRatePerMinute { get; set; }
    public double SurchargeEveryMinutes { get; set; } = 5;
    public double SurchargeAmount { get; set; } = 1.0;
}
