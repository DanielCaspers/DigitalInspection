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
using Claim = System.Security.Claims.Claim;

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

			var workOrder = GetWorkOrderResponse(CurrentUserClaims, id).WorkOrder;

			var checklists = _context.Checklists.OrderBy(c => c.Name).ToList();

			return PartialView(new WorkOrderInspectionViewModel
			{
				WorkOrder = workOrder,
				TabViewModel = tabVM,
				Checklists = checklists,
				InspectionId = _context.Inspections.Where(i => i.WorkOrderId == id).Select(i => i.Id).SingleOrDefault()
			});
		}

		[AllowAnonymous]
		public ActionResult Json(Guid inspectionId)
		{
			var workOrderId = _context.Inspections.Where(i => i.Id == inspectionId).Select(i => i.WorkOrderId).Single();
			return BuildJsonInternal(workOrderId);
		}

		[AllowAnonymous]
		public ActionResult JsonForOrder(string workOrderId)
		{
			return BuildJsonInternal(workOrderId);
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult SaveCustomer(string id, WorkOrderDetailViewModel vm, bool releaselockonly = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrder(CurrentUserClaims, vm.WorkOrder, releaselockonly);
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
				return await WorkOrderService.SaveWorkOrder(CurrentUserClaims, vm.WorkOrder);
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
					return await WorkOrderService.GetWorkOrders(CurrentUserClaims);
				});
				// Force Synchronous run for Mono to work. See Issue #37
				task.Wait();
				workOrders = task.Result;
			}
			else
			{
				var task = Task.Run(async () => {
					return await WorkOrderService.GetWorkOrdersForTech(CurrentUserClaims);
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
			var workOrderResponse = GetWorkOrderResponse(CurrentUserClaims, id, canEdit);

			ToastViewModel toast = null;
			if (workOrderResponse.IsSuccessStatusCode == false)
			{
				toast = DisplayErrorToast(workOrderResponse);
			}
			return PartialView(new WorkOrderDetailViewModel
			{
				WorkOrder = workOrderResponse.WorkOrder,
				CanEdit = canEdit,
				TabViewModel = tabVM,
				Toast = toast
			});
		}

		private WorkOrderResponse GetWorkOrderResponse(string workOrderId, bool canEdit = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(workOrderId, canEdit);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return task.Result;
		}

		private WorkOrderResponse GetWorkOrderResponse(IEnumerable<Claim> userClaims, string workOrderId, bool canEdit = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(userClaims, workOrderId, canEdit);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return task.Result;
		}

		private ActionResult BuildJsonInternal(string workOrderId)
		{
			if (workOrderId == null)
			{
				return HttpNotFound();
			}

			var workOrder = GetWorkOrderResponse(workOrderId).WorkOrder;

			return Json(workOrder, JsonRequestBehavior.AllowGet);
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
