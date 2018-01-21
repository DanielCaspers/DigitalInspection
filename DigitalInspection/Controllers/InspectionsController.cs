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
			_resource = "Inspection";
		}

		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public PartialViewResult Index(string workOrderId, Guid checklistId, Guid? tagId)
		{
			return GetInspectionViewModel(workOrderId, checklistId, tagId);
		}

		[AllowAnonymous]
		public JsonResult Report(string workOrderId, bool grouped = false)
		{
			var applicableTags = _context.Tags
				.Where(t => t.IsVisibleToCustomer)
				.Select(t => t.Id)
				.ToList();

			var inspectionItems = _context.Inspections
				.Where(i => i.WorkOrderId == workOrderId)
				.SingleOrDefault()
				.InspectionItems
				// Only show inspection items which correspond to one or more customer visible tags
				.Where(ii => ii.ChecklistItem.Tags
					.Select(t => t.Id)
					.Intersect(applicableTags)
					.Any()
				)
				// Only show inspection items which have had a marked condition
				.Where(ii => ii.Condition != RecommendedServiceSeverity.UNKNOWN);

			string BASE_URL = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

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
								Items = items.Select(ii => BuildInspectionReportItem(ii, workOrderId, BASE_URL))
							};
						});

				return Json(inspectionReportGroups, JsonRequestBehavior.AllowGet);
			}
			else
			{
				var inspectionReportItems = inspectionItems
					.OrderBy(ii => ii.Condition)
					.Select(ii => BuildInspectionReportItem(ii, workOrderId, BASE_URL));

				return Json(inspectionReportItems, JsonRequestBehavior.AllowGet);
			}
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

			if (TrySave() == false)
			{
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
		[Authorize(Roles = AuthorizationRoles.ADMIN + "," + AuthorizationRoles.USER)]
		public ActionResult Note(AddInspectionNoteViewModel NoteVM)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == NoteVM.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_subresource));
			}
			inspectionItemInDb.Note = NoteVM.Note;

			if (TrySave() == false)
			{
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

			if (TrySave() == false) {
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

			return RedirectToAction("Index", new { workOrderId = photoVM.WorkOrderId, checklistId = photoVM.ChecklistId, tagId = photoVM.TagId });
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

		private PartialViewResult GetInspectionViewModel(string workOrderId, Guid checklistId, Guid? tagId)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(workOrderId, false);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var workOrder = task.Result.WorkOrder;
			ToastViewModel toast = null;

			var checklist = _context.Checklists.SingleOrDefault(c => c.Id == checklistId);

			if (task.Result.IsSuccessStatusCode == false)
			{
				toast = DisplayErrorToast(task.Result);
			}
			else if (checklist == null)
			{
				toast = ToastService.ResourceNotFound(_resource, ToastActionType.NavigateBack);
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
				AddInspectionNoteVM = new AddInspectionNoteViewModel { },
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
					return ToastService.ResourceNotFound(_resource, ToastActionType.NavigateBack);
				case (HttpStatusCode)423:
					return ToastService.FileLockedByAnotherClient(response.ErrorMessage, ToastActionType.Refresh);
				case (HttpStatusCode)428:
					return ToastService.FileLockRequired();
				default:
					return ToastService.UnknownErrorOccurred(response.HTTPCode, response.ErrorMessage);
			}
		}

		private object BuildInspectionReportItem(InspectionItem ii, string workOrderId, string BASE_URL)
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
								// url = image.ImageUrl,
								title = ii.ChecklistItem.Name,
								altText = ii.ChecklistItem.Name,
								url = $"{BASE_URL}/Uploads/Inspections/{workOrderId}/{ii.Id}/{image.Title}",
								extUrl = $"{BASE_URL}/Uploads/Inspections/{workOrderId}/{ii.Id}/{image.Title}"
							})
			};
		}
	}
}
