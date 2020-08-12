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
        public static int UserId(this IIdentity identity)
        {
            if (identity == null)
                return -1;

            int id = (identity as ClaimsIdentity).GetUserId(ClaimTypes.NameIdentifier);

            return id;
        }

        internal static int GetUserId(this ClaimsIdentity identity, string claimType)
        {
            var val = identity.FindFirst(claimType);

            return val == null ? -1 : Convert.ToInt32(val.Value);
        }
    }
}
