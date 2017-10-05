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
	public class ChecklistItemsController : BaseController
	{
		public ChecklistItemsController()
		{
			_resource = "Checklist item";
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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult Index()
		{
			return PartialView(GetChecklistItemViewModel());
		}

		// GET: _ChecklistItemList partial and return it to _ChecklistItemList.cshtml
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult _ChecklistItemList()
		{
			return PartialView(GetChecklistItemViewModel());
		}

		//GET: ChecklistItems/Edit/:id
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult Edit(Guid id)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(c => c.Id == id);

			if (checklistItem == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult Update(Guid id, EditChecklistItemViewModel vm)
		{
			var checklistItemInDb = _context.ChecklistItems.SingleOrDefault(c => c.Id == id);
			if(checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
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

				IList<Tag> selectedTagsInDb = _context.Tags.Where(t => vm.SelectedTagIds.Contains(t.Id)).ToList();

				checklistItemInDb.Name = vm.ChecklistItem.Name;
				checklistItemInDb.Measurements = vm.ChecklistItem.Measurements;
				checklistItemInDb.CannedResponses = vm.ChecklistItem.CannedResponses;

				checklistItemInDb.Tags = selectedTagsInDb;

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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var checklistItemInDb = _context.ChecklistItems.Find(id);

				if (checklistItemInDb == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
				}

				// TODO: DJC Should cascade delete work from checklistitem to measurement and canned response
				foreach(var measurement in _context.Measurements)
				{
					if(measurement.ChecklistItemId == id)
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
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}
			return RedirectToAction("_ChecklistItemList");
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult AddMeasurement(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult DeleteMeasurement(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.FirstOrDefault(ci => ci.Measurements.Any(m => m.Id == id));

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
			}

			var measurementToRemove = checklistItemInDb.Measurements.Single(m => m.Id == id);
			checklistItemInDb.Measurements.Remove(measurementToRemove);

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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult AddCannedResponse(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult DeleteCannedResponse(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.FirstOrDefault(ci => ci.CannedResponses.Any(m => m.Id == id));

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
			}

			var cannedResponseToRemove = checklistItemInDb.CannedResponses.Single(m => m.Id == id);
			checklistItemInDb.CannedResponses.Remove(cannedResponseToRemove);
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
