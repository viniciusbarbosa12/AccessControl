using Domain.Config;
using Domain.Entities;
using Services.Filters;

namespace Services.Interfaces
{
    public interface ICardService
    {
        Task<string> AddCard(int cardNumber, string firstName, string lastName);
        Task<bool> CancelPermission(int cardNumber, int doorNumber);
        Task<PaginatedResult<Card>> GetPagedCardsAsync(PagedQuery<CardFilter> query);
        Task<Card> GetByNumber(int cardNumber);
        Task<string> GrantAccess(int cardNumber, int doorNumber);

    }
}
