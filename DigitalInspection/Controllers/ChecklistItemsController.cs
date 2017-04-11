using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;

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
			return new ManageChecklistItemsViewModel
			{
				Resource = "Checklists",
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
		public ActionResult Update(Guid id, AddChecklistItemViewModel checklistItem)
		{
			var checklistItemInDb = _context.ChecklistItems.SingleOrDefault(c => c.Id == id);
			if(checklistItemInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
			}
			else
			{
				checklistItemInDb.Name = checklistItem.Name;

				_context.SaveChanges();
				return RedirectToAction("Edit", new { id = checklistItemInDb.Id });
			}
		}

		[HttpPost]
		public ActionResult Create(AddChecklistItemViewModel checklistItem)
		{
			ChecklistItem newItem = new ChecklistItem
			{
				Name = checklistItem.Name,
				Id = Guid.NewGuid()
			};

			_context.ChecklistItems.Add(newItem);
			_context.SaveChanges();

			return RedirectToAction("_ChecklistItemList");
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

	}
}
