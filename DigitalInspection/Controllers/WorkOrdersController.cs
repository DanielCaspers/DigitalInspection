using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Threading.Tasks;
using DigitalInspection.ViewModels.TabContainers;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using DigitalInspection.Models.DTOs;

namespace DigitalInspection.Controllers
{
	public class WorkOrdersController : BaseController
	{

		public WorkOrdersController()
		{
			ResourceName = "Work order";
		}

		// GET: Work Orders page and return response to index.cshtml
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public async Task<PartialViewResult> Index()
		{
			var task = await GetWorkOrdersViewModel();
			return PartialView(task);
		}

		// GET: _WorkOrderTable partial and return it to _WorkOrderTable.cshtml 
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public async Task<PartialViewResult> _WorkOrderTable()
		{
			var task = await GetWorkOrdersViewModel();
			return PartialView(task);
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult _Customer(string id, bool canEdit = false)
		{
			TabContainerViewModel tabVM = new TabContainerViewModel
			{
				TabId = "customerTab",
				RouteId = id
			};
			return GetWorkOrderViewModel(id, tabVM, canEdit);
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult _Vehicle(string id, bool canEdit = false)
		{
			TabContainerViewModel tabVM = new TabContainerViewModel
			{
				TabId = "vehicleTab",
				RouteId = id
			};
			return GetWorkOrderViewModel(id, tabVM, canEdit);
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult _Inspection(string id)
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
				WorkOrder = task.Result.WorkOrder,
				TabViewModel = tabVM,
				Checklists = checklists
			});
		}

		[AllowAnonymous]
		public JsonResult Json(string workOrderId)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(workOrderId, false);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var workOrder = task.Result.WorkOrder;

			return Json(workOrder, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult SaveCustomer(string id, WorkOrderDetailViewModel vm, bool releaselockonly = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrder(vm.WorkOrder, releaselockonly);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return RedirectToAction("_Customer", new { id = id });
			}
			else
			{
				// TODO Return read only with toast
				// return DisplayErrorToast(task.Result);
				return new EmptyResult();
			}
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult SaveVehicle(string id, WorkOrderDetailViewModel vm)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrder(vm.WorkOrder);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return RedirectToAction("_Vehicle", new { id = id });
			}
			else
			{
				// TODO Return read only with toast
				// return DisplayErrorToast(task.Result);
				return new EmptyResult();
			}
		}

		private async Task<WorkOrderMasterViewModel> GetWorkOrdersViewModel()
		{
			IList<WorkOrder> workOrders;

			if (HttpContext.User.IsInRole(AuthorizationRoles.ADMIN))
			{
				var task = Task.Run(async () => {
					return await WorkOrderService.GetWorkOrders();
				});
				// Force Synchronous run for Mono to work. See Issue #37
				task.Wait();
				workOrders = task.Result;
			}
			else
			{
				var task = Task.Run(async () => {
					return await WorkOrderService.GetWorkOrdersForTech(
						Request.GetOwinContext().Authentication.User.Claims
							.Where(c => c.Type.ToString() == ClaimTypes.NameIdentifier)
							.Select(c => c.Value)
							.FirstOrDefault()
						);
				});
				// Force Synchronous run for Mono to work. See Issue #37
				task.Wait();
				workOrders = task.Result;
			}

			return new WorkOrderMasterViewModel
			{
				WorkOrders = workOrders
			};
		}

		private PartialViewResult GetWorkOrderViewModel(string id, TabContainerViewModel tabVM, bool canEdit = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(id, canEdit);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			ToastViewModel toast = null;
			if (task.Result.IsSuccessStatusCode == false)
			{
				toast = DisplayErrorToast(task.Result);
			}
			return PartialView(new WorkOrderDetailViewModel
			{
				WorkOrder = task.Result.WorkOrder,
				CanEdit = canEdit,
				TabViewModel = tabVM,
				Toast = toast
			});
		}

		private ToastViewModel DisplayErrorToast(WorkOrderResponse response)
		{
			switch (response.HTTPCode)
			{
				case HttpStatusCode.NotFound:
					return ToastService.ResourceNotFound(ResourceName, ToastActionType.NavigateBack);
				case (HttpStatusCode)423:
					return ToastService.FileLockedByAnotherClient(response.ErrorMessage, ToastActionType.Refresh);
				case (HttpStatusCode)428:
					return ToastService.FileLockRequired();
				default:
					return ToastService.UnknownErrorOccurred(response.HTTPCode, response.ErrorMessage);
			}
		}
	}
}
