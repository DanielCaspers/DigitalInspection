using System;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Threading.Tasks;
using DigitalInspection.ViewModels.TabContainers;
using System.Linq;

namespace DigitalInspection.Controllers
{
	public class WorkOrdersController : BaseController
	{
		// TODO: Determine how to store WorkOrder -> Checklist relationship for persistence

		public WorkOrdersController()
		{
			_resource = "Work order";
		}

		// GET: Work Orders page and return response to index.cshtml
		public async Task<PartialViewResult> Index()
		{
			var task = await GetWorkOrdersViewModel();
			return PartialView(task);
		}

		// GET: _WorkOrderTable partial and return it to _WorkOrderTable.cshtml 
		public async Task<PartialViewResult> _WorkOrderTable()
		{
			var task = await GetWorkOrdersViewModel();
			return PartialView(task);
		}

		public async Task<PartialViewResult> _Customer(string id, bool canEdit = false)
		{
			TabContainerViewModel tabVM = new TabContainerViewModel
			{
				TabId = "customerTab",
				RouteId = id
			};
			return await GetWorkOrderViewModel(id, tabVM, canEdit);
		}

		public async Task<PartialViewResult> _Vehicle(string id, bool canEdit = false)
		{
			TabContainerViewModel tabVM = new TabContainerViewModel
			{
				TabId = "vehicleTab",
				RouteId = id
			};
			return await GetWorkOrderViewModel(id, tabVM, canEdit);
		}

		public async Task<PartialViewResult> _Inspection(string id)
		{
			TabContainerViewModel tabVM = new TabContainerViewModel
			{
				TabId = "inspectionTab",
				RouteId = id
			};
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(id);
			});
			var checklists = _context.Checklists.OrderBy(c => c.Name).ToList();

			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return PartialView(new WorkOrderInspectionViewModel
			{
				WorkOrder = task.Result,
				TabViewModel = tabVM,
				Checklists = checklists
			});
		}

		[HttpPost]
		public async Task<ActionResult> SaveVehicle(string id, WorkOrderDetailViewModel vm)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrder(vm.WorkOrder);
			});
			// Request save with lock
						// If successful, redirect to _vehicle
			return RedirectToAction("_Vehicle", new { id = id });
			// if unsuccessful, return error toast with refresh setting
		}

		private async Task<WorkOrderMasterViewModel> GetWorkOrdersViewModel()
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

		private async Task<PartialViewResult> GetWorkOrderViewModel(string id, TabContainerViewModel tabVM, bool canEdit = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return PartialView(new WorkOrderDetailViewModel
			{
				WorkOrder = task.Result,
				CanEdit = canEdit,
				TabViewModel = tabVM
			});
		}
	}
}
