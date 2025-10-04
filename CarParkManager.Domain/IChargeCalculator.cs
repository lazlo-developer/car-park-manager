namespace CarParkManager.Domain;

public interface IChargeCalculator
{
    double Calculate(VehicleType vehicleType, DateTime timeIn, DateTime timeOut);
}
