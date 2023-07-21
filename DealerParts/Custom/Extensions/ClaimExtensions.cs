namespace DealerParts.Custom.Extensions
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using System.Security.Claims;

    public static class ClaimExtensions
    {
        public static void AddUpdateClaim(this ClaimsPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // Check for an existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // Add the new claim
            identity.AddClaim(new Claim(key, value));
        }

        public static string GetClaimValue(this ClaimsPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);

            // ?. prevents an exception if the claim is null.
            return claim?.Value;
        }
    }

}
