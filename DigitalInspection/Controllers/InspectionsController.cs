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
using DigitalInspection.Models.Orders;
using System.Web;
using System.Web.Helpers;
using System.Data.Entity.Validation;

namespace DigitalInspection.Controllers
{
	public class InspectionsController : BaseController
	{
		private static readonly string IMAGE_DIRECTORY = "Inspections";
		private static readonly string _subresource = "Inspection Item";

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
		// TODO: ChecklistItemId can be removed since it will exist on the InspectionItem.ChecklistItem.Id property
		public ActionResult Condition(Guid inspectionItemId, Guid checklistItemId, RecommendedServiceSeverity inspectionItemCondition)
		{
			// Save Condition
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}
			inspectionItemInDb.Condition = inspectionItemCondition;

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}

			// Prepare updated multiselect list for client
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var filteredCRs = checklistItem.CannedResponses.Where(cr => cr.LevelsOfConcern.Contains(inspectionItemCondition)).ToList();
			var options = filteredCRs.Select(cr => new { label = cr.Response, title = cr.Response, value = cr.Id }).ToList();
			var response = new
			{
				filteredCannedResponses = options,
				checklistItemId = checklistItem.Id
			};
			return Json(response);
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		// TODO: All of these inspectionIds and checklistIds should probably become inspectionItemId once available
		public ActionResult CannedResponse(Guid inspectionItemId, InspectionDetailViewModel vm)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			IList<Guid> selectedCannedResponseIds = vm.Inspection.InspectionItems
											.SingleOrDefault(inspItem => inspItem.Id == inspectionItemId)
											?.SelectedCannedResponseIds;

			foreach (var cannedResponse in inspectionItemInDb.CannedResponses)
			{
				_context.CannedResponses.Attach(cannedResponse);
			}

			IList<CannedResponse> cannedResponsesInDb = new List<CannedResponse>();
			foreach (Guid crId in selectedCannedResponseIds)
			{
				cannedResponsesInDb.Add(_context.CannedResponses.Single(cr => cr.Id == crId));
			}

			inspectionItemInDb.CannedResponses = cannedResponsesInDb;

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return new EmptyResult();
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult Note(AddInspectionNoteViewModel NoteVM)
		{
			// Save Note
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == NoteVM.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}
			inspectionItemInDb.Note = NoteVM.Note;

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}

			return new EmptyResult();
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult Measurements(AddMeasurementViewModel MeasurementsVM)
		{
			// TODO Needs to take an inspection somehow
			return new EmptyResult();
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult Photo(UploadInspectionPhotosViewModel photoVM)
		{
			// TODO: Use inspections and a real id instead of fake guid

			// TODO make Guid in file path use inspectionItemId
			Image image = ImageService.SaveImage(photoVM.Picture, new string[] { IMAGE_DIRECTORY, photoVM.WorkOrderId, "35066ff7-ebd7-4157-9e5b-b3af0fdd0000" }, Guid.NewGuid().ToString(), false);

			// Local test
			// return RedirectToAction("Index", new { workOrderId= "004584155" , checklistId=Guid.Parse("35066ff7-ebd7-4157-9e5b-b3af0fddcd98") });
			// Weekly staging server test - Mechanics checklist
			return RedirectToAction("Index", new { workOrderId = "004584155", checklistId = Guid.Parse("a8231f45-6d7c-4384-93d4-28fcc892fe57") });

		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetAddMeasurementDialog(Guid inspectionId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			return PartialView("_AddMeasurementDialog", new AddMeasurementViewModel
			{
				ChecklistItem = checklistItem,
				Measurements = checklistItem.Measurements
				// Needs to know about inspection measurements, and should be able to match up measurements and inspection measurements
			});
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetAddInspectionNoteDialog(Guid inspectionItemId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			return PartialView("_AddInspectionNoteDialog", new AddInspectionNoteViewModel
			{
				InspectionItem = inspectionItem,
				ChecklistItem = checklistItem,
				Note = inspectionItem.Note
			});
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetUploadInspectionPhotosDialog(Guid inspectionId, Guid checklistItemId, string workOrderId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			return PartialView("_UploadInspectionPhotosDialog", new UploadInspectionPhotosViewModel
			{
				ChecklistItem = checklistItem,
				WorkOrderId = workOrderId
			});
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetViewInspectionPhotosDialog(Guid inspectionId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);

			var imageSources = new List<string>();
			// TODO
			//foreach (Image image in inspectionItem)
			//{
			//	imageSources.Add(Path.Combine("/Uploads/Inspections/", workOrderId, image.Title));
			//}
			return PartialView("_ViewInspectionPhotosDialog", new ViewInspectionPhotosViewModel
			{
				ChecklistItem = checklistItem,
				ImageSources = imageSources
			});
		}

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

			// Sort all canned responses by response
			checklist.ChecklistItems.ToList().ForEach(ci =>
			{
				ci.CannedResponses = ci.CannedResponses.OrderBy(cr => cr.Response).ToList();
			});

			// TODO: Remove fake code once merge is working
			var inspectionItems = _context.InspectionItems.ToList();
			inspectionItems[0].Id = Guid.Parse("4c5501c2-7cd6-4311-96cc-cbbbcf3d6cbb");
			inspectionItems[0].SelectedCannedResponseIds = inspectionItems[0].CannedResponses.Select(cr => cr.Id).ToList();
			var inspection = new Inspection
			{
				InspectionItems = inspectionItems
			};
			return PartialView(new InspectionDetailViewModel
			{
				WorkOrder = task.Result.WorkOrder,
				Checklist = checklist,
				Inspection = inspection,
				Toast = toast,
				AddMeasurementVM = new AddMeasurementViewModel { },
				AddInspectionNoteVM = new AddInspectionNoteViewModel {
					InspectionItem = inspectionItems[0],
					Note = inspectionItems[0].Note
				},
				UploadInspectionPhotosVM = new UploadInspectionPhotosViewModel {},
				ViewInspectionPhotosVM = new ViewInspectionPhotosViewModel {}
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
