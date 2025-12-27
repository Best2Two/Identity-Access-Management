using IAMService.Data.DTOs;
using IAMService.Data.DTOs.Controllers.Request;
using IAMService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IAMService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ICredentialService _credService;

        public AuthController(IAuthenticationService authService, ICredentialService credService)
        {
            _authService = authService;
            _credService = credService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var userDto = new ApplicationUserDto
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _authService.RegisterUserAsync(userDto);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginUserAsync(request.Identifier, request.Password);
            if (!result.Success) return Unauthorized(result.Errors);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutRequestDto request)
        {
            var result = await _authService.LogoutUserAsync(request.RefreshTokenString);
            if (!result.Success) return BadRequest(result.Errors);
            return Ok(new { message = result.Message });
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            var result = await _authService.RefreshUserAsync(request.RefreshTokenString);
            if (!result.Success) return Unauthorized(result.Errors);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            // Just get the user ID from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Generic errors needs to be fixed

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "NON_VALID_OPERATION" });
            }

            var passwordChanged = await _credService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (passwordChanged.Succeeded)
            {
                return Ok(new { message = "USER_PASSWORD_CHANGED" });
            }

            return Unauthorized(new { message = "NON_VALID_OPERATION" });
        }

    }
}