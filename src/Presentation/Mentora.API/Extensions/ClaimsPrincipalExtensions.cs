using System.Security.Claims;

namespace Mentora.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(id))
        {
            return Guid.Empty;
        }

        return Guid.TryParse(id, out var parsedId) ? parsedId : Guid.Empty;
    }
}
