using System.Security.Claims;

namespace Backend.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (claim == null)
        {
            throw new UnauthorizedAccessException("User ID claim missing");
        }

        return int.Parse(claim);
    }
}