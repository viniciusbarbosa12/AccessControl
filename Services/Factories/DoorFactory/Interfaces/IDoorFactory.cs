using Domain.Entities;

namespace Services.Factories.DoorFactory.Interfaces
{
    public interface IDoorFactory
    {
        Door Create(int doorNumber, string doorName);
    }
}
