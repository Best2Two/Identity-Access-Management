using IAMService.Data.DTOs;
using IAMService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register
        // URL: POST api/auth/register?email=x&username=y&password=z&phoneNumber=123
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromForm] string email,
            [FromForm] string username,
            [FromForm] string password,
            [FromForm] string phoneNumber)
        {
            // We map the raw params to the object the Service expects
            var userDto = new ApplicationUserDto
            {
                Email = email,
                Username = username,
                Password = password,
                PhoneNumber = phoneNumber
            };

            var result = await _authService.RegisterUserAsync(userDto);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }


        // SWAGGER: Shows two input fields.
        // URL: POST api/auth/login?identifier=me@test.com&password=123
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string identifier, [FromQuery] string password)
        {
            var result = await _authService.LoginUserAsync(identifier, password);
            if (!result.Success) return Unauthorized(result.Errors);
            return Ok(result);
        }

        // SWAGGER: Shows one input field.
        // URL: POST api/auth/refresh?refreshToken=abc...
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromQuery] string refreshToken)
        {
            var result = await _authService.RefreshUserAsync(refreshToken);
            if (!result.Success) return Unauthorized(result.Errors);
            return Ok(result);
        }

        // SWAGGER: Shows one input field.
        // URL: POST api/auth/logout?refreshToken=abc...
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery] string refreshToken)
        {
            var result = await _authService.LogoutUserAsync(refreshToken);
            if (!result.Success) return BadRequest(result.Errors);
            return Ok(new { message = result.Message });
        }
    }
}