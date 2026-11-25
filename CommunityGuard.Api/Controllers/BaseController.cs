using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityGuard.Api.Controllers;

// Inherit from this instead of ControllerBase for secure endpoints.
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    // Extracts the User ID from the JWT token claims.
    protected Guid CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
        }
    }
}