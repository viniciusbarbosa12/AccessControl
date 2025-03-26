using Domain.Entities;

namespace Services.Configurations.Mediator.Interfaces
{
    public interface IAccessMediator
    {
        Task HandleCardSwipeAsync(Card card, int doorNumber);
    }

}
