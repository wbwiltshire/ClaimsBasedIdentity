using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;
using ClaimsBasedIdentity.Data.Repository;
using System.Security.Cryptography.X509Certificates;

namespace ClaimsBasedIdentity.Web.UI.Identity
{
	public interface IIdentityManager
	{
		Task<bool> RefreshClaimsAsync(ApplicationUser u, ICollection<ApplicationUserClaim> c);
		Task<bool> SignInAsync(ApplicationUser u);
		Task<bool> SignInWithPasswordAsync(ApplicationUser u, string pw);
		Task<bool> SignOutAsync(string n);
	}

	public class IdentityManager : IIdentityManager
	{
		private readonly ILogger<IsAuthorizedHandler> logger;
		private readonly AppSettingsConfiguration settings;
		private readonly IHttpContextAccessor contextAccessor;              // Don't forget to add services.AddHttpContextAccessor(); to Startup

		public IdentityManager(ILogger<IsAuthorizedHandler> l, IOptions<AppSettingsConfiguration> s, IHttpContextAccessor hca)
		{
			logger = l;
			settings = s.Value;
			contextAccessor = hca;
		}

		#region RefreshClaimsAsync(ApplicationUser, ICollection<ApplicationUserClaim>)
		public async Task<bool> RefreshClaimsAsync(ApplicationUser user, ICollection<ApplicationUserClaim> claims)
		{

			ClaimsPrincipal userPrincipal;
			bool result = false;

			try
			{
				// Remove all the old claims
				foreach (Claim c in contextAccessor.HttpContext.User.Claims.ToList())
					((ClaimsIdentity)contextAccessor.HttpContext.User.Identity).RemoveClaim(c);
				// Add all the new claims
				foreach (ApplicationUserClaim uc in claims)
					((ClaimsIdentity)contextAccessor.HttpContext.User.Identity).AddClaim(uc.ToClaim());


				// Build User Security Principal from the Identity Principal
				userPrincipal = new ClaimsPrincipal(contextAccessor.HttpContext.User.Identity);

				// Sign In User
				await contextAccessor.HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					userPrincipal,
					new AuthenticationProperties()
					{
						IsPersistent = false
					});

				logger.LogInformation($"Logged in user: {user.UserName}");
				result = true;
			}
			catch (Exception ex)
			{
				logger.LogError($"Exception: RefreshClaimsAsync - {ex.Message}");
			}

			return result;
		}
		#endregion

		#region SignInAsync(ApplicationUser)
		public async Task<bool> SignInAsync(ApplicationUser user)
		{
			ClaimsIdentity userIdentity;
			ClaimsPrincipal userPrincipal;
			bool result = false;

			try
			{
				// Build User Identity
				userIdentity = new ClaimsIdentity("LocalIdentity");

				// Add claims from the User
				foreach (ApplicationUserClaim c in user.Claims)
					userIdentity.AddClaim(new Claim(c.ClaimType, c.ClaimValue, ClaimValueTypes.String, c.ClaimIssuer));

				// Build User Security Principal from the Identity Principal
				userPrincipal = new ClaimsPrincipal(userIdentity);

				// Sign In User
				await contextAccessor.HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					userPrincipal,
					new AuthenticationProperties()
					{
						IsPersistent = false
					});

				logger.LogInformation($"Logged in user: {user.UserName}");
				result = true;
			}
			catch (Exception ex)
			{
				logger.LogError($"Exception: SignInAsync - {ex.Message}");
			}

			return result;
		}
		#endregion

		#region SignInWithPassword(ApplicationUser, string)
		public async Task<bool> SignInWithPasswordAsync(ApplicationUser user, string loginPassword) {

			if (PasswordHash.ValidateHashedPassword(user.PasswordHash, loginPassword))
				return await SignInAsync(user);
			else
				return await Task.FromResult(false); }

		#endregion

		#region SignOutAsync()
		public async Task<bool> SignOutAsync(string name)
		{
			bool result = false;

			try
			{
				await contextAccessor.HttpContext.SignOutAsync();
				logger.LogInformation($"Logged out user: {name}");
			}
			catch (Exception ex)
			{
				logger.LogError($"Exception: SignOutAsync - {ex.Message}");
			}

			return result;
		}
		#endregion

	}
}
