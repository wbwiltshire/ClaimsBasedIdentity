using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;
using ClaimsBasedIdentity.Data.Repository;
using System.Runtime.Serialization;
using Serilog.Core;


namespace ClaimsBasedIdentity.Web.UI.Identity
{
    #region IsAuthorizedRequirement/IsAuthorizedHandler
    public class IsAuthorizedRequirement : IAuthorizationRequirement
    {
        public IsAuthorizedRequirement()
        {
        }
    }

    public class IsAuthorizedHandler : AuthorizationHandler<IsAuthorizedRequirement>
    {
        private IHttpContextAccessor contextAccessor;                                   // Don't forget to add services.AddHttpContextAccessor(); to Startup
        private readonly ILogger<IsAuthorizedHandler> logger;
        private AppSettingsConfiguration settings = null;
        private DBConnection dbc;
        private ApplicationControllerSecurityRepository controllerSecurityRepo; 
        private ICollection<ApplicationControllerSecurity> roles;                      // Change type to ApplicationControllerSecurity

        public IsAuthorizedHandler(ILogger<IsAuthorizedHandler> l, IOptions<AppSettingsConfiguration> s, IHttpContextAccessor hca, IDBConnection d)
		{
            contextAccessor = hca;
            logger = l;
            settings = null;
            dbc = (DBConnection)d;
		}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAuthorizedRequirement requirement)
        {
            Endpoint endPoint;
            //ControllerActionDescriptor descriptor;
            string controllerName;
            string actionName;


            // Return if it isn't the right type of claim
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Issuer == "Local Authority"))
            {
                return Task.CompletedTask;
            }

			try
			{
                endPoint = contextAccessor.HttpContext.GetEndpoint();
                controllerName = ((ControllerActionDescriptor)endPoint.Metadata.GetMetadata<ControllerActionDescriptor>()).ControllerName;
                actionName = ((ControllerActionDescriptor)endPoint.Metadata.GetMetadata<ControllerActionDescriptor>()).ActionName;

                // 1. Retrieve the roles for this Controller/Action
                // 2. Retrieve the role claims for this user
                // 3. Success if the any user matches any Controller/Action role 

                controllerSecurityRepo = new ApplicationControllerSecurityRepository(settings, logger, dbc);
                roles = (controllerSecurityRepo.FindAll()).Where(cs => cs.ControllerName == controllerName && cs.ActionName == actionName).ToList();

                foreach (Claim c in context.User.Claims.Where(ct => ct.Type == ClaimTypes.Role))
                {
                    if (roles.FirstOrDefault(r => r.RoleName == c.Value) != null)
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }
            }
            catch (Exception ex)
			{
                logger.LogError($"Exception: {ex.Message}");
			}

            return Task.CompletedTask;
        }
    }
    #endregion

    #region IsAdultRequirement/IsAdultHandler
    public class IsAdultRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }
        public IsAdultRequirement(int m)
        {
            MinimumAge = m;
        }
    }

    public class IsAdultHandler : AuthorizationHandler<IsAdultRequirement>
    {
        DateTime dateOfBirth;
        int calculatedAge = 0;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdultRequirement requirement)
        {
            // Return if it isn't the right type of claim
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "Local Authority"))
            {
                return Task.CompletedTask;
            }

            dateOfBirth = Convert.ToDateTime(
                context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "Local Authority").Value
            );

            calculatedAge = DateTime.Today.Year - dateOfBirth.Year;

            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
                calculatedAge--;
            if (calculatedAge >= requirement.MinimumAge)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
	#endregion
}
