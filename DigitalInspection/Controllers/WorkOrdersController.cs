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
		#region Static Helpers

		private const string CustomerViewName = "_Customer";
		private const string VehicleViewName = "_Vehicle";

		private static TabContainerViewModel BuildCustomerTab(string routeId)
		{
			return new TabContainerViewModel()
			{
				TabId = "customerTab",
				RouteId = routeId
			};
		}

		private static TabContainerViewModel BuildInspectionTab(string routeId)
		{
			return new TabContainerViewModel()
			{
				TabId = "inspectionTab",
				RouteId = routeId
			};
		}

		private static TabContainerViewModel BuildVehicleTab(string routeId)
		{
			return new TabContainerViewModel()
			{
				TabId = "vehicleTab",
				RouteId = routeId
			};
		}

		#endregion

		public WorkOrdersController()
		{
			ResourceName = "Work order";
		}

		#region Partial View Actions

		// GET: Work Orders page and return response to index.cshtml
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public async Task<PartialViewResult> Index()
		{
			var task = await GetWorkOrdersViewModel();
			return PartialView(task);
		}

		// GET: _WorkOrderTable partial and return it to _WorkOrderTable.cshtml 
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public async Task<PartialViewResult> _WorkOrderTable()
		{
			var task = await GetWorkOrdersViewModel();
			return PartialView(task);
		}

		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult _Customer(string id, bool canEdit = false)
		{
			return GetWorkOrderViewModel(id, BuildCustomerTab(id), CustomerViewName, canEdit );
		}

		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult _Vehicle(string id, bool canEdit = false)
		{
			return GetWorkOrderViewModel(id, BuildVehicleTab(id), VehicleViewName, canEdit);
		}

		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult _Inspection(string id)
		{
			var workOrder = GetWorkOrderResponse(CurrentUserClaims, id).WorkOrder;

			return PartialView(new WorkOrderInspectionViewModel
			{
				WorkOrder = workOrder,
				TabViewModel = BuildInspectionTab(id),
				Checklists = _context.Checklists.OrderBy(c => c.Name).ToList(),
				InspectionId = _context.Inspections.Where(i => i.WorkOrderId == id).Select(i => i.Id).SingleOrDefault()
			});
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult ReleaseCustomerFileLock(string id)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.ReleaseLock(CurrentUserClaims, id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return RedirectToAction(CustomerViewName, new { id = id });
			}
			else
			{
				return GetWorkOrderDetailViewModel(task.Result, BuildCustomerTab(id), false, CustomerViewName);
			}
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult ReleaseVehicleFileLock(string id)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.ReleaseLock(CurrentUserClaims, id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return RedirectToAction(VehicleViewName, new { id = id });
			}
			else
			{
				return GetWorkOrderDetailViewModel(task.Result, BuildVehicleTab(id), false, VehicleViewName);
			}
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult SaveCustomer(string id, WorkOrderDetailViewModel vm, bool releaselockonly = false)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrder(CurrentUserClaims, vm.WorkOrder, releaselockonly);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return RedirectToAction(CustomerViewName, new { id = id });
			}
			else
			{
				return GetWorkOrderDetailViewModel(task.Result, BuildCustomerTab(id), false, CustomerViewName);
			}
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult SaveVehicle(string id, WorkOrderDetailViewModel vm)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrder(CurrentUserClaims, vm.WorkOrder);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return RedirectToAction(VehicleViewName, new { id = id });
			}
			else
			{
				return GetWorkOrderDetailViewModel(task.Result, BuildVehicleTab(id), false, VehicleViewName);
			}
		}

		#endregion

		#region Anonymous Access APIs

		[AllowAnonymous]
		public ActionResult Json(Guid inspectionId)
		{
			var workOrderId = _context.Inspections.Single(i => i.Id == inspectionId).WorkOrderId;
			return BuildJsonInternal(workOrderId);
		}

		[AllowAnonymous]
		public ActionResult JsonForOrder(string workOrderId)
		{
			return BuildJsonInternal(workOrderId);
		}

		#endregion

		#region ViewModel Helpers

		private async Task<WorkOrderMasterViewModel> GetWorkOrdersViewModel()
		{
			Task<IList<WorkOrder>> task;

			if (HttpContext.User.IsInRole(Roles.ServiceAdvisor))
			{
				task = Task.Run(async () => {
					return await WorkOrderService.GetWorkOrdersForServiceAdvisor(CurrentUserClaims);
				});
			}
			else if (HttpContext.User.IsInRole(Roles.Technician) || HttpContext.User.IsInRole(Roles.User))
			{
				task = Task.Run(async () => {
					return await WorkOrderService.GetWorkOrdersForTech(CurrentUserClaims);
				});
			}
			else // Admin, LocationManager
			{
				task = Task.Run(async () => {
					return await WorkOrderService.GetWorkOrders(CurrentUserClaims);
				});
			}

			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();
			return new WorkOrderMasterViewModel
			{
				WorkOrders = task.Result
			};
		}	

		private PartialViewResult GetWorkOrderViewModel(string id, TabContainerViewModel tabVM, string viewName, bool canEdit = false)
		{
			var workOrderResponse = GetWorkOrderResponse(CurrentUserClaims, id, canEdit);

			return GetWorkOrderDetailViewModel(workOrderResponse, tabVM, canEdit, viewName);
		}

		private PartialViewResult GetWorkOrderDetailViewModel(WorkOrderResponse response, TabContainerViewModel tabVM, bool canEdit, string viewName)
		{
			return PartialView(
				viewName,
				new WorkOrderDetailViewModel
				{
					WorkOrder = response.WorkOrder,
					CanEdit = canEdit,
					TabViewModel = tabVM,
					Toast = response.IsSuccessStatusCode ? null : DisplayErrorToast(response)
				}
			);
		}

		#endregion

		#region Response Helpers

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

		#endregion

	}
}
