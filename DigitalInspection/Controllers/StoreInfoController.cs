using System.Web.Mvc;
using System.Threading.Tasks;
using DigitalInspection.Services.Web;

namespace DigitalInspection.Controllers
{
	public class StoreInfoController : BaseController
	{
		[AllowAnonymous]
		public JsonResult Json(string companyNumber)
		{
			var task = Task.Run(async () => {
				return await StoreInfoHttpService.GetStoreInfo(companyNumber);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return Json(task.Result.Entity, JsonRequestBehavior.AllowGet);
		}
	}
}
