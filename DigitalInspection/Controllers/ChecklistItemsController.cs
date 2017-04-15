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
	public class ChecklistItemsController : Controller
	{
		private static readonly string RESOURCE = "Checklist item";

		private ApplicationDbContext _context;


		public ChecklistItemsController()
		{
			_context = new ApplicationDbContext();
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
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
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
			}
			else
			{
				var viewModel = new EditChecklistItemViewModel
				{
					ChecklistItem = checklistItem
				};
				return PartialView("_EditChecklistItem", viewModel);
			}
		}

		[HttpPost]
		public ActionResult Create(AddChecklistItemViewModel checklistItem)
		{
			ChecklistItem newItem = new ChecklistItem
			{
				Name = checklistItem.Name,
				Id = Guid.NewGuid(),
				Measurements = new List<Measurement>()
			};

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
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
			}
			else
			{
				checklistItemInDb.Name = vm.ChecklistItem.Name;
				checklistItemInDb.Measurements = vm.ChecklistItem.Measurements;

				try
				{
					// Duplicating database entries bug
					// http://stackoverflow.com/a/22389505/2831961
					//foreach (var measurement in vm.ChecklistItem.Measurements)
					//{
					//	_context.Measurements.Attach(measurement);
					//}

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
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
				}

				_context.ChecklistItems.Remove(checklistItemInDb);
				_context.SaveChanges();
			}
			catch
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}
			return RedirectToAction("_ChecklistItemList");
		}

		[HttpPost]
		public ActionResult AddMeasurement(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
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
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
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
	}
}
