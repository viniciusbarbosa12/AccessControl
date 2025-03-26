using Data.Repositories.Interfaces;
using Domain.Config;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Services.Filters;
using Services.Interfaces;

namespace Services.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly ILogger<CardService> _logger;

        public CardService(ICardRepository cardRepository, ILogger<CardService> logger)
        {
            _cardRepository = cardRepository;
            _logger = logger;
        }

        public async Task<string> AddCard(int cardNumber, string firstName, string lastName)
        {
            _logger.LogInformation($"[{nameof(Card)}] - Starting Card creation. cardNumber = {cardNumber}, firstName = {firstName} and lastName = {lastName}");
            Card card = new Card()
            {
                Code = "Card" + cardNumber,
                Number = cardNumber,
                FirstName = firstName,
                LastName = lastName,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(365)
            };

            try
            {
                await _cardRepository.CreateAsync(card);
                await _cardRepository.SaveChangesAsync();

                _logger.LogInformation($"[{nameof(Card)}] - Card successfully created. CardId = {card.Id}");

                return card.Code;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating card. CardNumber = {cardNumber}");
                throw;
            }
        }

        public async Task<string> GrantAccess(int cardNumber, int doorNumber)
        {
            try
            {
                _logger.LogInformation($"[{nameof(CardService)}] - Starting Grant Access. cardNumber = {cardNumber}, doorNumber = {doorNumber}");

                var card = await _cardRepository.GetByCardNumber(cardNumber);

                if (card == null)
                {
                    _logger.LogError($"[{nameof(CardService)}] - Card not found. cardNumber = {cardNumber}");
                    return "Failure";
                }

                card.AccessibleDoorNumbers.Add(doorNumber);
                _cardRepository.Update(card);
                card.MarkAsUpdated();
                await _cardRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(CardService)}] - An error occurred while updating card. cardNumber = {cardNumber}, doorNumber = {doorNumber}");

                return "Failure";
            }

            return "Success";
        }

        public async Task<bool> CancelPermission(int cardNumber, int doorNumber)
        {
            try
            {
                _logger.LogInformation($"[{nameof(CardService)}] - Canceling permission. cardNumber = {cardNumber}, doorNumber = {doorNumber}");

                var card = await _cardRepository.GetByCardNumber(cardNumber);
                if (card == null)
                {
                    _logger.LogWarning($"Card not found: {cardNumber}");
                    return false;
                }

                if (!card.AccessibleDoorNumbers.Contains(doorNumber))
                {
                    _logger.LogWarning($"Card {cardNumber} does not have access to door {doorNumber}");
                    return false;
                }

                card.AccessibleDoorNumbers.Remove(doorNumber);
                _cardRepository.Update(card);
                await _cardRepository.SaveChangesAsync();

                _logger.LogInformation($"Permission removed: cardNumber = {cardNumber}, doorNumber = {doorNumber}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while canceling permission. cardNumber = {cardNumber}, doorNumber = {doorNumber}");
                return false;
            }
        }

        public async Task<Card?> GetByNumber(int cardNumber)
        {
            try
            {
                _logger.LogInformation($"[{nameof(CardService)}] - Retrieving card by number: {cardNumber}");
                return await _cardRepository.GetByCardNumber(cardNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(CardService)}] - Error retrieving card {cardNumber}");
                throw;
            }
        }

        public async Task<PaginatedResult<Card>> GetPagedCardsAsync(PagedQuery<CardFilter> query)
        {
            return await _cardRepository.GetAllPagedAsync(query, (cards, filter) =>
            {
                if (!string.IsNullOrWhiteSpace(filter?.FirstName))
                    cards = cards.Where(c => c.FirstName.Contains(filter.FirstName));

                if (filter?.Number != null)
                    cards = cards.Where(c => c.Number == filter.Number);

                return cards;
            });
        }
    }
}
