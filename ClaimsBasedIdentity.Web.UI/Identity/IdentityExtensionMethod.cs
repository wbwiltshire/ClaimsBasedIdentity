using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ClaimsBasedIdentity.Web.UI.Identity
{
    public static class IdentityExtensionMethod
    {
        // Deprecated: Don't expose the Id of the ApplicationUser, because it makes them easy to guess someone elses
        //public static int UserId(this IIdentity identity)
        //{
        //    if (identity == null)
        //        return -1;

        //    int id = (identity as ClaimsIdentity).GetUserId(ClaimTypes.NameIdentifier);

        //    return id;
        //}

        //internal static int GetUserId(this ClaimsIdentity identity, string claimType)
        //{
        //    var val = identity.FindFirst(claimType);

        //    return val == null ? -1 : Convert.ToInt32(val.Value);
        //}

        public static string HashId(this IIdentity identity)
        {
            if (identity == null)
                return null;

            string id = (identity as ClaimsIdentity).GetHashId(ClaimTypes.NameIdentifier);

            return id;
        }

        internal static string GetHashId(this ClaimsIdentity identity, string claimType)
        {
            Claim val = identity.FindFirst(claimType);

            return val is null ? null : val.Value;
        }
    }
}
