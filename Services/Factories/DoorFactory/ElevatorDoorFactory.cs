using Domain.Entities;
using Domain.Enum;
using Services.Factories.DoorFactory.Interfaces;

namespace Services.Factories.DoorFactory
{
    public class ElevatorDoorFactory : IDoorFactory
    {
        public Door Create(int doorNumber, string doorName)
        {
            return new Door
            {
                Code = $"D{doorNumber}T{(int)DoorType.Elevator}",
                Number = doorNumber,
                Name = doorName,
                Description = "Elevator"
            };
        }
    }
}
