using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClaimsBasedIdentity.Web.UI.Identity
{
    public class UserClaim
    {
        public static int GetUserId(ClaimsPrincipal cp)
        {
            Claim claim = null;

                claim = cp.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return (claim != null ? Convert.ToInt32(claim.Value) : -1);
            }
    }
}
