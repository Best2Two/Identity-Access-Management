using Microsoft.AspNetCore.Identity;

namespace IAMService.Data.Identities
{
    // Using custom UserIdentity to avoid tight coupling with Microsoft.AspNetCore.Identity.
    // This allows flexibility to change identity providers or migrate away from Identity without refactoring the entire codebase.
    public class ApplicationUserIdentity:IdentityUser
    {

    }
}
