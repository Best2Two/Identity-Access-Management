using IAMService.Data.DTOs;
using IAMService.Data.Identities;
using System.Security.Claims;

namespace IAMService.Services.Interfaces
{
    public interface ITokenService
    {
        // Generates a JWT string for a given user.
        public string GenerateAccessToken(ApplicationUserIdentity user);
        public Task<string> GenerateRefreshTokenAsync(string userId);

        public Task<string> RefreshAccessToken(string securityToken, string refreshToken);

    }

}
