using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Threading.Tasks;
using DigitalInspection.ViewModels.TabContainers;
using System.Net;
using System.Linq;
using System;
using System.Collections.Generic;

namespace DigitalInspection.Controllers
{
	public class InspectionsController : BaseController
	{
		public InspectionsController()
		{
			_resource = "Inspection";
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult Index(string workOrderId, Guid checklistId)
		{
			return GetInspectionViewModel(workOrderId, checklistId);
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetAddMeasurementDialog(Guid id)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == id);
			return PartialView("_AddMeasurementDialog", new AddMeasurementViewModel
			{
				Measurements = checklistItem.Measurements
			});
		}


		//[HttpPost]
		//[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		//public ActionResult SaveInspection(string id, WorkOrderDetailViewModel vm, bool releaselockonly = false)
		//{
		//	var task = Task.Run(async () => {
		//		return await WorkOrderService.SaveWorkOrder(vm.WorkOrder, releaselockonly);
		//	});
		//	// Force Synchronous run for Mono to work. See Issue #37
		//	task.Wait();

		//	if (task.Result.IsSuccessStatusCode)
		//	{
		//		return RedirectToAction("_Customer", new { id = id });
		//	}
		//	else
		//	{
		//		// TODO Return read only with toast
		//		// return DisplayErrorToast(task.Result);
		//		return new EmptyResult();
		//	}
		//}

		private PartialViewResult GetInspectionViewModel(string workOrderId, Guid checklistId)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(workOrderId, false);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			ToastViewModel toast = null;

			var checklist = _context.Checklists.SingleOrDefault(c => c.Id == checklistId);

			if (task.Result.IsSuccessStatusCode == false)
			{
				toast = DisplayErrorToast(task.Result);
			}
			else if(checklist == null)
			{
				toast = ToastService.ResourceNotFound(_resource, ToastActionType.NavigateBack);
			}
			return PartialView(new InspectionDetailViewModel
			{
				WorkOrder = task.Result.WorkOrder,
				Checklist = checklist,
				Toast = toast,
				AddMeasurementVM = new AddMeasurementViewModel
				{
					Measurements = new List<Measurement>() {}
				},
				AddInspectionNoteVM = new AddInspectionNoteViewModel { Note = ""},
				UploadInspectionPhotosVM = new UploadInspectionPhotosViewModel { }
			});
		}

		private ToastViewModel DisplayErrorToast(WorkOrderResponse response)
		{
			switch (response.HTTPCode)
			{
				case HttpStatusCode.NotFound:
					return ToastService.ResourceNotFound(_resource, ToastActionType.NavigateBack);
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
