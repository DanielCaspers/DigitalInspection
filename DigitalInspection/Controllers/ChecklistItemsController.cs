using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Services;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Services.Web;
using DigitalInspection.ViewModels.ChecklistItems;

namespace DigitalInspection.Controllers
{
	[AuthorizeRoles(Roles.Admin)]
	public class ChecklistItemsController : BaseController
	{
		public ChecklistItemsController()
		{
			ResourceName = "Checklist item";
		}

		private ManageChecklistItemsViewModel GetChecklistItemViewModel()
		{
			var tagsTask = Task.Run(async () => {
				return await TagService.GetTags();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			tagsTask.Wait();

			var checklistItemsTask = Task.Run(async () => {
				return await ChecklistItemService.GetChecklistItems();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			tagsTask.Wait();

			var checklistItems = checklistItemsTask.Result.Entity.ToList();
			var tags = tagsTask.Result.Entity.ToList();
			return new ManageChecklistItemsViewModel
			{
				ChecklistItems = checklistItems,
				AddChecklistItemVM = new AddChecklistItemViewModel { Name = "", Tags = tags }
			};
		}

		// GET: Checklist items page and return response to index.cshtml
		public PartialViewResult Index()
		{
			return PartialView(GetChecklistItemViewModel());
		}

		// GET: _ChecklistItemList partial and return it to _ChecklistItemList.cshtml
		public PartialViewResult _ChecklistItemList()
		{
			return PartialView(GetChecklistItemViewModel());
		}

		public ViewResult Report()
		{
			return View("Report", GetChecklistItemViewModel());
		}

		//GET: ChecklistItems/Edit/:id
		public PartialViewResult Edit(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.GetEdit(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var viewModel = task.Result.Entity;

			return viewModel == null ?
				PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName)) :
				PartialView("_EditChecklistItem", viewModel);
		}

		// TODO DJC FINISH converting this endpoint
		[HttpPost]
		public ActionResult Create(AddChecklistItemViewModel checklistItem, IList<Guid> tags)
		{
			ChecklistItem newItem = new ChecklistItem
			{
				Name = checklistItem.Name,
				Tags = new List<Tag>(),
				CannedResponses = new List<CannedResponse>(),
				Measurements = new List<Measurement>()
			};

			// TODO: Figure out how to do this with LINQ
			// Push each full tag object onto list
			foreach (var tagId in tags)
			{
				var tag = _context.Tags.Find(tagId);
				newItem.Tags.Add(tag);
			}

			_context.ChecklistItems.Add(newItem);

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return RedirectToAction("_ChecklistItemList");
		}

		// TODO DJC: Verify model binding from here to NET Core. Getting HTTP 500 with duplicate for primary key error
		[HttpPost]
		public ActionResult Update(Guid id, EditChecklistItemViewModel vm)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.UpdateChecklistItem(id, vm);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var wasSuccessful = task.Result.IsSuccessStatusCode;
			if (wasSuccessful == false)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			return RedirectToAction("Edit", new { id });
		}

		// TODO DJC: Noticed checklist items count is broken in checklists view, because join tags are not present. Consider porting new join models to old client
		// TODO DJC: Verify model binding from here to NET Core. Getting HTTP 500 with cannot update due to foreign key constraint error
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.DeleteChecklistItem(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var wasSuccessful = task.Result.IsSuccessStatusCode;
			if (wasSuccessful == false)
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred(new Exception()));
			}

			return RedirectToAction("_ChecklistItemList");
		}

		[HttpPost]
		public ActionResult AddMeasurement(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			checklistItemInDb.Measurements.Add(new Measurement());

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return RedirectToAction("Edit", new { id = checklistItemInDb.Id });
		}

		[HttpPost]
		public ActionResult DeleteMeasurement(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.FirstOrDefault(ci => ci.Measurements.Any(m => m.Id == id));

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			var measurementToRemove = checklistItemInDb.Measurements.Single(m => m.Id == id);
			checklistItemInDb.Measurements.Remove(measurementToRemove);

			// Uncomment this line if it is desired to remove all notions of this measurement from the APP.
			// Leaving this commented out only removes its association from a checklist for new items, but allows
			// old inspections to remain historically accurate. 
			//_context.Measurements.Remove(measurementToRemove);
			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return RedirectToAction("Edit", new { id = checklistItemInDb.Id });
		}

		[HttpPost]
		public ActionResult AddCannedResponse(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			var cannedResponse = new CannedResponse()
			{
				Response = "A new response"
			};
			checklistItemInDb.CannedResponses.Add(cannedResponse);

			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return RedirectToAction("Edit", new { id = checklistItemInDb.Id });
		}

		[HttpPost]
		public ActionResult DeleteCannedResponse(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.FirstOrDefault(ci => ci.CannedResponses.Any(m => m.Id == id));

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			var cannedResponseToRemove = checklistItemInDb.CannedResponses.Single(m => m.Id == id);
			checklistItemInDb.CannedResponses.Remove(cannedResponseToRemove);

			// Uncomment this line if it is desired to remove all notions of this canned response from the APP.
			// Leaving this commented out only removes its association from a checklist for new items, but allows
			// old inspections to remain historically accurate. 
			//_context.CannedResponses.Remove(cannedResponseToRemove);
			try
			{
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return RedirectToAction("Edit", new { id = checklistItemInDb.Id });
		}
	}
}
