using Domain.Entities;
using Domain.Enum;
using Services.Factories.DoorFactory.Interfaces;

namespace Services.Factories.DoorFactory
{
    public class RegularDoorFactory : IDoorFactory
    {
        public Door Create(int doorNumber, string doorName)
        {
            return new Door
            {
                Code = $"D{doorNumber}T{(int)DoorType.Regular}",
                Number = doorNumber,
                Name = doorName,
                Description = "Regular door"
            };
        }
    }
}
