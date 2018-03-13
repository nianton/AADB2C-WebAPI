using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace WebApi
{
    public static class UserExtensions
    {
        // OWIN auth middleware constants
        private const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        private const string ObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public static bool HasPermission(this IPrincipal principal, string permission)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;

            if (claimsPrincipal == null)
                return false;

            var scopeClaim = claimsPrincipal.FindFirst(ScopeClaimType);
            if (scopeClaim == null)
                return false;

            return scopeClaim.Value.Split(' ').Contains(permission);
        }

        public static string GetId(this IPrincipal principal)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var idClaim = claimsPrincipal?.FindFirst(ObjectIdClaimType)?.Value;
            return idClaim;
        }
    }
}