using DigitalInspection.Models;
using System.Web.Mvc;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.Controllers
{
	[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View(new BaseHomeViewModel());
		}

		public ActionResult About()
		{
			return View(new BaseAboutViewModel());
		}
	}
}