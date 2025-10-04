using System;

namespace CarParkManager.Domain
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string VehicleReg { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
    }
}
