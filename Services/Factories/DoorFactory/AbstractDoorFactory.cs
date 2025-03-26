using Domain.Enum;
using Services.Factories.DoorFactory.Interfaces;

namespace Services.Factories.DoorFactory
{
    public static class AbstractDoorFactory
    {
        public static IDoorFactory GetFactory(DoorType type)
        {
            return type switch
            {
                DoorType.Regular => new RegularDoorFactory(),
                DoorType.Elevator => new ElevatorDoorFactory(),
                DoorType.Tripod => new TripodDoorFactory(),
                _ => throw new ArgumentException("Unsupported door type")
            };
        }
    }
}
