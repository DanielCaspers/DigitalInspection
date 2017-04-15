using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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