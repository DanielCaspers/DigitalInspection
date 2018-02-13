using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Collections.Generic;
using System.Data.Entity.Validation;

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
			var checklistItems = _context.ChecklistItems.OrderBy(c => c.Name).ToList();
			var tags = _context.Tags.OrderBy(t => t.Name).ToList();
			// TODO: Why is this necessary?
			var measurements = new List<Measurement>();
			return new ManageChecklistItemsViewModel
			{
				ChecklistItems = checklistItems,
				AddChecklistItemVM = new AddChecklistItemViewModel { Name = "", Tags = tags, Measurements = measurements }
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

		//GET: ChecklistItems/Edit/:id
		public PartialViewResult Edit(Guid id)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(c => c.Id == id);

			if (checklistItem == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}
			else
			{
				checklistItem.Measurements = checklistItem.Measurements.OrderBy(m => m.Label).ToList();
				checklistItem.CannedResponses = checklistItem.CannedResponses.OrderBy(c => c.Response).ToList();
				var tags = _context.Tags.OrderBy(t => t.Name).ToList();
				var selectedTagIds = checklistItem.Tags.Select(t => t.Id);
				var viewModel = new EditChecklistItemViewModel
				{
					ChecklistItem = checklistItem,
					Tags = tags,
					SelectedTagIds = selectedTagIds
				};
				return PartialView("_EditChecklistItem", viewModel);
			}
		}

		[HttpPost] //TODO: Move Tags parameter into viewModel??
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
			newItem.Tags.OrderBy(t => t.Name);
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

		[HttpPost]
		public ActionResult Update(Guid id, EditChecklistItemViewModel vm)
		{
			var checklistItemInDb = _context.ChecklistItems.SingleOrDefault(c => c.Id == id);
			if(checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}
			else
			{
				// Duplicating database entries bug - MUST BE DONE BEFORE PROP CHANGES 
				// http://stackoverflow.com/a/22389505/2831961
				foreach (var measurement in checklistItemInDb.Measurements)
				{
					_context.Measurements.Attach(measurement);
				}

				foreach (var cannedResponse in checklistItemInDb.CannedResponses)
				{
					_context.CannedResponses.Attach(cannedResponse);
				}

				foreach (var tag in checklistItemInDb.Tags)
				{
					_context.Tags.Attach(tag);
				}

				foreach (var measurementInVm in vm.ChecklistItem.Measurements)
				{
					var measurementInDb = checklistItemInDb.Measurements.SingleOrDefault(cm => cm.Id == measurementInVm.Id);
					if (measurementInDb != null)
					{
						measurementInDb.Label = measurementInVm.Label;
						measurementInDb.Unit = measurementInVm.Unit;
						measurementInDb.MinValue = measurementInVm.MinValue;
						measurementInDb.MaxValue = measurementInVm.MaxValue;
						measurementInDb.StepSize = measurementInVm.StepSize;
					}
				}

				foreach (var cannedResponseInVm in vm.ChecklistItem.CannedResponses)
				{
					var cannedResponseInDb = checklistItemInDb.CannedResponses.SingleOrDefault(cr => cr.Id == cannedResponseInVm.Id);
					if (cannedResponseInDb != null)
					{
						cannedResponseInDb.Response = cannedResponseInVm.Response;
						cannedResponseInDb.LevelsOfConcern = cannedResponseInVm.LevelsOfConcern;
						cannedResponseInDb.Url = cannedResponseInVm.Url;
						cannedResponseInDb.Description = cannedResponseInVm.Description;
					}
				}

				checklistItemInDb.Name = vm.ChecklistItem.Name;
				checklistItemInDb.Tags = _context.Tags.Where(t => vm.SelectedTagIds.Contains(t.Id)).ToList();

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

		// POST: ChecklistItems/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var checklistItemInDb = _context.ChecklistItems.Find(id);

				if (checklistItemInDb == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
				}

				// TODO: DJC Should cascade delete work from checklistitem to measurement and canned response
				foreach (var measurement in _context.Measurements)
				{
					if (measurement.ChecklistItemId == id)
					{
						_context.Measurements.Remove(measurement);
					}
				}

				// TODO: DJC Should cascade delete work from checklistitem to measurement and canned response
				foreach (var cannedResponse in _context.CannedResponses)
				{
					if (cannedResponse.ChecklistItemId == id)
					{
						_context.CannedResponses.Remove(cannedResponse);
					}
				}

				_context.ChecklistItems.Remove(checklistItemInDb);
				_context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred(dbEx));
			}
			catch (Exception e)
			{
				return PartialView("Toasts/_Toast", ToastService.DatabaseException(e));
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
