using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System;
using System.Collections.Generic;
using DigitalInspection.Models.Orders;
using System.Data.Entity.Validation;
using System.IO;
using DigitalInspection.ViewModels.TabContainers;

namespace DigitalInspection.Controllers
{
	public class InspectionsController : BaseController
	{
		private static readonly string IMAGE_DIRECTORY = "Inspections";
		private static readonly string _subresource = "Inspection Item";

		public InspectionsController()
		{
			ResourceName = "Inspection";
		}

		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult Index(string workOrderId, Guid checklistId, Guid? tagId)
		{
			return GetInspectionViewModel(workOrderId, checklistId, tagId);
		}

		[AllowAnonymous]
		public JsonResult ReportForOrder(string workOrderId, bool grouped = false)
		{
			var inspectionItems = _context.Inspections
				.Single(i => i.WorkOrderId == workOrderId)
				.InspectionItems;

			return BuildInspectionReportInternal(inspectionItems, grouped, workOrderId);
		}

		[AllowAnonymous]
		public JsonResult Report(Guid inspectionId, bool grouped = false)
		{
			string workOrderId = _context.Inspections.Single(i => i.Id == inspectionId).WorkOrderId;

			var inspectionItems = _context.Inspections
				.Single(i => i.Id == inspectionId)
				.InspectionItems;

			return BuildInspectionReportInternal(inspectionItems, grouped, workOrderId);
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult Condition(Guid inspectionItemId, RecommendedServiceSeverity inspectionItemCondition)
		{
			// Save Condition
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}

			// Only change condition and canned response if different from previous condition
			if (inspectionItemInDb.Condition != inspectionItemCondition)
			{
				inspectionItemInDb.Condition = inspectionItemCondition;

				// Clear canned response IDs when switching conditions.
				// This is because otherwise, the inspection table's select box and the DB (and thus the report) can get out of sync
				foreach (var cannedResponse in inspectionItemInDb.CannedResponses)
				{
					_context.CannedResponses.Attach(cannedResponse);
				}
				inspectionItemInDb.CannedResponses = new List<CannedResponse>();

				if (TrySave() == false)
				{
					return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
				}
			}


			// Prepare updated multiselect list for client
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == inspectionItemInDb.ChecklistItem.Id);
			var filteredCRs = checklistItem.CannedResponses.Where(cr => cr.LevelsOfConcern.Contains(inspectionItemCondition)).ToList();

			// Options may be selected in the case where we haven't changed to a new condition
			var options = filteredCRs.Select(cr => new { label = cr.Response, title = cr.Response, value = cr.Id, selected = inspectionItemInDb.CannedResponses.Any(c => c.Id == cr.Id) }).ToList();
			var response = new
			{
				filteredCannedResponses = options,
				checklistItemId = checklistItem.Id
			};
			return Json(response);
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
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

			TrySave();
			return new EmptyResult();
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult ItemNote(AddInspectionItemNoteViewModel itemNoteVm)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == itemNoteVm.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}
			inspectionItemInDb.Note = itemNoteVm.Note;

