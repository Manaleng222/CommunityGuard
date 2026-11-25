using Microsoft.AspNetCore.Identity;

namespace CommunityGuard.Api.Models;

// Inherits from IdentityUser so ASP.NET Core handles password hashing.
public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
}