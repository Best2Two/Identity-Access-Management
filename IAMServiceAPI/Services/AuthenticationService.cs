using IAMService.Data.DTOs;
using IAMService.Data.Entities;
using IAMService.Data.Identities;
using IAMService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IAMService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager <ApplicationUserIdentity> _userManager;
        private readonly ITokenService _tokenService;
        public AuthenticationService(
        UserManager<ApplicationUserIdentity> userManager,
        ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<AuthenticationResult> RegisterUserAsync(ApplicationUserDto user)
        {

            //Check if the user exists in the DB by them credential
            var existingUserByEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingUserByEmail != null)
            {
                    //fix here error string
                    return AuthenticationResult.Failed("USER_EMAIL_REGISTERED_ALREADY");
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(user.Username);
            if (existingUserByUsername != null)
            {
                //fix here error string
                return AuthenticationResult.Failed("USER_USERNAME_REGISTERED_ALREADY");
            }

            //Map current DTO with the applicationUserIdentity
            var applicationUserIdentity = new ApplicationUserIdentity
            {
                Email = user.Email,
                UserName = user.Username,
                PhoneNumber = user.PhoneNumber,                
                NormalizedUserName = _userManager.KeyNormalizer.NormalizeName(user.Username),
                NormalizedEmail = _userManager.KeyNormalizer.NormalizeEmail(user.Email)
            };

            //CreateAsync salt and hash the password!
            var result = await _userManager.CreateAsync(applicationUserIdentity, user.Password);

            if (!result.Succeeded)
            {
                return AuthenticationResult.Failed("User can't be created, please try again");
            }


            //Create jwt access token
            string accessToken = _tokenService.GenerateAccessToken(applicationUserIdentity);
            string refreshToken = await _tokenService.GenerateRefreshTokenAsync(applicationUserIdentity);

            return AuthenticationResult.Succeeded("AUTHENTICATED_SUCCESS", accessToken, refreshToken);


        }
        public async Task<AuthenticationResult> LoginUserAsync(string loginIdentifier, string password)
        {
            var normalizedInput = loginIdentifier.ToUpperInvariant();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedInput ||
                                          u.NormalizedUserName == normalizedInput);

            if (user == null)
            {
                return AuthenticationResult.Failed("Invalid Credentials");
            }

            var isPasswordMatch = await _userManager.CheckPasswordAsync(user, password);

            if (!isPasswordMatch)
            {
                return AuthenticationResult.Failed("Invalid Credentials");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            return AuthenticationResult.Succeeded("AUTHENTICATED_SUCCESS", accessToken, refreshToken);

        }
        public async Task<AuthenticationResult> RefreshUserAsync(string refreshTokenString)
        {
            RefreshToken refreshToken = await _tokenService.GetStoredRefreshTokenAsync(refreshTokenString);

            if (refreshToken == null || refreshToken.IsRevoked==true)
            {
                return AuthenticationResult.Failed("TOKEN_IS_NOT_VALID");
            }
            //Maybe considering a helper function heres
            if(refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return AuthenticationResult.Failed("TOKEN_IS_EXPIRED");
            }
            var accessToken = _tokenService.GenerateAccessToken(refreshToken.User);
            
            //Returning the refresh token here again maybe indicates a logical error
            return AuthenticationResult.Succeeded("AUTHENTICATED_SUCCESS", accessToken, refreshToken.Token);
        }
        public async Task<AuthenticationResult> LogoutUserAsync(string refreshTokenString)
        {
            bool revoked  = await _tokenService.RevokeRefreshTokenAsync(refreshTokenString);
            if (revoked)
            {
                return AuthenticationResult.Succeeded("TOKEN_REVOKED_SUCCESSFULLY");
            }

            //Rest of reclaiming revoking logic here
            return AuthenticationResult.Failed("TOKEN_CANNOT_BE_REVOKED");
        }




    }
}
