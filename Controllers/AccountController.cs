using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<AccountController> logger;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
			this.logger = logger;
		}
        [HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                };
                
               
                var result = await userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
					var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmationEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Error, confirmationLink);

					if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUser", "Administration");
                    }
                    ViewBag.ErrorTitle = "Registration Successfull";
                    ViewBag.ErrorMessage = "Please confirm your registration with email";
                    return View("Error");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View();
        }
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmationEmail(string userId, string token)
		{
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Company");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user id {userId} is invalid";
                return View("NotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
			ViewBag.ErrorMessage = $"Confirmation process cannot be done";
			return View("Error");
		}

		[HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Company");
        }


		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string ReturnUrl = null)
		{
            LoginViewModel model = new LoginViewModel();
            model.ReturnURL = ReturnUrl;
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			return View(model);
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				string returnUrl = model.ReturnURL;
				model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
				var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user,model.Password)))
                {
                    ModelState.AddModelError("", "User Email Not Confirmed");
                    return View(model);
                }


				var result = await signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,true);
				if (result.Succeeded)
				{					
                    //if (!string.IsNullOrEmpty(ReturnUrl))
                    //{
                    //    return LocalRedirect(ReturnUrl);
                    //}
					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
                    {
						return RedirectToAction("Index", "Company");
					}
			
				}		
                if (result.IsLockedOut)
                {
                    return View("LockedOut");
                }
					ModelState.AddModelError("","Invalid Login Attempt");
			}
			
			return View(model);
		}


		[HttpPost]
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> IsEmailInUse(string email)
		{
			ApplicationUser user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
                return Json(true);
			}
            else
            {
                return Json($"Email {email} is already taken");
            }
			
		}
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string ReturnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback","Account", new { ReturnUrl = ReturnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string ReturnUrl = null,string remoteError = null)
		{
            var returnUrl = ReturnUrl ?? Url.Content("~/");

			if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            }
            LoginViewModel model = new LoginViewModel()
            {
                ReturnURL = returnUrl,
			ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
			};
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
				ModelState.AddModelError(string.Empty, $"Error loading external information");
                return RedirectToAction("Login",model);
			}
			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			
			var signinResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey,isPersistent: false,bypassTwoFactor:false);
            if (signinResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
               

                if (email != null)
                {

					var user = await userManager.FindByEmailAsync(email);

					if (user == null)
                    {
						user = new ApplicationUser()
						{
							UserName = email,
							Email = email,
						};

                        await userManager.CreateAsync(user);
						var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
						var confirmationLink = Url.Action("ConfirmationEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
						logger.Log(LogLevel.Error, confirmationLink);
						ViewBag.ErrorTitle = "Registration Successfull";
						ViewBag.ErrorMessage = "Please confirm your registration with email";
						return View("Error");
					}

					if (user != null && !user.EmailConfirmed)
					{
						ModelState.AddModelError("", "User Email Not Confirmed");
						return View("Login", model);
					}

					await userManager.AddLoginAsync(user,info);
                    await signInManager.SignInAsync(user,isPersistent: false);

					return LocalRedirect(returnUrl);
				}
                else
                {
                    ViewBag.ErrorMessage = "Please contact Support";
                    ViewBag.ErrorTitle = $"Email Claim not recived from :{info.LoginProvider}";
                    return View("Error");

				}
               
            }
		}
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && (await userManager.IsEmailConfirmedAsync(user)))
                {
					var token = await userManager.GeneratePasswordResetTokenAsync(user);
					var resetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, token = token }, Request.Scheme);
					logger.LogError(resetPasswordLink);
					return View("ForgotPasswordConfirmation");
				}
                return View("ForgotPasswordConfirmation");
            }
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string Email,string token)
		{
            if (Email == null || token == null)
            {
                ModelState.AddModelError("", "Invalid Token");
            }
			return View();
		}
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
            if (ModelState.IsValid)
            {
				var user = await userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
                    var result = await userManager.ResetPasswordAsync(user, model.token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
							await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
						}
                      
						return View("ResetPasswordConfirmation");
					}
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
						return View(model);
					}
                }
                return View("ResetPasswordConfirmation");
			}
            
			return View(model);
		}


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
			var user = await userManager.GetUserAsync(User);
			if (!(await userManager.HasPasswordAsync(user)))
			{
				return RedirectToAction("AddPassword");
			}
			//	var email = User.FindFirstValue(ClaimTypes.Email);
			return View();
        }
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var result = await userManager.ChangePasswordAsync(user, model.oldPassword, model.newPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                   return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("ChangePasswordConfirmation");


            }
            //var email = User.FindFirstValue(ClaimTypes.Email);
            //var user = await userManager.FindByEmailAsync(email); or
            return View(model);
		}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (await userManager.HasPasswordAsync(user))
            {
                return RedirectToAction("ChangePassword");
            }
     
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var result = await userManager.AddPasswordAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    return View("AddPasswordConfirmation");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return View(model);
        }


		
	}
}