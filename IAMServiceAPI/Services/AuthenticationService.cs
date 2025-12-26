using IAMService.Data.DTOs;
using IAMService.Data.Identities;
using IAMService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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

        public async Task<AuthenticationResult> RegisterUserAsync(ApplicationUser user)
        {

            //Check if the user exists in the DB by them credential
            var existingUserByEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingUserByEmail != null)
            {
                    //fix here error string
                    return AuthenticationResult.Failed("USER_EMAIL_REGISTERED_ALREADY");
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(user.Email);
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
                return AuthenticationResult.Failed("User can't be created into the DB");
            }


            //Create jwt access token
            string accessToken = _tokenService.GenerateAccessToken(applicationUserIdentity);



            return AuthenticationResult.Succeeded(accessToken, accessToken);


        }

        public async Task<AuthenticationResult> LoginUserAsync(string loginIdentifier, string password)
        {
            
            //Returning invalid exact credential here is totally acceptable as they are public ones
            var user = await _userManager.FindByEmailAsync(loginIdentifier);
            
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(loginIdentifier);
            }

            if (user == null) return AuthenticationResult.Failed("INVALID_Username");

            bool isPasswordMatch = await _userManager.CheckPasswordAsync(user, password);

            if (!isPasswordMatch)
            {
                return AuthenticationResult.Failed("INVALID_Email");
            }
            
            //JWT Logic here

        }

        public Task<AuthenticationResult> AuthenticateUserRequest(string securityToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> RefreshUserAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> LogoutUserAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
