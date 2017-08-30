using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using System.Web.Mvc;

namespace DigitalInspection.Controllers
{
	public class HomeController : Controller
	{

		public HomeController()
		{
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult Index()
		{
			return View(new BaseHomeViewModel());
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult About()
		{
			return View(new BaseAboutViewModel());
		}
	}
}