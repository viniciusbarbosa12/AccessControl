using Domain.Config;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Filters;
using Services.Interfaces;

namespace AccessControl.Controllers
{
    [ApiController]
    [Route("api/cards")]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardsService;
        private readonly ILogger<CardsController> _logger;

        public CardsController(ICardService cardsService, ILogger<CardsController> logger)
        {
            _cardsService = cardsService;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCardModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
            {
                _logger.LogWarning("First name or last name is missing.");
                return BadRequest("First name and last name are required.");
            }

            try
            {
                var result = await _cardsService.AddCard(model.CardNumber, model.FirstName, model.LastName);
                _logger.LogInformation("Card created successfully. CardId: {CardId}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating card.");
                return StatusCode(500, "An unexpected error occurred while creating the card.");
            }
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPagedCards(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? firstName = null,
            [FromQuery] int? number = null)
        {
            _logger.LogInformation("[{Controller}] - Fetching paged cards. Page = {Page}, PageSize = {PageSize}, FirstName = {FirstName}, Number = {Number}",
                nameof(CardsController), page, pageSize, firstName, number);
            try
            {
                var query = new PagedQuery<CardFilter>
                {
                    Page = page,
                    PageSize = pageSize,
                    Filter = new CardFilter
                    {
                        FirstName = firstName,
                        Number = number
                    }
                };

                var result = await _cardsService.GetPagedCardsAsync(query);
                _logger.LogInformation("[{Controller}] - Retrieved {Count} cards (Page {Page} of {TotalPages})",
                nameof(CardsController), result.Items.Count(), result.Page, result.TotalPages);


                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Controller}] - Error while retrieving paged cards", nameof(CardsController));
                return StatusCode(500, "An unexpected error occurred while fetching cards.");
            }
        }


        [HttpPost("grant-access")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GrantAccess([FromQuery] int cardNumber, [FromQuery] int doorNumber)
        {
            if (cardNumber <= 0 || doorNumber <= 0)
            {
                _logger.LogWarning("Invalid cardNumber ({CardNumber}) or doorNumber ({DoorNumber}).", cardNumber, doorNumber);
                return BadRequest("Both cardNumber and doorNumber must be greater than zero.");
            }

            try
            {
                var result = await _cardsService.GrantAccess(cardNumber, doorNumber);

                if (result == "Failure")
                {
                    _logger.LogWarning("Failed to grant access. CardNumber: {CardNumber}, DoorNumber: {DoorNumber}", cardNumber, doorNumber);
                    return BadRequest("Could not grant access. Card or door not found.");
                }

                _logger.LogInformation("Access granted. CardNumber: {CardNumber}, DoorNumber: {DoorNumber}", cardNumber, doorNumber);
                return Ok("Access granted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while granting access.");
                return StatusCode(500, "An unexpected error occurred while granting access.");
            }
        }


        [HttpPost("cancel-permission")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> CancelPermission([FromQuery] int cardNumber, [FromQuery] int doorNumber)
        {
            try
            {
                var result = await _cardsService.CancelPermission(cardNumber, doorNumber);

                if (!result)
                {
                    _logger.LogWarning("Failed to cancel permission. CardNumber: {CardNumber}, DoorNumber: {DoorNumber}", cardNumber, doorNumber);
                    return NotFound("Permission not found or already revoked.");
                }

                _logger.LogInformation("Permission canceled. CardNumber: {CardNumber}, DoorNumber: {DoorNumber}", cardNumber, doorNumber);
                return Ok("Permission canceled successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while canceling permission.");
                return StatusCode(500, "An unexpected error occurred while canceling permission.");
            }
        }
    }
}
