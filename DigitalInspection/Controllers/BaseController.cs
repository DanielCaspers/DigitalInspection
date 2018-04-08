using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;
using DigitalInspection.Models;
using System.Web.Mvc;
using DigitalInspection.Services;
using DigitalInspection.Services.Web;
using DigitalInspection.ViewModels;

namespace DigitalInspection.Controllers
{
	public abstract class BaseController : Controller
	{
		protected string ResourceName;

		protected ApplicationDbContext _context;

		protected BaseController()
		{
			_context = new ApplicationDbContext();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
		}

		protected IEnumerable<Claim> CurrentUserClaims
		{
			get { return Request.GetOwinContext().Authentication.User.Claims; }
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			filterContext.ExceptionHandled = true;
			var exception = filterContext.Exception;

			var info = new HandleErrorInfo(
				exception,
				filterContext.RouteData.Values["controller"].ToString(),
				filterContext.RouteData.Values["action"].ToString()
			);

			filterContext.Result = this.View(
				"~/Views/Shared/Error.cshtml",
				new BaseErrorModel() {
					Toast = ToastService.UnknownErrorOccurred(exception, info),
					Error = info,
					StackTrace = new StackTrace(exception)
				}
			);
		}

		// Retreives company number from cookie
		protected string GetCompanyNumber()
		{
			return Request.Cookies.Get(CookieFactory.CompanyCookieName)?.Value;
		}
	}
}
