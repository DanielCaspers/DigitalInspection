using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using DigitalInspection.Services.Web;

namespace DigitalInspection.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public AccountController()
		{
			ResourceName = "Accounts";
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			private set => _signInManager = value;
		}

		public ApplicationUserManager UserManager
		{
			get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			private set => _userManager = value;
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ViewResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			var task = Task.Run(async () => {
				return await AuthenticationService.Login(model.Username, model.Password);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();
			var response = task.Result;

			if (response.IsSuccessStatusCode)
			{
				var userIdCookie = Request.Cookies.Get(CookieFactory.UserIdCookieName);
				// If this user is not the same as last user, mutate the cookie.
				var userClaims = response.Entity.Claims.ToList();
				var userId = userClaims.Single(c => c.Type == "empID").Value;
				if (userIdCookie?.Value != userId)
				{
					var userCompany = userClaims.First(c => c.Type == "roles").Value?.Substring(0,3);
					Response.Cookies.Add(CookieFactory.CreateCompanyCookie(userCompany));
					Response.Cookies.Add(CookieFactory.CreateUserIdCookie(userId));
				}
				// Else, leave user's preferred sign in

				// Set ASP.NET Application Cookie
				HttpContext.GetOwinContext().Authentication.SignIn(
				   new AuthenticationProperties { IsPersistent = true }, response.Entity);
				return RedirectToLocal(returnUrl);
			}
			else
			{
				model.Toast = ToastService.Error(response.ErrorMessage, ToastActionType.Close);
				return View(model);
			}
		}

		//
		// POST: /Account/Logout
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Logout()
		{
			var task = Task.Run(async () => {
				return await AuthenticationService.Logout(CurrentUserClaims);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
				return RedirectToAction("Login", "Account");
			}
			else
			{
				var model = new LoginViewModel()
				{
					Toast = ToastService.Error(task.Result.ErrorMessage, ToastActionType.Close)
				};
				return View("Login", model);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers
		private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl) == false || returnUrl == "/Account/Logout")
			{
				return RedirectToAction("Index", "WorkOrders");
			}

			return Redirect(returnUrl);
		}
		#endregion
	}
}