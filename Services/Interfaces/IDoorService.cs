using Domain.Entities;

namespace Services.Interfaces
{
    public interface IDoorService
    {
        Task<Door> AddDoor(int doorNumber, int doorType, string doorName);
        Task<bool> RemoveDoor(int doorNumber);
        Task SimulateCardSwipe(Card card, int doorNumber);
    }
}
