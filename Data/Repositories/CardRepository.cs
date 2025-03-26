using Data.Context;
using Data.Repositories.Base;
using Data.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        public CardRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Card?> GetByCardNumber(int cardNumber)
        {
            return await GetOneAsync(x => x.Number == cardNumber);
        }

    }
}
