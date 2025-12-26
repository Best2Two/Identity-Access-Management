using dotenv.net;
using IAMService.Data.DTOs;
using IAMService.Data.Identities;
using IAMService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        public string GenerateAccessToken(ApplicationUserIdentity user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())

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

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            // 1. Generate the Cryptographically Secure Random String
            var randomTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // 2. Create the Entity to save
            var refreshTokenEntity = new RefreshToken
            {
                UserId = userId,
                Token = randomTokenString,
                IssuedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtsettings.RefreshTokenExpiryDays),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return randomTokenString;
        }

    }
}
