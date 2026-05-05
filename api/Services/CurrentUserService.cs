using System.Security.Claims;

namespace OpenIga.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claimValue = user?.FindFirstValue("oid")
                ?? user?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
                ?? user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("sub");

            return Guid.TryParse(claimValue, out var userId) ? userId : null;
        }
    }

    public string? Email
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.Email)
                ?? user?.FindFirstValue("preferred_username")
                ?? user?.FindFirstValue("email")
                ?? user?.FindFirstValue("upn");
        }
    }
}
