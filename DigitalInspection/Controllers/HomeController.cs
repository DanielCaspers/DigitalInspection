using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using System.Web.Mvc;

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