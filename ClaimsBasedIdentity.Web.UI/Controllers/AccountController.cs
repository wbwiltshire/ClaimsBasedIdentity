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
using Microsoft.AspNetCore.Identity;

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
		[Authorize(Policy = "IsAuthorized")]
		[Route("/[controller]/EditUser/{id:int}")]
		public IActionResult EditUser(int id)
		{
			ApplicationUserRepository userRepo;
			ApplicationRoleRepository roleRepo;
			ApplicationUserViewModel view = new ApplicationUserViewModel() { User = null, Roles = null };

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);
				roleRepo = new ApplicationRoleRepository(settings, logger, dbc);

				view.User = userRepo.FindByPKView(new PrimaryKey() { Key = id, IsIdentity = true });
				view.Roles = roleRepo.FindAll();

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
		[Authorize(Policy = "IsAuthorized")]
		[Route("/[controller]/EditUser/{id:int}")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditUser(ApplicationUserViewModel view)
		{
			ApplicationUserRepository userRepo;
			ApplicationRoleRepository roleRepo;
			ApplicationUserClaimRepository userClaimRepo;
			ApplicationAuditLogRepository logRepo;
			ApplicationUserClaim userClaim = null;
			IList<string> currentRoles = null;
			string role = String.Empty;
			int claimId = 0;
			int rows = 0;
			const string issuer = "Local Authority";
			const string claimTypesDepartment = "Department";
			bool isCurrentUser = view.User.UserName.ToUpper() == HttpContext.User.Identity.Name.ToUpper() ? true : false;

			try
			{
				userRepo = new ApplicationUserRepository(settings, logger, dbc);
				userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);
				roleRepo = new ApplicationRoleRepository(settings, logger, dbc);
				logRepo = new ApplicationAuditLogRepository(settings, logger, dbc);

				if (ModelState.IsValid)
				{
					// Update the user in the database
					userRepo.Update(view.User);

					//Add DOB claim to database and to claims list of the current user, if applicable
					userClaim = (userClaimRepo.FindAll()).FirstOrDefault(uc => uc.UserId == view.User.Id && uc.ClaimType == ClaimTypes.DateOfBirth);
					if (userClaim != null) {
						if (userClaim.ClaimValue != view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss")) {
							userClaim.ModifiedDt = DateTime.Now;
							userClaim.ClaimValue = view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss");
							// Update the database
							userClaimRepo.Update(userClaim);
							logger.LogInformation($"Updated claim({ClaimTypes.DateOfBirth}) for user account: {view.User.UserName}");
						}
						else
						{
							// Nothing changed, so no need to update the database
						}

					}
					else
					{
						// Add DOB claim to the database
						userClaim = new ApplicationUserClaim()
						{
							UserId = view.User.Id,
							ClaimType = ClaimTypes.DateOfBirth, ClaimValue = view.User.DOB.ToString("yyyy-MM-dd hh:mm:ss"), ClaimIssuer = issuer,
							Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
						};
						userClaimRepo.Add(userClaim);
						logger.LogInformation($"Added new claim({ClaimTypes.DateOfBirth}) to user account: {view.User.UserName}");
					}

					//TODO: Department still has issues when transitioning from null to not null
					//Add Department claim to database and to claims list of the user
					userClaim = (userClaimRepo.FindAll()).FirstOrDefault(uc => uc.UserId == view.User.Id && uc.ClaimType == claimTypesDepartment);
					if (userClaim != null) {
						if (view.User.Department != null && userClaim.ClaimValue != view.User.Department) {
							userClaim.ModifiedDt = DateTime.Now;
							userClaim.ClaimValue = view.User.Department;
							userClaimRepo.Update(userClaim);
							logger.LogInformation($"Updated claim({claimTypesDepartment}) for user account: {view.User.UserName}");
						}
						else
						{
							// Nothing changed, so no need to update the database
						}
					}
					else {
						if (view.User.Department != null) {
							userClaim = new ApplicationUserClaim()
							{
								UserId = view.User.Id,
								ClaimType = claimTypesDepartment, ClaimValue = view.User.Department, ClaimIssuer = issuer,
								Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
							};
							userClaimRepo.Add(userClaim);
							logger.LogInformation($"Assigned new claim({claimTypesDepartment}) to user account: {view.User.UserName}");
						}
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
										ClaimType = ClaimTypes.Role, ClaimValue = role, ClaimIssuer = issuer,
										Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
									});

									if (claimId > 0)
										logger.LogInformation($"Assigned role({role}) to user account: {view.User.UserName}");
									else
										logger.LogError($"Error assigning role({role}) to user account: {view.User.UserName}");
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
						}
					}

					// If we've updated the claims for the currently signed-in user, 
					// then refresh Cookie by recreating the User Security Principal from the database
					if (isCurrentUser)
					{
						await identityManager.RefreshClaimsAsync(view.User, userClaimRepo.FindAll().Where(uc => uc.UserId == view.User.Id).ToList());
						logRepo.Add(new ApplicationAuditLog() { CategoryId = 1, Description = $"User ({view.User.UserName}) logged into application with refreshed claims." });
						logger.LogInformation($"Refreshed cookie for user: {view.User.UserName}");
					}

					return RedirectToAction("Index", "Account");
				}
				else
				{
					userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);
					userRepo = new ApplicationUserRepository(settings, logger, dbc);

					view.User = userRepo.FindByPKView(new PrimaryKey() { Key = view.User.Id, IsIdentity = true });
					view.Roles = roleRepo.FindAll();

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
		public IActionResult LoginRegister(string action = null, string ReturnUrl = null)
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("/[controller]/Login")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel login, string ReturnUrl = null)
		{
			ApplicationUserRepository userRepo = null;
			ApplicationUserClaimRepository userClaimRepo = null;
			ApplicationAuditLogRepository logRepo = null;
			ApplicationUser user = null;

			try
			{
				if (ModelState.IsValid)
				{
					userRepo = new ApplicationUserRepository(settings, logger, dbc);
					userClaimRepo = new ApplicationUserClaimRepository(settings, logger, dbc);
					logRepo = new ApplicationAuditLogRepository(settings, logger, dbc);

					// Find user in database and validate there password				
					user = (userRepo.FindAll()).FirstOrDefault(u => u.NormalizedUserName == login.UserName.ToUpper());
					if (user != null)
					{
						//valid = PasswordHash.ValidateHashedPassword(user.PasswordHash, login.Password);

						// Add user claims from the database
						user.Claims = new List<ApplicationUserClaim>();
						foreach (ApplicationUserClaim c in userClaimRepo.FindAll().Where(c => c.UserId == user.Id))
							user.Claims.Add(c);

						// Sign in the user, if there password is correct
						if (await identityManager.SignInWithPasswordAsync(user, login.Password))
						{

							logRepo.Add(new ApplicationAuditLog() { CategoryId = 1, Description = $"User ({user.UserName}) logged into application." });
							logger.LogInformation($"Logged in user: {login.UserName}");
							if (ReturnUrl == null) 
								return LocalRedirect("/Home/LoginSuccess");
							else 
								return LocalRedirect($"{ReturnUrl}");
						}
						else
						{
							logger.LogError($"Invalid user name / password combination: {login.UserName}");
							logRepo.Add(new ApplicationAuditLog() { CategoryId = 1, Description = $"Invalid user name / password combination for User ({user.UserName})." });
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
				{
					logger.LogError("Invalid ModelState: AccountController.Login()");
					return View($"LoginRegister?action=login?ReturnUrl={ReturnUrl}");
				}
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
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel register, string ReturnUrl = null)
		{
			ApplicationUserRepository userRepo;
			ApplicationUserClaimRepository userClaimRepo;
			ApplicationUser user;
			//ApplicationUserClaim userClaim;
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

						if (ReturnUrl == null)
							return LocalRedirect("/Home/LoginSuccess");
						else
							return LocalRedirect($"{ReturnUrl}");
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
		public async Task<IActionResult> Logout()
		{
			ApplicationAuditLogRepository logRepo;
			var redirectUrl = new { url = "/Home/Logout" };

			try
			{
				logRepo = new ApplicationAuditLogRepository(settings, logger, dbc);

				await identityManager.SignOutAsync(HttpContext.User.Identity.Name);
				logRepo.Add(new ApplicationAuditLog() { CategoryId = 1, Description = $"User ({HttpContext.User.Identity.Name}) logged out of the application." });

			}
			catch (Exception ex)
			{
				logger.LogError($"Exception logging user({HttpContext.User.Identity.Name}) out of the application: {ex.Message}");
				return RedirectToAction("Error");
			}
			return Json(redirectUrl);
		}

		#region ResetPassword
		[HttpGet]
		[Authorize(Policy = "IsAuthorized")]
		[Route("/[controller]/ResetPassword")]
		public IActionResult ResetPassword()
		{
			ResetPasswordViewModel view = new ResetPasswordViewModel();
			ApplicationUserRepository applicationUserRepository;

			applicationUserRepository = new ApplicationUserRepository(settings, logger, dbc);
			view.Users = applicationUserRepository.FindAll().Where(u => u.NormalizedUserName != "ADMIN" && u.NormalizedUserName != "ADMINISTRATOR").ToList();

			return View("ResetPassword", view);
		}

		[HttpGet]
		[Authorize]
		[Route("/[controller]/PasswordResetSuccess")]
		public IActionResult PasswordResetSuccess()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Policy = "IsAuthorized")]
		[Route("/[controller]/ResetPassword")]
		[ValidateAntiForgeryToken]
		public IActionResult ResetPassword(ResetPasswordViewModel view)
		{
			ApplicationUserRepository userRepo = null;
			ApplicationUser user = null;

			try
			{
				if (ModelState.IsValid)
				{
					if (view.Password == view.ConfirmPassword) {
						if (view.UserName.ToUpper() != "ADMIN" && view.UserName.ToUpper() != "ADMINISTRATOR")
						{
							userRepo = new ApplicationUserRepository(settings, logger, dbc);
							// Find user in database and validate there password				
							user = (userRepo.FindAll()).FirstOrDefault(u => u.NormalizedUserName == view.UserName.ToUpper());
							user.PasswordHash = PasswordHash.HashPassword(view.Password);
							userRepo.Update(user);

							return RedirectToAction("PasswordResetSuccess", "Account");
						}
						else
						{
							// Can't reset Admin password
							ModelState.AddModelError("UserName", "You can't reset the Admin users password!");
							return View("ResetPassword", view);
						}
					}
					else {
						// Passwords don't match
						ModelState.AddModelError("ConfirmPassword", "Confirm Password doesn't match New Password.");
						return View("ResetPassword", view);

					}
				}
				else
				{
					return View("ResetPassword", view);
				}
			}
			catch (Exception ex)
			{
				logger.LogError($"Exception: {ex.Message}");
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

		}
		#endregion

		#region ChangePassword
		[HttpGet]
		[Authorize]
		[Route("/[controller]/ChangePassword")]
		public IActionResult ChangePassword()
		{
			ChangePasswordViewModel view = new ChangePasswordViewModel();

			return View("ChangePassword", view);
		}

		[HttpGet]
		[Authorize]
		[Route("/[controller]/PasswordChangeSuccess")]
		public IActionResult PasswordChangeSuccess()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		[Route("/[controller]/ChangePassword")]
		[ValidateAntiForgeryToken]
		public IActionResult ChangePassword(ChangePasswordViewModel view)
		{
			ApplicationUserRepository userRepo = null;
			ApplicationUser user = null;

			try
			{
				if (ModelState.IsValid)
				{
					if (view.OldPassword != view.NewPassword)
					{
						userRepo = new ApplicationUserRepository(settings, logger, dbc);

						// Find user in database and validate there password				
						user = (userRepo.FindAll()).FirstOrDefault(u => u.NormalizedUserName == view.UserName.ToUpper());
						if (PasswordHash.ValidateHashedPassword(user.PasswordHash, view.OldPassword))
						{
							user.PasswordHash = PasswordHash.HashPassword(view.NewPassword);
							userRepo.Update(user);
							return RedirectToAction("PasswordChangeSuccess", "Account");
						}
						else
						{
							// Old Password wrong
							ModelState.AddModelError("OldPassword", "Password doesn't match our records.");
							return View("ChangePassword", view);
						}
					}
					else
					{
						// Passwords matched
						ModelState.AddModelError("NewPassword", "New Password is the same as the Old Password.");
						return View("ChangePassword", view);
					}
				}
				else
				{
					return View("ChangePassword", view);
				}
			}
			catch (Exception ex)
			{
				logger.LogError($"Exception: {ex.Message}");
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

		}
		#endregion

		[HttpGet]
		[Authorize]
		[Route("/[controller]/Roles")]
		public IActionResult Roles()
		{
			ApplicationRoleRepository roleRepo;
			ICollection<ApplicationRole> roles;

			try
			{
				roleRepo = new ApplicationRoleRepository(settings, logger, dbc);
				roles = roleRepo.FindAll();
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

			return View("Roles", roles);
		}

		[HttpGet]
		[Authorize]
		[Route("/[controller]/AuditLog")]
		public IActionResult AuditLog()
		{
			ApplicationAuditLogRepository logRepo;
			IPager<ApplicationAuditLog> pager;
			
			try
			{
				logRepo = new ApplicationAuditLogRepository(settings, logger, dbc);
				pager = logRepo.FindAll(new Pager<ApplicationAuditLog>() { PageNbr = 0, PageSize = 20 });
			}
			catch (Exception ex)
			{
				throw (Exception)Activator.CreateInstance(ex.GetType(), ex.Message + ex.StackTrace);
			}

			return View("AuditLog", pager);
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
		[Route("/[controller]/AccessDenied")]
		public IActionResult AccessDenied()
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
