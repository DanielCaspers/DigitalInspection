using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using System.Security.Claims;

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
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set 
			{ 
				_signInManager = value; 
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ViewResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		// See https://stackoverflow.com/questions/31584506/how-to-implement-custom-authentication-in-asp-net-mvc-5
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{

			if (UserManager.IsValid(model.Username, model.Password))
			{
				var ident = new ClaimsIdentity(
				  new[] { 
			  // adding following 2 claim just for supporting default antiforgery provider
			  new Claim(ClaimTypes.NameIdentifier, "4067"),
			  new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

			  // TODO: Change to extract this from the JWT returned
			  new Claim(ClaimTypes.Name, model.Username),

			  // optionally you could add roles if any
			  new Claim(ClaimTypes.Role, model.Password == "admin"? AuthorizationRoles.ADMIN : AuthorizationRoles.USER),

				  },
				  DefaultAuthenticationTypes.ApplicationCookie);

				HttpContext.GetOwinContext().Authentication.SignIn(
				   new AuthenticationProperties { IsPersistent = true }, ident);
				return RedirectToLocal(returnUrl);
			}
			else
			{
				model.Toast = new ToastViewModel()
				{
					Type = ToastType.Error,
					Message = "Username or password mismatch.",
					Icon = "error",
					Action = ToastActionType.Close
				};
				return View(model);
			}
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// POST: /Account/Logout
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Logout()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Login", "Account");
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
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}