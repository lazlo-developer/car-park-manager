using System;
using System.ComponentModel.DataAnnotations;

namespace CarParkManager.Domain
{
    public class ParkingSession
    {
        [Key]
        public Guid Id { get; set; }
        public required string VehicleReg { get; set; }
        public int SpaceNumber { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public double? VehicleCharge { get; set; }
    }
}
