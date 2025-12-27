using IAMService.Data;
using IAMService.Data.Entities;
using IAMService.Data.Identities;
using IAMService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IAMService.Services
{
    public class JwtTokenService: ITokenService
    {

        private readonly JwtSettings _jwtsettings;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;

        private readonly AppDbContext _context;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings,AppDbContext context)
        {
            _jwtsettings = jwtSettings.Value;
            _securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtsettings.Secret)

            );
            _signingCredentials = new SigningCredentials(
          _securityKey,
          SecurityAlgorithms.HmacSha256
      );
            _context = context;
        }
        
        //Should unify the returning signature
        public string GenerateAccessToken(ApplicationUserIdentity user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, user.Role)
                    ]),

                Expires = DateTime.UtcNow.AddMinutes(_jwtsettings.ExpiryMinutes),
                SigningCredentials = _signingCredentials,
                Issuer = _jwtsettings.Issuer,
                Audience = _jwtsettings.Audience
            };

            var jwtHandler = new JsonWebTokenHandler();
            string jwtToken = jwtHandler.CreateToken(tokenDescriptor);

            return jwtToken;
        }
        public async Task<string> GenerateRefreshTokenAsync(ApplicationUserIdentity user)
        {
            // 1. Generate the Cryptographically Secure Random String
            var randomTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // 2. Create the Entity to save
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = randomTokenString,
                IssuedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtsettings.RefreshTokenExpiryDays),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return randomTokenString;
        }

        public async Task<RefreshToken?> GetStoredRefreshTokenAsync(string refreshTokenString)
        {
            // ONE DB Call. Fetches Token + User details immediately.
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshTokenString &&
                    x.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshTokenString)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshTokenString);

            if (token == null)
            {
                return false;
            }

            _context.RefreshTokens.Remove(token);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
