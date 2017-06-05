using System;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Threading.Tasks;

namespace DigitalInspection.Controllers
{
	public class WorkOrdersController : BaseController
	{
		// TODO: Determine how to store WorkOrder -> Checklist relationship for persistence

		public WorkOrdersController()
		{
			_resource = "Work order";
		}

		private async Task<WorkOrderMasterViewModel> GetWorkOrderViewModel()
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrders();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return new WorkOrderMasterViewModel
			{
				WorkOrders = task.Result
			};
		}

		// GET: Work Orders page and return response to index.cshtml
		public async Task<PartialViewResult> Index()
		{
			var task = await GetWorkOrderViewModel();
			return PartialView(task);
		}

		// GET: _WorkOrderTable partial and return it to _WorkOrderTable.cshtml 
		public async Task<PartialViewResult> _WorkOrderTable()
		{
			var task = await GetWorkOrderViewModel();
			return PartialView(task);
		}

		public async Task<PartialViewResult> _Vehicle(string id)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return PartialView(new WorkOrderDetailViewModel {
				WorkOrder = task.Result
			});
		}

		public async Task<PartialViewResult> _VehicleEdit(string id)
		{
			var task = Task.Run(async () => {
				// TODO, actually request the lock
				return await WorkOrderService.GetWorkOrder(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return PartialView(new WorkOrderDetailViewModel
			{
				WorkOrder = task.Result
			});
		}

	}
}
