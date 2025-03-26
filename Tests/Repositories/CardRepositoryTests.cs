using Data.Context;
using Data.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Domain.Config;

namespace AccessControl.Tests.Repositories
{
    public class CardRepositoryTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // banco novo por teste
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCard()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repo = new CardRepository(context);

            var card = new Card
            {
                Code = "Card100",
                Number = 100,
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            await repo.CreateAsync(card);
            await repo.SaveChangesAsync();

            var saved = await context.Cards.FirstOrDefaultAsync(c => c.Code == "Card100");

            // Assert
            saved.Should().NotBeNull();
            saved!.FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetByCardNumber_ShouldReturnCard()
        {
            var context = CreateInMemoryContext();
            var repo = new CardRepository(context);

            await context.Cards.AddAsync(new Card
            {
                Code = "Card123",
                Number = 123,
                FirstName = "Ana",
                LastName = "Silva"
            });
            await context.SaveChangesAsync();

            var result = await repo.GetByCardNumber(123);

            result.Should().NotBeNull();
            result!.FirstName.Should().Be("Ana");
        }

        [Fact]
        public async Task GetAllPagedAsync_ShouldReturnPagedCards()
        {
            var context = CreateInMemoryContext();
            var repo = new CardRepository(context);

            for (int i = 1; i <= 25; i++)
            {
                await context.Cards.AddAsync(new Card
                {
                    Code = $"Card{i}",
                    Number = i,
                    FirstName = $"User{i}",
                    LastName = "Test"
                });
            }
            await context.SaveChangesAsync();

            var query = new PagedQuery<object> { Page = 2, PageSize = 10 };

            var result = await repo.GetAllPagedAsync(query);

            result.Should().NotBeNull();
            result.Items.Count().Should().Be(10);
            result.TotalItems.Should().Be(25);
            result.TotalPages.Should().Be(3);
        }

        [Fact]
        public async Task DeleteAsync_ShouldMarkAsDeleted()
        {
            var context = CreateInMemoryContext();
            var repo = new CardRepository(context);

            var card = new Card
            {
                Code = "Card500",
                Number = 500,
                FirstName = "ToDelete",
                LastName = "User"
            };

            await repo.CreateAsync(card);
            await repo.SaveChangesAsync();

            await repo.DeleteAsync(card);

            var deletedCard = await context.Cards.FindAsync(card.Id);
            deletedCard!.DeletedAt.Should().NotBeNull();
        }
    }
}
