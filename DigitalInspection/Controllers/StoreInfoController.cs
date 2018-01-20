using System.Web.Mvc;
using DigitalInspection.Services;
using System.Threading.Tasks;

namespace DigitalInspection.Controllers
{
	public class StoreInfoController : BaseController
	{
		[AllowAnonymous]
		public JsonResult Json(string companyNumber)
		{
			var task = Task.Run(async () => {
				return await StoreInfoService.GetStoreInfo(companyNumber);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return Json(task.Result.StoreInfo, JsonRequestBehavior.AllowGet);
		}
	}
}
