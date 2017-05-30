using DigitalInspection.Models;
using System.Web.Mvc;

namespace DigitalInspection.Controllers
{
	public class BaseController : Controller
	{
		protected string _resource;
		protected ApplicationDbContext _context;

		public BaseController()
		{
			_context = new ApplicationDbContext();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
		}

	}
}