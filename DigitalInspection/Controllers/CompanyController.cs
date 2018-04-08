using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Services.Web;

namespace DigitalInspection.Controllers
{
	public class CompanyController : BaseController
	{
		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult ChangeCompany(string companyNumber)
		{
			Response.Cookies.Add(CookieFactory.CreateCompanyCookie(companyNumber));

			return RedirectToAction("Index", "WorkOrders");
		}
	}
}