			if (TrySave() == false)
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}
			return new EmptyResult();
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult WorkOrderNote(AddInspectionWorkOrderNoteViewModel workOrderNoteVm)
		{
			if (workOrderNoteVm.Note == null)
			{
				workOrderNoteVm.Note = "";
			}

			// https://msdn.microsoft.com/en-us/library/tabh47cf(v=vs.110).aspx
			// NOTE: Cannot use Environment.NewLine since the filter will be less strict on Mono. 
			IList<string> returnCarriageSeparatedNotes = workOrderNoteVm.Note.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

			var task = Task.Run(async () => {
				return await WorkOrderService.SaveWorkOrderNote(CurrentUserClaims, workOrderNoteVm.WorkOrderId, GetCompanyNumber(), returnCarriageSeparatedNotes);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode)
			{
				return new EmptyResult();
			}
			else
			{
				return PartialView("Toasts/_Toast", DisplayErrorToast(task.Result));
			}
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
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

			if (TrySave() == false) {
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}
			return new EmptyResult();
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
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

			return RedirectToAction("Index", new { workOrderId = photoVM.WorkOrderId, checklistId = photoVM.ChecklistId, tagId = photoVM.TagId });
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetAddMeasurementDialog(Guid inspectionItemId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			return PartialView("_AddMeasurementDialog", new AddMeasurementViewModel
			{
				ChecklistItem = checklistItem,
				InspectionItem = inspectionItem,
				Measurements = checklistItem.Measurements
			});
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetAddInspectionWorkOrderNoteDialog(string workOrderId)
		{
			var workOrderResponse = GetWorkOrderResponse(workOrderId);
			var workOrder = workOrderResponse.WorkOrder;

			string combinedNote = string.Join(Environment.NewLine, workOrder.Notes);

			return PartialView("_AddInspectionWorkOrderNoteDialog", new AddInspectionWorkOrderNoteViewModel
			{
				WorkOrderId = workOrderId,
				Note = combinedNote
			});
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetAddInspectionItemNoteDialog(Guid inspectionItemId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			return PartialView("_AddInspectionItemNoteDialog", new AddInspectionItemNoteViewModel
			{
				InspectionItem = inspectionItem,
				ChecklistItem = checklistItem,
				Note = inspectionItem.Note
			});
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetUploadInspectionPhotosDialog(Guid inspectionItemId, Guid checklistItemId, Guid checklistId, Guid? tagId, string workOrderId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			return PartialView("_UploadInspectionPhotosDialog", new UploadInspectionPhotosViewModel
			{

				InspectionItem = inspectionItem,
				ChecklistItem = checklistItem,
				TagId = tagId,
				WorkOrderId = workOrderId
			});
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
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

		private PartialViewResult GetInspectionViewModel(string workOrderId, Guid checklistId, Guid? tagId)
		{
			var workOrderResponse = GetWorkOrderResponse(workOrderId);
			var workOrder = workOrderResponse.WorkOrder;
			ToastViewModel toast = null;

			var checklist = _context.Checklists.SingleOrDefault(c => c.Id == checklistId);

			if (workOrderResponse.IsSuccessStatusCode == false)
			{
				toast = DisplayErrorToast(workOrderResponse);
			}
			else if (checklist == null)
			{
				toast = ToastService.ResourceNotFound(ResourceName, ToastActionType.NavigateBack);
			}

			// Sort all canned responses by response
			checklist.ChecklistItems.ToList().ForEach(ci =>
			{
				ci.CannedResponses = ci.CannedResponses.OrderBy(cr => cr.Response).ToList();
			});

			var realInspection = GetOrCreateInspection(workOrder.Id, checklist);
			realInspection = GetOrCreateInspectionItems(checklist, realInspection);
			realInspection = GetOrCreateInspectionMeasurements(checklist, realInspection);

			// Filter inspection items
			Func<InspectionItem, bool> filterByChecklistsAndTags = ii => ii.ChecklistItem.Checklists.Any(c => c.Id == checklistId) && ii.ChecklistItem.Tags.Any(t => t.Id == tagId);
			Func<InspectionItem, bool> filterByChecklists = ii => ii.ChecklistItem.Checklists.Any(c => c.Id == checklistId);

			realInspection.InspectionItems = realInspection.InspectionItems
				.Where(tagId.HasValue ? filterByChecklistsAndTags : filterByChecklists)
				.OrderBy(ii => ii.ChecklistItem.Name)
				.ToList();

			foreach(var inspectionItem in realInspection.InspectionItems)
			{
				inspectionItem.SelectedCannedResponseIds = inspectionItem.CannedResponses.Select(cr => cr.Id).ToList();
			}

			return PartialView(new InspectionDetailViewModel
			{
				WorkOrder = workOrder,
				Checklist = checklist,
				Inspection = realInspection,
				Toast = toast,
				AddMeasurementVM = new AddMeasurementViewModel { },
				AddInspectionWorkOrderNoteVm = new AddInspectionWorkOrderNoteViewModel { },
				AddInspectionItemNoteVm = new AddInspectionItemNoteViewModel { },
				UploadInspectionPhotosVM = new UploadInspectionPhotosViewModel {
					WorkOrderId = workOrderId
				},
				ViewInspectionPhotosVM = new ViewInspectionPhotosViewModel { },
				ScrollableTabContainerVM = GetScrollableTabContainerViewModel(tagId),
				FilteringTagId = tagId
			});
		}

		/**
		 * Gets inspection from DB if already exists, or create one in the DB and then return it. 
		 */
		private Inspection GetOrCreateInspection(string workOrderId, Checklist checklist)
		{
			var inspection = _context.Inspections.SingleOrDefault(i => i.WorkOrderId == workOrderId);
			if (inspection == null)
			{
				inspection = new Inspection
				{
					WorkOrderId = workOrderId,
					Checklists = new List<Checklist> { checklist }
				};

				_context.Inspections.Add(inspection);
			}
			else if (inspection.Checklists.Any(c => c.Id == checklist.Id) == false)
			{
				inspection.Checklists.Add(checklist);
			}

			TrySave();

			return inspection;
		}

		/**
		 * Gets all inspectionitems from DB if they already exists, and creates them if they don't already 
		 */
		private Inspection GetOrCreateInspectionItems(Checklist checklist, Inspection inspection)
		{
			foreach (var ci in checklist.ChecklistItems)
			{
				var inspectionItem = inspection.InspectionItems.SingleOrDefault(item => item.ChecklistItem.Id == ci.Id);
				if (inspectionItem == null)
				{
					inspectionItem = new InspectionItem
					{
						ChecklistItem = ci,
						Inspection = inspection
					};

					inspection.InspectionItems.Add(inspectionItem);
					_context.InspectionItems.Add(inspectionItem);
				}
			}

			TrySave();

			return inspection;
		}

		/**
		 * Gets all inspectionmeasurements from DB if they already exist, and creates them if they don't already 
		 */
		private Inspection GetOrCreateInspectionMeasurements(Checklist checklist, Inspection inspection)
		{
			foreach (var item in inspection.InspectionItems)
			{
				var measurements = _context.Measurements.Where(m => m.ChecklistItem.Id == item.ChecklistItem.Id).ToList();

				foreach(var measurement in measurements)
				{
					if (item.InspectionMeasurements.Any(im => im.Measurement.Id == measurement.Id) == false)
					{
						var inspectionMeasurement = new InspectionMeasurement
						{
							InspectionItem = item,
							Measurement = measurement,
							Value = measurement.MinValue
						};
						_context.InspectionMeasurements.Add(inspectionMeasurement);
					}
				}
			}

			TrySave();

			return _context.Inspections.Single(i => i.Id == inspection.Id);
		}

		private ScrollableTabContainerViewModel GetScrollableTabContainerViewModel(Guid? tagId)
		{
			// Construct tabs based on current selection
			var applicableTags = _context.Tags
				.Where(t => t.IsVisibleToEmployee)
				.OrderBy(t => t.Name)
				.ToList();

			IList<ScrollableTab> tabs = new List<ScrollableTab>
			{
				new ScrollableTab {
					paneId = null,
					title = "All tags"
				}
			};

			foreach (Tag tag in applicableTags)
			{
				tabs.Add(new ScrollableTab { paneId = tag.Id, title = tag.Name });
			}

			if (tagId.HasValue)
			{
				// Make matching tag active
				tabs.Where(tab => tab.paneId == tagId).ElementAt(0).active = true;
			}
			else
			{
				// Make default (All tags) appear active
				tabs.ElementAt(0).active = true;
			}

			return new ScrollableTabContainerViewModel
			{
				Tabs = tabs
			};
		}

		private bool TrySave()
		{
			bool wasSuccessful = false;
			try
			{
				_context.SaveChanges();
				wasSuccessful = true;
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return wasSuccessful;
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

		private JsonResult BuildInspectionReportInternal(IEnumerable<InspectionItem> unfilteredInspectionItems, bool grouped, string workOrderId)
		{
			var applicableTags = _context.Tags
				.Where(t => t.IsVisibleToCustomer)
				.Select(t => t.Id)
				.ToList();

			// Only show inspection items which correspond to one or more customer visible tags
			var inspectionItems = unfilteredInspectionItems
				.Where(ii => ii.ChecklistItem.Tags
					.Select(t => t.Id)
					.Intersect(applicableTags)
					.Any()
				)
				// Only show inspection items which have had a marked condition
				.Where(ii => ii.Condition != RecommendedServiceSeverity.UNKNOWN);

			string baseUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

			if (grouped)
			{
				var inspectionReportGroups = inspectionItems
					.GroupBy(ii =>
						ii.ChecklistItem.Tags
							.Where(t => t.IsVisibleToCustomer)
							.Select(t => t.Name)
							.First()
					)
					.OrderBy(ig => ig.OrderBy(ii => ii.Condition).ToList().FirstOrDefault().Condition)
					.Select(ig => {
						var items = ig.OrderBy(ii => ii.Condition).ToList();
						return new
						{
							Name = ig.Key,
							items.FirstOrDefault().Condition,
							Items = items.Select(ii => BuildInspectionReportItem(ii, workOrderId, baseUrl))
						};
					});

				return Json(inspectionReportGroups, JsonRequestBehavior.AllowGet);
			}
			else
			{
				var inspectionReportItems = inspectionItems
					.OrderBy(ii => ii.Condition)
					.Select(ii => BuildInspectionReportItem(ii, workOrderId, baseUrl));

				return Json(inspectionReportItems, JsonRequestBehavior.AllowGet);
			}
		}

		private object BuildInspectionReportItem(InspectionItem ii, string workOrderId, string baseUrl)
		{
			return new
			{
				InspectionItemId = ii.Id,
				ii.Condition,
				ii.Note,
				ii.ChecklistItem.Name,
				CannedResponses = ii.CannedResponses.Select(cr => new { cr.Response, cr.Description, cr.Url }),
				Measurements = ii.InspectionMeasurements.Select(im => new { im.Value, im.Measurement.Label, im.Measurement.Unit }),
				Images = ii.InspectionImages
							.Select(image => new
							{
								title = ii.ChecklistItem.Name,
								altText = ii.ChecklistItem.Name,
								url = $"{baseUrl}/Uploads/Inspections/{workOrderId}/{ii.Id}/{image.Title}",
								extUrl = $"{baseUrl}/Uploads/Inspections/{workOrderId}/{ii.Id}/{image.Title}"
							})
			};
		}

		private WorkOrderResponse GetWorkOrderResponse(string workOrderId)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(CurrentUserClaims, workOrderId, GetCompanyNumber(), false);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return task.Result;
		}
	}
}
