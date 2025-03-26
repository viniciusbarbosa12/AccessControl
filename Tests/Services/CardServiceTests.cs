using Data.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Configurations.Mediator.Interfaces;
using Services.Services;

namespace Tests.Services
{
    public class CardServiceTests
    {
        private readonly Mock<ICardRepository> _mockRepository;
        private readonly Mock<ILogger<CardService>> _mockLogger;
        private readonly CardService _cardService;
        private readonly Mock<IAccessMediator> _mockMediator;

        public CardServiceTests()
        {
            _mockRepository = new Mock<ICardRepository>();
            _mockMediator = new Mock<IAccessMediator>();
            _mockLogger = new Mock<ILogger<CardService>>();
            _cardService = new CardService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddCard_ShouldReturnCardId_WhenSuccess()
        {
            // Arrange
            int cardNumber = 123;
            string firstName = "John";
            string lastName = "Doe";
            var cardMoq = new Card
            {
                FirstName = firstName,
                LastName = lastName,
                Number = cardNumber,
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Card>())).ReturnsAsync(cardMoq);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _cardService.AddCard(cardNumber, firstName, lastName);

            // Assert
            Assert.Equal("Card123", result);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddCard_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            int cardNumber = 123;
            string firstName = "John";
            string lastName = "Doe";

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Card>())).ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _cardService.AddCard(cardNumber, firstName, lastName));
        }


        [Fact]
        public async Task GrantAccess_ShouldReturnSuccess_WhenCardExists()
        {
            // Arrange
            int cardNumber = 123;
            int doorNumber = 10;
            var card = new Card
            {
                Number = cardNumber,
                FirstName = "John",
                LastName = "Doe",
                AccessibleDoorNumbers = new List<int>()
            };

            _mockRepository
                .Setup(r => r.GetByCardNumber(cardNumber))
                .ReturnsAsync(card);

            _mockRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cardService.GrantAccess(cardNumber, doorNumber);

            // Assert
            Assert.Equal("Success", result);
            Assert.Contains(doorNumber, card.AccessibleDoorNumbers);
            _mockRepository.Verify(r => r.Update(card), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }



        [Fact]
        public async Task GrantAccess_ShouldReturnFailure_WhenCardNotFound()
        {
            // Arrange
            int cardNumber = 123;
            int doorNumber = 10;

            _mockRepository.Setup(r => r.GetByCardNumber(cardNumber)).ReturnsAsync((Card?)null);

            // Act
            var result = await _cardService.GrantAccess(cardNumber, doorNumber);

            // Assert
            Assert.Equal("Failure", result);
            _mockRepository.Verify(r => r.Update(It.IsAny<Card>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }


    }
}
