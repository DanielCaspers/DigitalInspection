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
			var viewModel = new BaseViewModel
			{
				Resource = "Home"
			};
			return View(viewModel);
		}

		public ActionResult About()
		{
			var viewModel = new BaseViewModel
			{
				Resource = "About"
			};
			return View(viewModel);
		}
	}
}