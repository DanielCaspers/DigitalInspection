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
using System.IO;

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
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == MeasurementsVM.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}

			InspectionMeasurement inspectionMeasurementInDb;
			foreach (InspectionMeasurement inspectionMeasurement in MeasurementsVM.InspectionItem.InspectionMeasurements)
			{
				inspectionMeasurementInDb = _context.InspectionMeasurements.SingleOrDefault(im => im.Id == inspectionMeasurement.Id);
				inspectionMeasurementInDb.Value = inspectionMeasurement.Value;
			}

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
		public ActionResult Photo(UploadInspectionPhotosViewModel photoVM)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == photoVM.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}
			// New guid is used as a random prefix to the filename to ensure uniqueness
			Image imageDto = ImageService.SaveImage(photoVM.Picture, new string[] { IMAGE_DIRECTORY, photoVM.WorkOrderId, photoVM.InspectionItem.Id.ToString() }, Guid.NewGuid().ToString(), false);

			InspectionImage image = new InspectionImage
			{
				Id = imageDto.Id,
				Title = imageDto.Title,
				CreatedDate = imageDto.CreatedDate,
				ImageUrl = imageDto.ImageUrl,
				InspectionItem = inspectionItemInDb
			};

			inspectionItemInDb.InspectionImages.Add(image);
			_context.InspectionImages.Add(image);

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}

			return RedirectToAction("Index", new { workOrderId = photoVM.WorkOrderId, checklistId = photoVM.ChecklistId });
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetAddMeasurementDialog(Guid inspectionItemId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			return PartialView("_AddMeasurementDialog", new AddMeasurementViewModel
			{
				ChecklistItem = checklistItem,
				InspectionItem = inspectionItem,
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
		public PartialViewResult GetUploadInspectionPhotosDialog(Guid inspectionItemId, Guid checklistItemId, Guid checklistId, string workOrderId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			return PartialView("_UploadInspectionPhotosDialog", new UploadInspectionPhotosViewModel
			{

				InspectionItem = inspectionItem,
				ChecklistItem = checklistItem,
				WorkOrderId = workOrderId
			});
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult GetViewInspectionPhotosDialog(Guid inspectionItemId, Guid checklistItemId, string workOrderId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			IList<string> imageSources = inspectionItem.InspectionImages
				.Select((image) => Path.Combine($"/Uploads/{IMAGE_DIRECTORY}/{workOrderId}/{inspectionItemId.ToString()}/", image.Title))
				.ToList();

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

			// TODO: Remove fake code to end of func once merge is working
			var inspectionItems = _context.InspectionItems.ToList();
			var fakeInspectionItemId = Guid.Parse("4c5501c2-7cd6-4311-96cc-cbbbcf3d6cbb");
			inspectionItems[0].Id = fakeInspectionItemId;

			// Filter canned responses down to IDs
			inspectionItems[0].SelectedCannedResponseIds = inspectionItems[0].CannedResponses.Select(cr => cr.Id).ToList();
			inspectionItems[0].InspectionMeasurements = _context.InspectionMeasurements.Where(im => im.InspectionItem.Id == fakeInspectionItemId).ToList();
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
				UploadInspectionPhotosVM = new UploadInspectionPhotosViewModel {
					WorkOrderId = workOrderId
				},
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
