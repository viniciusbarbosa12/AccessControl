using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AccessControl.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
        {
            _logger.LogInformation("[{Controller}] - Attempting to register admin. Username = {Username}", nameof(AuthController), model.Username);

            try
            {
                var result = await _authService.RegisterAdminAsync(model);
                _logger.LogInformation("[{Controller}] - Admin registered successfully. Username = {Username}", nameof(AuthController), model.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Controller}] - Error while registering admin. Username = {Username}", nameof(AuthController), model.Username);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("[{Controller}] - Attempting login. Username = {Username}", nameof(AuthController), model.Username);

            var token = await _authService.LoginAsync(model);
            if (token == null)
            {
                _logger.LogWarning("[{Controller}] - Invalid login attempt. Username = {Username}", nameof(AuthController), model.Username);
                return Unauthorized();
            }

            _logger.LogInformation("[{Controller}] - Login successful. Username = {Username}", nameof(AuthController), model.Username);
            return Ok(new { token });
        }

        [HttpPost("register-user")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] LoginModel model)
        {
            _logger.LogInformation("[{Controller}] - Attempting to register user. Username = {Username}", nameof(AuthController), model.Username);

            try
            {
                var result = await _authService.RegisterUserAsync(model);
                _logger.LogInformation("[{Controller}] - User registered successfully. Username = {Username}", nameof(AuthController), model.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Controller}] - Error while registering user. Username = {Username}", nameof(AuthController), model.Username);
                return BadRequest(ex.Message);
            }
        }

    }
}
