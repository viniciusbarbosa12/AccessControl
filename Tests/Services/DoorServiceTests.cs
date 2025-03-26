using Moq;
using Services.Services;
using Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.Enum;
using Services.Factories.DoorFactory;
using Services.Configurations.Mediator.Interfaces;

namespace Tests.Services
{
    public class DoorServiceTests
    {
        private readonly Mock<IDoorRepository> _mockDoorRepository;
        private readonly Mock<ILogger<DoorService>> _mockLogger;
        private readonly Mock<IAccessMediator> _mockMediator;
        private readonly DoorService _doorService;

        public DoorServiceTests()
        {
            _mockDoorRepository = new Mock<IDoorRepository>();
            _mockLogger = new Mock<ILogger<DoorService>>();
            _mockMediator = new Mock<IAccessMediator>();
            _doorService = new DoorService(_mockDoorRepository.Object, _mockLogger.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task AddDoor_ShouldReturnDoor_WhenValidInput()
        {
            // Arrange
            int doorNumber = 1;
            int doorType = (int)DoorType.Elevator;
            string doorName = "Elevador Leste";

            // Usamos o factory real para comparar
            var factory = AbstractDoorFactory.GetFactory(DoorType.Elevator);
            var expectedDoor = factory.Create(doorNumber, doorName);

            _mockDoorRepository
                .Setup(r => r.CreateAsync(It.IsAny<Door>()))
                .ReturnsAsync((Door d) => d); // Simula retorno do próprio objeto

            _mockDoorRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _doorService.AddDoor(doorNumber, doorType, doorName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDoor.Code, result.Code);
            Assert.Equal(expectedDoor.Name, result.Name);
            Assert.Equal(expectedDoor.Number, result.Number);
            Assert.Equal(expectedDoor.Description, result.Description);
            _mockDoorRepository.Verify(r => r.CreateAsync(It.IsAny<Door>()), Times.Once);
            _mockDoorRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddDoor_ShouldThrowException_WhenDoorTypeIsInvalid()
        {
            // Arrange
            int doorNumber = 10;
            int invalidDoorType = 99;
            string doorName = "Porta Bugada";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _doorService.AddDoor(doorNumber, invalidDoorType, doorName));

            // O factory vai lançar antes de chamar o repositório, então os métodos abaixo não são chamados
            _mockDoorRepository.Verify(r => r.CreateAsync(It.IsAny<Door>()), Times.Never);
            _mockDoorRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddDoor_ShouldThrowException_WhenSaveFails()
        {
            // Arrange
            int doorNumber = 2;
            string doorName = "Principal door";

            var factory = AbstractDoorFactory.GetFactory(DoorType.Regular);
            var expectedDoor = factory.Create(doorNumber, doorName);


            _mockDoorRepository
                .Setup(r => r.CreateAsync(It.IsAny<Door>()))
                .ReturnsAsync(expectedDoor);

            _mockDoorRepository
                .Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new Exception("Error in db"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _doorService.AddDoor(doorNumber, (int)DoorType.Regular, doorName));

            _mockDoorRepository.Verify(r => r.CreateAsync(It.IsAny<Door>()), Times.Once);
            _mockDoorRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}