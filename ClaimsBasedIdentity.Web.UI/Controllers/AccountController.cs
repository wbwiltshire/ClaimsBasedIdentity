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
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;

namespace ClaimsBasedIdentity.Web.UI.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> logger;
		private readonly AppSettingsConfiguration settings;
		private readonly IDBConnection dbc;
		private readonly IIdentityManager identityManager;

		public AccountController(ILogger<AccountController> l, IOptions<AppSettingsConfiguration> s, IIdentityManager m, IDBConnection d)
		{
			logger = l;
			settings = s.Value;
			identityManager = m;
			dbc = d;
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

				user = userRepo.FindByPKView(new PrimaryKey() { Key = id, IsIdentity = true });
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

					user = userRepo.FindByPKView(new PrimaryKey() { Key = id, IsIdentity = true });
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

		#region /Account/EditProfile
		//GET: /Account/EditProfile/{id}
		[HttpGet]
		[Authorize]
		[Route("/[controller]/EditProfile/{id:int}")]
		public IActionResult EditProfile(int id)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserViewModel view = new ApplicationUserViewModel() { User = null, Roles = null };

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);

				view.User = userRepo.FindByPKView(new PrimaryKey() { Key = id, IsIdentity = true });
				//view.Roles = ApplicationRole.Roles;
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

			return View("EditProfile", view);
		}

		// POST: /Account/EditProfile
		[HttpPost]
		[Authorize]
		[Route("/[controller]/EditProfile/{id:int}")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditProfile(ApplicationUserViewModel view)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserClaimRepository userClaimRepo;
			ApplicationUserClaim userClaim = null;
			ClaimsPrincipal userPrincipal = null;
			Claim dobClaim = null; Claim deptClaim = null;
			const string issuer = "Local Authority";
			const string claimTypesDepartment = "Department";

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);
				userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);

				if (ModelState.IsValid)
				{
					userRepo.Update(view.User);

					//Add DOB claim to database and to claims list of the user
					dobClaim = HttpContext.User.FindFirst(ClaimTypes.DateOfBirth);
					if (dobClaim != null)
					{
						((ClaimsIdentity)HttpContext.User.Identity).RemoveClaim(dobClaim);
						userClaim = (userClaimRepo.FindAll()).FirstOrDefault(uc => uc.UserId == view.User.Id && uc.ClaimType == ClaimTypes.DateOfBirth);
						userClaim.ModifiedDt = DateTime.Now;
						userClaim.ClaimValue = view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss");
						userClaimRepo.Update(userClaim);
					}
					else
						userClaimRepo.Add(new ApplicationUserClaim()
						{
							UserId = view.User.Id,
							ClaimType = ClaimTypes.DateOfBirth,
							ClaimValue = view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss"),
							ClaimIssuer = issuer,
							Active = true,
							ModifiedDt = DateTime.Now,
							CreateDt = DateTime.Now
						});

					((ClaimsIdentity)HttpContext.User.Identity)
						.AddClaim(new Claim(ClaimTypes.DateOfBirth, view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss"), ClaimValueTypes.String, issuer));

					//Add Department claim to database and to claims list of the user
					if (view.User.Department != null)
					{
						deptClaim = HttpContext.User.FindFirst(claimTypesDepartment);
						if (deptClaim != null)
						{
							((ClaimsIdentity)HttpContext.User.Identity).RemoveClaim(deptClaim);
							userClaim = (userClaimRepo.FindAll()).FirstOrDefault(uc => uc.UserId == view.User.Id && uc.ClaimType == claimTypesDepartment);
							userClaim.ModifiedDt = DateTime.Now;
							userClaim.ClaimValue = view.User.Department;
							userClaimRepo.Update(userClaim);
						}
						else
							userClaimRepo.Add(new ApplicationUserClaim()
							{
								UserId = view.User.Id,
								ClaimType = claimTypesDepartment,
								ClaimValue = view.User.Department,
								ClaimIssuer = issuer,
								Active = true,
								ModifiedDt = DateTime.Now,
								CreateDt = DateTime.Now
							});
						((ClaimsIdentity)HttpContext.User.Identity)
							.AddClaim(new Claim(claimTypesDepartment, view.User.Department, ClaimValueTypes.String, issuer));
					}

					// Refresh Cookie by recreating the User Security Principal from the current Identity Principal
					userPrincipal = new ClaimsPrincipal(HttpContext.User.Identity);

					// Sign In User
					await HttpContext.SignInAsync(
						CookieAuthenticationDefaults.AuthenticationScheme,
						userPrincipal,
						new AuthenticationProperties()
						{
							IsPersistent = false
						});

					logger.LogInformation($"Refreshed cookie for user: {view.User.UserName}");

					return RedirectToAction("LoginSuccess", "Home");
				}
				else
				{
					userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);
					userRepo = new ApplicationUserRepository(settings, logger, dbc);

					view.User = userRepo.FindByPKView(new PrimaryKey() { Key = view.User.Id, IsIdentity = true });
					//view.Roles = ApplicationRole.Roles;

					return View("EditProfile", view);
				}
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

		}
		#endregion

		#region /Account/EditUser
		//GET: /Account/EditUser/{id}
		[HttpGet]
		[Authorize(Policy="IsAuthorized")]
		[Route("/[controller]/EditUser/{id:int}")]
		public IActionResult EditUser(int id)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserViewModel view = new ApplicationUserViewModel() { User = null, Roles = null };

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);

				view.User = userRepo.FindByPKView(new PrimaryKey() { Key = id, IsIdentity = true });
				view.Roles = ApplicationRole.Roles;

				// Update the RoleBadges
				foreach (ApplicationUserClaim uc in view.User.Claims.Where(uc => uc.UserId == view.User.Id && uc.ClaimType == ClaimTypes.Role))
					view.User.RoleBadges += String.Format("{0}-{1}|", uc.Id, uc.ClaimValue);
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

			return View("EditUser", view);
		}

		// POST: /Account/EditUser
		[HttpPost]
		[Authorize(Policy="IsAuthorized")]
		[Route("/[controller]/EditUser/{id:int}")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditUser(ApplicationUserViewModel view)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserClaimRepository userClaimRepo;
			ApplicationUserClaim userClaim = null;
			ClaimsPrincipal userPrincipal = null;
			Claim dobClaim = null; Claim deptClaim = null; Claim roleClaim = null;
			IList<string> currentRoles = null;
			string role = String.Empty; 
			int claimId = 0;
			int rows = 0;
			const string issuer = "Local Authority";
			const string claimTypesDepartment = "Department";
			bool isCurrentUser = view.User.UserName.ToUpper() == HttpContext.User.Identity.Name.ToUpper() ? true : false ;

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);
				userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);

				if (ModelState.IsValid)
				{
					userRepo.Update(view.User);

					//Add DOB claim to database and to claims list of the current user, if applicable
					userClaim = (userClaimRepo.FindAll()).FirstOrDefault(uc => uc.UserId == view.User.Id && uc.ClaimType == ClaimTypes.DateOfBirth);
					if (userClaim != null) {
						if (userClaim.ClaimValue != view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss")) {
							userClaim.ModifiedDt = DateTime.Now;
							userClaim.ClaimValue = view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss");
							userClaimRepo.Update(userClaim);
							logger.LogInformation($"Updated role({ClaimTypes.DateOfBirth}) for user account: {view.User.UserName}");
						}
						else
						{
							// Nothing changed, so no need to update the database
						}
					}
					else
					{
						userClaimRepo.Add(new ApplicationUserClaim()
						{
							UserId = view.User.Id,
							ClaimType = ClaimTypes.DateOfBirth,
							ClaimValue = view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss"),
							ClaimIssuer = issuer,
							Active = true,
							ModifiedDt = DateTime.Now,
							CreateDt = DateTime.Now
						});
						logger.LogInformation($"Added new role({ClaimTypes.DateOfBirth}) to user account: {view.User.UserName}");
					}
					if (isCurrentUser) {
						dobClaim = HttpContext.User.FindFirst(ClaimTypes.DateOfBirth);
						if (dobClaim != null && dobClaim.Value != view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss")) {
							((ClaimsIdentity)HttpContext.User.Identity).RemoveClaim(dobClaim);
						}
						((ClaimsIdentity)HttpContext.User.Identity)
							.AddClaim(new Claim(ClaimTypes.DateOfBirth, view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss"), ClaimValueTypes.String, issuer));
					}

					//TODO: Department still has issues when transitioning from null to not null
					//Add Department claim to database and to claims list of the user
					userClaim = (userClaimRepo.FindAll()).FirstOrDefault(uc => uc.UserId == view.User.Id && uc.ClaimType == claimTypesDepartment);
					if (userClaim != null) {
						if (view.User.Department != null && userClaim.ClaimValue != view.User.Department) {
							userClaim.ModifiedDt = DateTime.Now;
							userClaim.ClaimValue = view.User.Department;
							userClaimRepo.Update(userClaim);
							logger.LogInformation($"Updated role({claimTypesDepartment}) for user account: {view.User.UserName}");
						}
						else
						{
							// Nothing changed, so no need to update the database
						}
					}
					else {
						if (view.User.Department != null) {
							userClaimRepo.Add(new ApplicationUserClaim()
							{
								UserId = view.User.Id,
								ClaimType = claimTypesDepartment,
								ClaimValue = view.User.Department,
								ClaimIssuer = issuer,
								Active = true,
								ModifiedDt = DateTime.Now,
								CreateDt = DateTime.Now
							});
							logger.LogInformation($"Assigned new role({claimTypesDepartment}) to user account: {view.User.UserName}");
						}
					}
					if (isCurrentUser) {
						deptClaim = HttpContext.User.FindFirst(claimTypesDepartment);
						if (deptClaim != null && view.User.Department != null && deptClaim.Value != view.User.Department)
						{
							((ClaimsIdentity)HttpContext.User.Identity).RemoveClaim(deptClaim);
						}
						if (view.User.Department != null)
							((ClaimsIdentity)HttpContext.User.Identity)
								.AddClaim(new Claim(claimTypesDepartment, view.User.Department, ClaimValueTypes.String, issuer));
					}

					//Add Role claim to database and to claims list of the user
					// Process the roles and update the role store
					currentRoles = (userClaimRepo.FindAll()).Where(uc => uc.UserId == view.User.Id && uc.ClaimType == ClaimTypes.Role).Select(r => r.ClaimValue).ToList(); ;
					if (view.User.RoleBadges != null) {
						foreach (string r in view.User.RoleBadges.Split("|"))
						{
							if (r != String.Empty)
							{
								role = r.Substring(r.IndexOf('-') + 1, r.Length - r.IndexOf('-') - 1);
								// Add, if it's a new role
								if (!currentRoles.Contains(role))
								{
									claimId = (int)userClaimRepo.Add(new ApplicationUserClaim()
									{
										UserId = view.User.Id,
										ClaimType = ClaimTypes.Role,
										ClaimValue = role,
										ClaimIssuer = issuer,
										Active = true,
										ModifiedDt = DateTime.Now,
										CreateDt = DateTime.Now
									});
									if (claimId > 0)
										logger.LogInformation($"Assigned role({role}) to user account: {view.User.UserName}");
									else
										logger.LogError($"Error assigning role({role}) to user account: {view.User.UserName}");
									if (isCurrentUser)
										((ClaimsIdentity)HttpContext.User.Identity)
											.AddClaim(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, issuer));
								}
							}
						}
					}
					// Remove any roles of which the user is no longer a member
					foreach (string r in currentRoles)
					{
						if (!view.User.RoleBadges.Contains(r)) {
							claimId = (userClaimRepo.FindAll()).FirstOrDefault(c => c.ClaimType == ClaimTypes.Role && c.UserId == view.User.Id).Id;
							rows = userClaimRepo.Delete(new PrimaryKey() { Key = (int)claimId, IsIdentity = true });
							if (rows > 0)
								logger.LogInformation($"Removed role({r}) from user account: {view.User.UserName}");
							else
								logger.LogError($"Error removing role({r}) from account: {view.User.UserName}");
							if (isCurrentUser) {
								roleClaim = HttpContext.User.FindFirst(r);
								((ClaimsIdentity)HttpContext.User.Identity).RemoveClaim(roleClaim);
							}
						}
					}

					// If we've updated the claims for the currently signed-in user, 
					// then refresh Cookie by recreating the User Security Principal from the current Identity Principal
					if (isCurrentUser)
					{
						userPrincipal = new ClaimsPrincipal(HttpContext.User.Identity);

						// Sign In User and Refresh Cookie
						await HttpContext.SignInAsync(
							CookieAuthenticationDefaults.AuthenticationScheme,
							userPrincipal,
							new AuthenticationProperties()
							{
								IsPersistent = false
							});
						logger.LogInformation($"Refreshed cookie for user: {view.User.UserName}");
					}

					return RedirectToAction("Index", "Account");
				}
				else
				{
					userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);
					userRepo = new ApplicationUserRepository(settings, logger, dbc);

					view.User = userRepo.FindByPKView(new PrimaryKey() { Key = view.User.Id, IsIdentity = true });
					view.Roles = ApplicationRole.Roles;

					// Update the RoleBadges
					foreach (ApplicationUserClaim uc in view.User.Claims.Where(uc => uc.UserId == view.User.Id))
						view.User.RoleBadges += String.Format("{0}-{1}|", uc.Id, uc.ClaimType);

					return View("EditUser", view);
				}
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel login)
		{
			ApplicationUserRepository userRepo = null;
			ApplicationUserClaimRepository userClaimRepo = null;
			ApplicationUser user = null;
			bool valid = false;

			try
			{
				if (ModelState.IsValid)
				{
					userRepo = new ApplicationUserRepository(settings, logger, dbc);
					userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);

					// Find user in database and validate there password				
					user = (userRepo.FindAll()).FirstOrDefault(u => u.NormalizedUserName == login.UserName.ToUpper());
					if (user != null)
					{
						valid = PasswordHash.ValidateHashedPassword(user.PasswordHash, login.Password);

						// Build the user Identity context, if they are validated
						if (valid)
						{
							// Add user claims from the database
							user.Claims = new List<ApplicationUserClaim>();
							foreach (ApplicationUserClaim c in userClaimRepo.FindAll().Where(c => c.UserId == user.Id))
								user.Claims.Add(c);

							// Sign in the user
							await identityManager.SignInAsync(user);

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
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel register)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserClaimRepository userClaimRepo;
			ApplicationUser user;
			ApplicationUserClaim userClaim;
			//AuthenticationProperties props = null;
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
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now,
							Claims = new List<ApplicationUserClaim>()
						};

						// NOTE: This shoudl be wrapped in a Unit of Work
						// Add User to the database
						user.PasswordHash = PasswordHash.HashPassword(register.Password);
						id = (int)userRepo.Add(user);
						logger.LogInformation($"Created new user account: {register.UserName}");

						// Add default User Claims
						user.Claims.Add(new ApplicationUserClaim()
						{
							UserId = id,
							ClaimType = ClaimTypes.Name, ClaimValue = user.UserName, ClaimIssuer = issuer,
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
						});
						user.Claims.Add(new ApplicationUserClaim()
						{
							UserId = id, 
							ClaimType = ClaimTypes.NameIdentifier, ClaimValue = id.ToString(), ClaimIssuer = issuer,
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
						});
						user.Claims.Add(new ApplicationUserClaim()
						{
							UserId = id,
							ClaimType = ClaimTypes.Role, ClaimValue = "Basic", ClaimIssuer = issuer,
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
						});
						user.Claims.Add(new ApplicationUserClaim()
						{
							UserId = id,
							ClaimType = ClaimTypes.DateOfBirth, ClaimValue = user.DOB.ToString("yyyy-MM-dd hh:mm:ss"), ClaimIssuer = issuer,
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
						});

						// Add User Claims to the database
						foreach (ApplicationUserClaim c in user.Claims)
							userClaimRepo.Add(c);

						logger.LogInformation($"Assigned default claims to new user account: {register.UserName}");

						// Sign in the user
						await identityManager.SignInAsync(user);

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
