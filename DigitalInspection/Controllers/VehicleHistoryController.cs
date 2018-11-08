using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using DigitalInspection.Models;
using DigitalInspection.Models.Orders;
using DigitalInspection.Services.Web;
using DigitalInspection.ViewModels.VehicleHistory;

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

			var vehicleHistoryItems = AddInspectionsToVehicleHistory(task.Result.Entity);

			return PartialView("../Shared/Dialogs/_VehicleHistoryDialog", new VehicleHistoryViewModel
			{
				VehicleHistory = vehicleHistoryItems
			});
		}

		private IList<VehicleHistoryItem> AddInspectionsToVehicleHistory(IList<VehicleHistoryItem> vehicleHistoryItems)
		{
			var workOrderIds = vehicleHistoryItems.Select(i => i.OrderId).Distinct().ToList();
			var inspectionsForOrders = _context.Inspections.Where(i => workOrderIds.Contains(i.WorkOrderId));

			foreach (var inspection in inspectionsForOrders)
			{
				var historyItem = vehicleHistoryItems.FirstOrDefault(item => item.OrderId == inspection.WorkOrderId);
				if (historyItem != null)
				{
					historyItem.InspectionId = inspection.Id;
				}
			}

			return vehicleHistoryItems;
		}
	}
}
