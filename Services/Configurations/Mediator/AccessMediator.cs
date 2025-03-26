using Data.Repositories.Interfaces;
using Domain.Entities;
using Services.Configurations.Mediator.Interfaces;

public class AccessMediator : IAccessMediator
{
    private readonly IDoorRepository _doorRepository;

    public AccessMediator(IDoorRepository doorRepository)
    {
        _doorRepository = doorRepository;
    }

    public async Task HandleCardSwipeAsync(Card card, int doorNumber)
    {
        var door = await _doorRepository.GetOneAsync(d => d.Number == doorNumber);

        if (door == null)
        {
            Console.WriteLine(" Door not found.");
            return;
        }

        if (card.AccessibleDoorNumbers.Contains(doorNumber))
        {
            Console.WriteLine($" Access granted. Opening door {doorNumber} ({door.Name})");
        }
        else
        {
            Console.WriteLine($" Access denied. Card {card.Code} does not have access to door {doorNumber}");
        }
    }
}
