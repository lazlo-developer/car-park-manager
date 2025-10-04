using System.ComponentModel.DataAnnotations;

namespace CarParkManager.Domain
{
    public class ParkingSpace
    {
        [Key]
        public int SpaceNumber { get; set; }
        public bool IsOccupied { get; set; }
        public Vehicle? ParkedVehicle { get; set; }
        public ParkingSession? CurrentSession { get; set; }
    }
}
