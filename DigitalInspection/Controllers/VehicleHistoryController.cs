using System.Web.Mvc;
using DigitalInspection.Services;
using System.Threading.Tasks;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;

namespace DigitalInspection.Controllers
{
	public class VehicleHistoryController : BaseController
	{
		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetVehicleHistoryDialog(string VIN)
		{
			var task = Task.Run(async () => {
				return await VehicleHistoryService.GetVehicleHistory(CurrentUserClaims, VIN, GetCompanyNumber());
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return PartialView("../Shared/Dialogs/_VehicleHistoryDialog", new VehicleHistoryViewModel
			{
				VehicleHistory = task.Result.VehicleHistory
			});
		}
	}
}
