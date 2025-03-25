using System.Security.Claims;

namespace Authorization.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user?.FindFirst("id")?.Value;

            if (userIdClaim != null && int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return 0;
        }
    }
}


