using IAMService.Data.DTOs;
using IAMService.Data.Entities;

namespace IAMService.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> RegisterUserAsync(ApplicationUserDto user);
        Task<AuthenticationResult> LoginUserAsync(string loginIdentifier, string password);
        Task<AuthenticationResult> RefreshUserAsync(string refreshTokenString);
        Task<AuthenticationResult> LogoutUserAsync(string refreshTokenString);
    }
}