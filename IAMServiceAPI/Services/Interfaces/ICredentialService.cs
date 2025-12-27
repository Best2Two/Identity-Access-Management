using Microsoft.AspNetCore.Identity;

namespace IAMService.Services.Interfaces
{
    public interface ICredentialService
    {
        public Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
