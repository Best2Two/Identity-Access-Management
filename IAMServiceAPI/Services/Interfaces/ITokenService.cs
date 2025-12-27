using IAMService.Data.DTOs;
using IAMService.Data.Identities;
using System.Security.Claims;

namespace IAMService.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateAccessToken(ApplicationUserIdentity user);
        public Task<string> GenerateRefreshTokenAsync(ApplicationUserIdentity user);
        public Task<RefreshToken?> GetStoredRefreshTokenAsync(string refreshTokenString);
        public Task<bool> RevokeRefreshTokenAsync(string refreshTokenString);
    }

}
