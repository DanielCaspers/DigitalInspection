﻿using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Models.Web;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using DigitalInspection.Models.Orders;
using System.IO;
using DigitalInspection.Models.Inspections.Reports;
using DigitalInspection.Services.Core;
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

			return BuildInspectionReportInternal(inspectionItems, grouped);
		}

		[AllowAnonymous]
		public JsonResult Report(Guid inspectionId, bool grouped = false)
		{
			var inspectionItems = _context.Inspections
				.Single(i => i.Id == inspectionId)
				.InspectionItems;

			return BuildInspectionReportInternal(inspectionItems, grouped);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Delete(string workOrderId)
		{
			if (Request.Headers["DigitalInspection-AppKey"] !=
			    ConfigurationManager.AppSettings.Get("DigitalInspection-AppKey"))
			{
				return Json($"Not authorized to delete inspection for {workOrderId}");
			}

			var inspection = _context.Inspections.SingleOrDefault(i => i.WorkOrderId == workOrderId);
			if (inspection == null)
			{
				return new EmptyResult();
			}

			InspectionService.DeleteInspection(_context, inspection);

			return new EmptyResult();
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

			if (InspectionService.UpdateInspectionItemCondition(_context, inspectionItemInDb, inspectionItemCondition) == false)
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}

			// Prepare updated multiselect list for client
			var checklistItem = _context.ChecklistItems.Single(ci => ci.Id == inspectionItemInDb.ChecklistItem.Id);
			var filteredCRs = checklistItem.CannedResponses.Where(cr => cr.LevelsOfConcern.Contains(inspectionItemCondition));

			// Options may be selected in the case where we haven't changed to a new condition
			var options = filteredCRs.Select(cr => new
			{
				label = cr.Response,
				title = cr.Response,
				value = cr.Id,
				selected = inspectionItemInDb.CannedResponses.Any(c => c.Id == cr.Id)
			});

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
			var inspectionItemInDb = _context.InspectionItems.Single(item => item.Id == inspectionItemId);

			IList<Guid> selectedCannedResponseIds = vm.Inspection.InspectionItems.Single(ii => ii.Id == inspectionItemId).SelectedCannedResponseIds;

			InspectionService.UpdateInspectionItemCannedResponses(_context, inspectionItemInDb, selectedCannedResponseIds);
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

			if (InspectionService.UpdateInspectionItemNote(_context, inspectionItemInDb, itemNoteVm.Note) == false)
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

			return PartialView("Toasts/_Toast", ToastService.WorkOrderError(task.Result));
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

			if (InspectionService.UpdateInspectionItemMeasurements(_context, inspectionItemInDb, MeasurementsVM.InspectionItem.InspectionMeasurements))
			{
				return new EmptyResult();
			}

			return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
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

			if (InspectionService.AddInspectionItemImage(_context, inspectionItemInDb, imageDto))
			{
				return RedirectToAction("Index", new { workOrderId = photoVM.WorkOrderId, checklistId = photoVM.ChecklistId, tagId = photoVM.TagId });
			}

			return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetAddMeasurementDialog(Guid inspectionItemId, Guid checklistItemId)
		{
			var checklistItem = _context.ChecklistItems.Single(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.Single(item => item.Id == inspectionItemId);
			inspectionItem.InspectionMeasurements = inspectionItem.InspectionMeasurements.OrderBy(im => im.Measurement.Label).ToList();

			return PartialView("_AddMeasurementDialog", new AddMeasurementViewModel
			{
				ChecklistItem = checklistItem,
				InspectionItem = inspectionItem,
				Measurements = checklistItem.Measurements.OrderBy(m => m.Label).ToList()
			});
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetAddInspectionWorkOrderNoteDialog(string workOrderId)
		{
			var workOrderResponse = GetWorkOrderResponse(workOrderId);
			var workOrder = workOrderResponse.Entity;

			var combinedNote = string.Join(Environment.NewLine, workOrder.Notes);

			return PartialView("../Shared/Dialogs/_AddInspectionWorkOrderNoteDialog", new AddInspectionWorkOrderNoteViewModel
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
			var inspectionItem = _context.InspectionItems.Single(item => item.Id == inspectionItemId);

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
			var inspectionItem = _context.InspectionItems.Single(item => item.Id == inspectionItemId);

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
			var workOrder = workOrderResponse.Entity;
			ToastViewModel toast = null;

			var checklist = _context.Checklists.Single(c => c.Id == checklistId);

			if (workOrderResponse.IsSuccessStatusCode == false)
			{
				toast = ToastService.WorkOrderError(workOrderResponse);
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

			var inspection = InspectionService.GetOrCreateInspection(_context, workOrderId, checklist);

			// Filter inspection items
			Func<InspectionItem, bool> filterByChecklistsAndTags = ii => ii.ChecklistItem.Checklists.Any(c => c.Id == checklistId) && ii.ChecklistItem.Tags.Any(t => t.Id == tagId);
			Func<InspectionItem, bool> filterByChecklists = ii => ii.ChecklistItem.Checklists.Any(c => c.Id == checklistId);

			inspection.InspectionItems = inspection.InspectionItems
				.Where(tagId.HasValue ? filterByChecklistsAndTags : filterByChecklists)
				.OrderBy(ii => ii.ChecklistItem.Name)
				.ToList();

			foreach(var inspectionItem in inspection.InspectionItems)
			{
				inspectionItem.SelectedCannedResponseIds = inspectionItem.CannedResponses.Select(cr => cr.Id).ToList();
			}

			return PartialView(new InspectionDetailViewModel
			{
				WorkOrder = workOrder,
				Checklist = checklist,
				Inspection = inspection,
				Toast = toast,
				AddMeasurementVM = new AddMeasurementViewModel { },
				AddInspectionWorkOrderNoteVm = new AddInspectionWorkOrderNoteViewModel { },
				AddInspectionItemNoteVm = new AddInspectionItemNoteViewModel { },
				UploadInspectionPhotosVM = new UploadInspectionPhotosViewModel {
					WorkOrderId = workOrderId
				},
				ViewInspectionPhotosVM = new ViewInspectionPhotosViewModel { },
				VehicleHistoryVM = new VehicleHistoryViewModel() { },
				ScrollableTabContainerVM = GetScrollableTabContainerViewModel(tagId),
				FilteringTagId = tagId
			});
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

		private JsonResult BuildInspectionReportInternal(IEnumerable<InspectionItem> unfilteredInspectionItems, bool grouped)
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
					.OrderBy(ig => ig.OrderBy(ii => ii.Condition).First().Condition)
					.Select(ig => new InspectionReportGroup(ig, baseUrl));

				return Json(inspectionReportGroups, JsonRequestBehavior.AllowGet);
			}
			else
			{
				var inspectionReportItems = inspectionItems
					.OrderBy(ii => ii.Condition)
					.Select(ii => new InspectionReportItem(ii, baseUrl));

				return Json(inspectionReportItems, JsonRequestBehavior.AllowGet);
			}
		}

		private HttpResponse<WorkOrder> GetWorkOrderResponse(string workOrderId)
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
