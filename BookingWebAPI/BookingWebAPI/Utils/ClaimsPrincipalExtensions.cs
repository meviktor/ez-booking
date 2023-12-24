using System.Security.Claims;

namespace BookingWebAPI.Utils
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasClaims(this ClaimsPrincipal user, params string[] claims) =>
            claims.All(claimToFind => user.Claims.Any(userClaim => userClaim.Type.Equals(claimToFind)));
    }
}
