using DigitalInspection.ViewModels;
using System.Web.Mvc;

namespace DigitalInspection.Controllers
{
	public class HomeController : Controller
	{

		public HomeController()
		{
		}

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