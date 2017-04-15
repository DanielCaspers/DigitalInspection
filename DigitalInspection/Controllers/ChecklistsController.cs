using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;

namespace DigitalInspection.Controllers
{
	public class ChecklistsController : Controller
	{
		private static readonly string RESOURCE = "Checklist";
		private static readonly string IMAGE_SUBDIRECTORY = "Checklists";

		//TODO Remove this temp code once checklist items can be added onto checklist
		private List<ChecklistItem> Items
		{
			get
			{
				return new List<ChecklistItem>
				{
					new ChecklistItem
					{
						Name = "Front Brakes"
					},
					new ChecklistItem
					{
						Name = "Rear Brakes"
					}
				};
			}

			set { Items = value; }
		}

		private ApplicationDbContext _context;


		public ChecklistsController()
		{
			_context = new ApplicationDbContext();
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
		}

		private ManageChecklistMasterViewModel GetChecklistViewModel()
		{
			var checklists = _context.Checklists;

			//Temp append to not mess with the db
			foreach (var checklist in checklists)
			{
				checklist.Items = Items;
			}

			return new ManageChecklistMasterViewModel
			{
				Checklists = checklists.OrderBy(c => c.Name).ToList(),
				AddChecklistVM = new AddChecklistViewModel
				{
					Name = "",
					Picture = null
				}
			};
		}

		// GET: Checklists page and return response to index.cshtml
		public PartialViewResult Index()
		{
			return PartialView(GetChecklistViewModel());
		}

		// GET: Checklists_ChecklistList partial and return it to _ChecklistList.cshtml 
		public PartialViewResult _ChecklistList()
		{
			return PartialView(GetChecklistViewModel());
		}

		//GET: Checklists/Edit/:id
		public PartialViewResult Edit(Guid id)
		{
			var checklist = _context.Checklists.SingleOrDefault(c => c.Id == id);

			if( checklist == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
			}
			else
			{
				var viewModel = new EditChecklistViewModel {
					Checklist = checklist
				};
				return PartialView("_EditChecklist", viewModel);
			}
		}

		[HttpPost]
		public ActionResult Update(Guid id, Checklist checklist)
		{
			var checklistInDb = _context.Checklists.SingleOrDefault(c => c.Id == id);
			if(checklistInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
			}
			else
			{
				checklistInDb.Name = checklist.Name;

				HttpPostedFileBase picture = Request.Files[0];

				// Only update the picture if a new one was uploaded
				if(picture != null && picture.ContentLength > 0)
				{
					ImageService.DeleteImage(checklistInDb.Image, IMAGE_SUBDIRECTORY);
					checklistInDb.Image = ImageService.SaveImage(picture, IMAGE_SUBDIRECTORY, id.ToString());
				}

				_context.SaveChanges();
				return RedirectToAction("Edit", new { id = checklistInDb.Id });
			}
		}

		[HttpPost]
		public ActionResult Create(AddChecklistViewModel list)
		{
			Checklist newList = new Checklist
			{
				Name = list.Name,
				Id = Guid.NewGuid()
			};

			newList.Image = ImageService.SaveImage(list.Picture, IMAGE_SUBDIRECTORY, newList.Id.ToString());

			_context.Checklists.Add(newList);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// POST: Checklist/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var checklist = _context.Checklists.Find(id);

				if (checklist == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
				}

				ImageService.DeleteImage(checklist.Image, IMAGE_SUBDIRECTORY);

				_context.Checklists.Remove(checklist);
				_context.SaveChanges();
			}
			catch
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}
			return RedirectToAction("_ChecklistList");
		}

	}
}
