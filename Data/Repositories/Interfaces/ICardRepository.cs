using Data.Repositories.Base;
using Domain.Entities;

namespace Data.Repositories.Interfaces
{
    public interface ICardRepository : IRepository<Card>
    {
        Task<Card?> GetByCardNumber(int cardNumber);
    }
}
