using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Web.UI.Models;
using ClaimsBasedIdentity.Data;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;
using ClaimsBasedIdentity.Data.Repository;
using ClaimsBasedIdentity.Web.UI.Identity;

namespace ClaimsBasedIdentity.Web.UI.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> logger;
		private readonly AppSettingsConfiguration settings;
		private readonly DBConnection dbc;

		public AccountController(ILogger<AccountController> l, IOptions<AppSettingsConfiguration> s, IDBConnection d)
		{
			logger = l;
			settings = s.Value;
			dbc = (DBConnection)d;
		}

		#region Login and Register
		[HttpGet]
		[AllowAnonymous]
		[Route("/[controller]/LoginRegister")]
		public IActionResult LoginRegister()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("/[controller]/Login")]
		public async Task<IActionResult> Login(LoginViewModel login)
		{
			//ApplicationUserClaimRepository userClaimRepo;
			ApplicationUserRepository userRepo = null;
			ApplicationUser user = null;
			IList<Claim> claims = new List<Claim>();
			ClaimsIdentity userIdentity = null;
			ClaimsPrincipal userPrincipal = null;
			bool valid = false;
			const string issuer = "Local Authority";

			try
			{
				if (ModelState.IsValid)
				{

					// Find user in database and validate there password				
					userRepo = new ApplicationUserRepository(settings, logger, dbc);
					user = (userRepo.FindAll()).FirstOrDefault(u => u.NormalizedUserName == login.UserName.ToUpper());
					if (user != null)
					{
						valid = PasswordHash.ValidateHashedPassword(user.PasswordHash, login.Password);
						// Build the user Identity context, if they are validated
						if (valid)
						{

							// Build User Identity from claims in the database
							userIdentity = new ClaimsIdentity("LocalIdentity");
							claims.Add(new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, issuer));
							claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer32, issuer));

							// Build User Security Principal
							userIdentity.AddClaims(claims);
							userPrincipal = new ClaimsPrincipal(userIdentity);

							// Sign In User
							await HttpContext.SignInAsync(
								CookieAuthenticationDefaults.AuthenticationScheme,
								userPrincipal,
								new AuthenticationProperties()
								{
									IsPersistent = false
								});

							logger.LogInformation($"Logged in user: {login.UserName}");
							return LocalRedirect("/Home/LoginSuccess");
						}
						else
						{
							logger.LogError($"Invalid user name / password combination: {login.UserName}");
							return LocalRedirect("/Home/InvalidCredentials");
						}
					}
					else
					{
						logger.LogError($"Invalid user name / password combination: {login.UserName}");
						return LocalRedirect("/Home/InvalidCredentials");
					}
				}
				else
					logger.LogError("Invalid ModelState: AccountController.Login()");
			}
			catch (Exception ex)
			{
				logger.LogError($"Exception: {ex.Message}");
			}

			return RedirectToAction("Error");
		}
		#endregion

		[HttpPost]
		[Route("/[controller]/Logout")]
		public IActionResult Logout()
		{
			var redirectUrl = new { url = "/Home/Logout" };

			HttpContext.SignOutAsync();
			return Json(redirectUrl);
		}

		[AllowAnonymous]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
