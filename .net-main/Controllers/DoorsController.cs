using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace AccessControl.Controllers
{
    [ApiController]
    [Route("api/doors")]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorService _doorService;
        private readonly ILogger<DoorsController> _logger;

        public DoorsController(IDoorService doorService, ILogger<DoorsController> logger)
        {
            _doorService = doorService;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> CreateDoor([FromQuery] int doorNumber, [FromQuery] int doorType, [FromQuery] string doorName)
        {
            if (doorNumber <= 0 || doorType < 0 || string.IsNullOrWhiteSpace(doorName))
            {
                _logger.LogWarning("Invalid input for door creation.");
                return BadRequest("All parameters are required and must be valid.");
            }

            try
            {
                var result = await _doorService.AddDoor(doorNumber, doorType, doorName);
                _logger.LogInformation("Door created successfully. DoorCode: {DoorCode}", result.Code);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid door type provided.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating door.");
                return StatusCode(500, "An unexpected error occurred while creating the door.");
            }
        }

        [HttpDelete("remove")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> DeleteDoor([FromQuery] int doorNumber)
        {
            if (doorNumber <= 0)
            {
                _logger.LogWarning("Invalid door number provided.");
                return BadRequest("Door number must be greater than zero.");
            }

            try
            {
                var result = await _doorService.RemoveDoor(doorNumber);

                if (!result)
                {
                    _logger.LogWarning("Door not found or already removed. DoorNumber: {DoorNumber}", doorNumber);
                    return NotFound("Door not found.");
                }

                _logger.LogInformation("Door removed successfully. DoorNumber: {DoorNumber}", doorNumber);
                return Ok("Door removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while removing door.");
                return StatusCode(500, "An unexpected error occurred while removing the door.");
            }
        }

        [HttpPost("swipe")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> SimulateSwipe([FromBody] Card card, [FromQuery] int doorNumber)
        {
            if (card == null || doorNumber <= 0)
            {
                _logger.LogWarning("Invalid input for card swipe.");
                return BadRequest("Valid card and door number are required.");
            }

            try
            {
                await _doorService.SimulateCardSwipe(card, doorNumber);
                return Ok("Swipe simulated. Check logs for result.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during card swipe simulation.");
                return StatusCode(500, "An error occurred during card swipe simulation.");
            }
        }

    }
}
