using System.Security.Claims;

namespace backend.Security;

public static class ClaimsPrincipalExtensions
{
    public static long? GetUserId(this ClaimsPrincipal principal)
    {
        // JwtRegisteredClaimNames.Sub is mapped to ClaimTypes.NameIdentifier by default
        var raw = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? principal.FindFirstValue("sub");

        if (long.TryParse(raw, out var id))
        {
            return id;
        }

        return null;
    }
}
