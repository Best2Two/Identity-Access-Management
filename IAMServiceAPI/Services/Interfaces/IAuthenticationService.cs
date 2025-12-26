using IAMService.Data.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IAMService.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task <AuthenticationResult> RegisterUserAsync(ApplicationUser user);
        Task<AuthenticationResult> LoginUserAsync(string loginIdentifier, string password);
        Task<AuthenticationResult> AuthenticateUserRequest(string securityToken);
        Task<AuthenticationResult> RefreshUserAsync(string refreshToken);
        Task<AuthenticationResult> LogoutUserAsync(string refreshToken);
    }

}
