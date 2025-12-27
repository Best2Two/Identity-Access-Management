using IAMService.Data.Identities;
using Microsoft.AspNetCore.Identity;

namespace IAMService.Services
{
    public class CredentialService
    {
        private readonly UserManager<ApplicationUserIdentity> _userManager;

        public CredentialService(UserManager<ApplicationUserIdentity> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "USER_NOT_FOUND" });
            }

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

    }
}