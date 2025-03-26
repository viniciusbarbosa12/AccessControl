using Data.Repositories.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Microsoft.Extensions.Logging;
using Services.Configurations.Mediator.Interfaces;
using Services.Factories.DoorFactory;
using Services.Interfaces;

namespace Services.Services
{
    public class DoorService : IDoorService
    {
        private readonly ILogger<DoorService> _logger;
        private readonly IDoorRepository _doorRepository;
        private readonly IAccessMediator _accessMediator;

        public DoorService(IDoorRepository doorRepository, ILogger<DoorService> logger, IAccessMediator accessMediator)
        {
            _doorRepository = doorRepository;
            _logger = logger;
            _accessMediator = accessMediator;
        }

        public async Task<Door> AddDoor(int doorNumber, int doorType, string doorName)
        {
            _logger.LogInformation($"[{nameof(DoorService)}] - Starting door creating. doorNumber - {doorNumber}, doorType - {doorType}, doorName - {doorName}");

            var factory = AbstractDoorFactory.GetFactory((DoorType)doorType);
            var door = await _doorRepository.CreateAsync(factory.Create(doorNumber, doorName));
            try
            {
                await _doorRepository.SaveChangesAsync();
                _logger.LogInformation($"[{nameof(DoorService)}] - Door successfuly created. doorId - {door.Id}");

                return door;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"[{nameof(DoorService)}] - An error ocurred while creating door. doorId - {door.Id}");
                throw;
            }
        }

        public async Task<bool> RemoveDoor(int doorNumber)
        {
            try
            {
                _logger.LogInformation("[{Service}] - Attempting to remove door. DoorNumber = {DoorNumber}", nameof(DoorService), doorNumber);

                var door = await _doorRepository.GetOneAsync(d => d.Number == doorNumber);

                if (door == null)
                {
                    _logger.LogWarning("[{Service}] - Door not found. DoorNumber = {DoorNumber}", nameof(DoorService), doorNumber);
                    return false;
                }

                door.MarkAsDeleted();
                _doorRepository.Update(door);
                await _doorRepository.SaveChangesAsync();

                _logger.LogInformation("[{Service}] - Door removed successfully. DoorNumber = {DoorNumber}", nameof(DoorService), doorNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Service}] - Error while removing door. DoorNumber = {DoorNumber}", nameof(DoorService), doorNumber);
                return false;
            }
        }

        public async Task SimulateCardSwipe(Card card, int doorNumber)
        {
            await _accessMediator.HandleCardSwipeAsync(card, doorNumber);
        }
    }
}
