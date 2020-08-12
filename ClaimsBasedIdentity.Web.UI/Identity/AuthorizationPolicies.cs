using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace ClaimsBasedIdentity.Web.UI.Identity
{
    public class IsAuthorizedRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }
        public IsAuthorizedRequirement(int m)
        {
            MinimumAge = m;
        }
    }

    public class IsAdultHandler : AuthorizationHandler<IsAuthorizedRequirement>
    {
        DateTime dateOfBirth;
        int calculatedAge = 0;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAuthorizedRequirement requirement)
        {
            // Return if it isn't the right type of claim
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "LOCAL AUTHORITY"))
            {
                return Task.CompletedTask;
            }

            dateOfBirth = Convert.ToDateTime(
                context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "LOCAL AUTHORITY").Value
            );

            calculatedAge = DateTime.Today.Year - dateOfBirth.Year;

            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
                calculatedAge--;
            if (calculatedAge >= requirement.MinimumAge)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
