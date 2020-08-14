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

		#region /Account/Index
		// GET: /Account/
		[HttpGet]
		[Authorize(Policy = "IsAuthorized")]
		[Route("/[controller]/Index")]
		public IActionResult Index()
		{
			ApplicationUserRepository userRepo;
			IPager<ApplicationUser> pager = null;

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);

				pager = userRepo.FindAll(new Pager<ApplicationUser>() { PageNbr = 0, PageSize = 20 });
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}

			return View(pager);
		}
		#endregion

		#region /Account/UserProfile
		//GET: /Account/UserProfile/{id}
		[HttpGet]
		[Authorize]
		[Route("/[controller]/UserProfile/{id:int}")]
		public IActionResult UserProfile(int id)
		{
			ApplicationUserRepository userRepo;
			//ApplicationUserRoleRepository userRoleRepo;
			ApplicationUser user = null;

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);
				//userRoleRepo = new ApplicationUserRoleRepository(settings, logger, dbc);

				user = userRepo.FindByPK(new PrimaryKey() { Key = id, IsIdentity = true });
				//user.Roles = new List<ApplicationRole>();
				//foreach (ApplicationUserRole ur in (await userRoleRepo.FindAllView()).Where(r => r.UserId == user.Id))
				//	user.Roles.Add(ur.ApplicationRole);
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

			return View("UserProfile", user);
		}
		#endregion

		#region /Account/UserDetails
		//GET: /Account/UserDetails/{id}
		[HttpGet]
		[Authorize(Policy = "IsAuthorized")]
		[Route("/[controller]/UserDetails/{id:int}")]
		public IActionResult UserDetails(int id)
		{
			ApplicationUserRepository userRepo;
			//ApplicationUserRoleRepository userRoleRepo;
			ApplicationUser user = null;

			try
			{
					userRepo = new ApplicationUserRepository(settings, logger, dbc);
					//userRoleRepo = new ApplicationUserRoleRepository(settings, logger, dbc);

					user = userRepo.FindByPK(new PrimaryKey() { Key = id, IsIdentity = true });
					//user.Roles = new List<ApplicationRole>();
					//foreach (ApplicationUserRole ur in (await userRoleRepo.FindAllView()).Where(r => r.UserId == user.Id))
					//	user.Roles.Add(ur.ApplicationRole);
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

			return View("UserDetails", user);
		}
		#endregion

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
			ApplicationUserClaimRepository userClaimsRepo = null;
			ApplicationUser user = null;
			ClaimsIdentity userIdentity = null;
			ClaimsPrincipal userPrincipal = null;
			bool valid = false;
			//const string issuer = "Local Authority";

			try
			{
				if (ModelState.IsValid)
				{
					userRepo = new ApplicationUserRepository(settings, logger, dbc);
					userClaimsRepo = new ApplicationUserClaimRepository(settings, logger, dbc);

					// Find user in database and validate there password				
					user = (userRepo.FindAll()).FirstOrDefault(u => u.NormalizedUserName == login.UserName.ToUpper());
					if (user != null)
					{
						valid = PasswordHash.ValidateHashedPassword(user.PasswordHash, login.Password);
						// Build the user Identity context, if they are validated
						if (valid)
						{
							// Build User Identity
							userIdentity = new ClaimsIdentity("LocalIdentity");

							// Add claims from the database
							foreach (ApplicationUserClaim c in userClaimsRepo.FindAll().Where(c => c.UserId == user.Id))
								userIdentity.AddClaim(new Claim(c.ClaimType, c.ClaimValue, ClaimValueTypes.String, c.ClaimIssuer));
							
							// Build User Security Principal from the Identity Principal
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

		[HttpPost]
		[AllowAnonymous]
		[Route("/[controller]/Register")]
		//[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel register)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserClaimRepository userClaimRepo;
			ApplicationUser user;
			AuthenticationProperties props = null;
			ClaimsIdentity userIdentity = null;
			ClaimsPrincipal userPrincipal = null;
			const string issuer = "Local Authority";
			int id = 0;

			if (ModelState.IsValid)
			{
				try
				{
					userRepo = new ApplicationUserRepository(settings, logger, dbc);
					userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);
					user = userRepo.FindAll().FirstOrDefault(u => u.NormalizedUserName == register.UserName.ToUpper());

					if (user == null)
					{
						user = new ApplicationUser()
						{
							UserName = register.UserName,
							NormalizedUserName = register.UserName.ToUpper(),
							Email = register.Email,
							NormalizedEmail = register.Email.ToUpper(),
							EmailConfirmed = true,
							PhoneNumber = String.Empty,
							PhoneNumberConfirmed = false,
							TwoFactorEnabled = false,
							DOB = DateTime.Now,
							Department = String.Empty,
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
						};

						// Add user to the database
						user.PasswordHash = PasswordHash.HashPassword(register.Password);
						id = (int)userRepo.Add(user);
						logger.LogInformation($"Created new user account: {register.UserName}");

						// Build User Identity
						userIdentity = new ClaimsIdentity("LocalIdentity");
						userIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, issuer));
						userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id.ToString(), ClaimValueTypes.Integer32, issuer));
						userIdentity.AddClaim(new Claim(ClaimTypes.Role, "Basic", ClaimValueTypes.String, issuer));
						userIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, user.DOB.ToString("yyyy-MM-dd hh:mm:ss"), ClaimValueTypes.String, issuer));

						// Add User Claims to the database
						foreach (Claim c in userIdentity.Claims)
							userClaimRepo.Add(new ApplicationUserClaim() {
								UserId = id,
								ClaimType = c.Type,
								ClaimValue = c.Value,
								ClaimIssuer = c.Issuer,
								Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
							});

						logger.LogInformation($"Assigned default claims to new user account: {register.UserName}");

						// Build User Security Principal from the Identity Principal
						userPrincipal = new ClaimsPrincipal(userIdentity);

						// Sign In User
						await HttpContext.SignInAsync(
							CookieAuthenticationDefaults.AuthenticationScheme,
							userPrincipal,
							new AuthenticationProperties()
							{
								IsPersistent = false
							});

						return RedirectToAction("LoginSuccess", "Home");
					}
					else
					{
						logger.LogError($"User is already registered: {register.UserName}");
						return LocalRedirect("/Account/UserAlreadyRegistered");
					}

				}
				catch (Exception ex)
				{
					logger.LogError($"Exception registering user({register.UserName}): {ex.Message}");
					return RedirectToAction("Error");
				}
			}

			return RedirectToAction("Error");
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("/[controller]/UserAlreadyRegistered")]
		public IActionResult UserAlreadyRegistered()
		{
			return View();
		}
		#endregion

		[HttpPost]
		[Authorize]
		[Route("/[controller]/Logout")]
		public IActionResult Logout()
		{
			var redirectUrl = new { url = "/Home/Logout" };

			HttpContext.SignOutAsync();
			return Json(redirectUrl);
		}

		[HttpGet]
		[Authorize]
		[Route("/[controller]/Claims")]
		public IActionResult Claims()
		{
			return View();
		}

		[HttpGet]
		[Authorize]
		[Route("/[controller]/Forbidden")]
		public IActionResult Forbidden()
		{
			return View();
		}

		[AllowAnonymous]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
