using Domain.Entities;
using Domain.Enum;
using Services.Factories.DoorFactory.Interfaces;

namespace Services.Factories.DoorFactory
{
    public class TripodDoorFactory : IDoorFactory
    {
        public Door Create(int doorNumber, string doorName)
        {
            return new Door
            {
                Code = $"D{doorNumber}T{(int)DoorType.Tripod}",
                Number = doorNumber,
                Name = doorName,
                Description = "Tripod"
            };
        }
    }
}
