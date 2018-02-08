using DigitalInspection.Models;
using System.Web.Mvc;

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

	}
}
