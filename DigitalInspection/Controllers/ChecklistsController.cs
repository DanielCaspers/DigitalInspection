﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;

namespace DigitalInspection.Controllers
{
	public class ChecklistsController : BaseController
	{
		private static readonly string IMAGE_SUBDIRECTORY = "Checklists";

		public ChecklistsController()
		{
			_resource = "Checklist";
		}

		private ManageChecklistMasterViewModel GetChecklistViewModel()
		{
			var checklists = _context.Checklists;

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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult Index()
		{
			return PartialView(GetChecklistViewModel());
		}

		// GET: Checklists_ChecklistList partial and return it to _ChecklistList.cshtml
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult _ChecklistList()
		{
			return PartialView(GetChecklistViewModel());
		}

		//GET: Checklists/Edit/:id
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult Edit(Guid id)
		{
			var checklist = _context.Checklists.SingleOrDefault(c => c.Id == id);
			IList<ChecklistItem> checklistItems = _context.ChecklistItems.OrderBy(c => c.Name).ToList();
			IList<bool> isChecklistItemSelected = new List<bool>();
			
			foreach(var checklistItem in checklistItems)
			{
				isChecklistItemSelected.Add(checklist.ChecklistItems.Contains(checklistItem));
			}


			if (checklist == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
			}
			else
			{
				var viewModel = new EditChecklistViewModel {
					Checklist = checklist,
					ChecklistItems = checklistItems,
					IsChecklistItemSelected = isChecklistItemSelected
				};
				return PartialView("_EditChecklist", viewModel);
			}
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult Update(Guid id, EditChecklistViewModel vm)
		{
			var checklistInDb = _context.Checklists.SingleOrDefault(c => c.Id == id);
			if(checklistInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
			}
			else
			{
				checklistInDb.Name = vm.Checklist.Name;

				IList<ChecklistItem> selectedItems = new List<ChecklistItem>();
				// Using plain for loop for parallel array data reference
				for(int i=0; i < vm.IsChecklistItemSelected.Count(); i++)
				{
					if (vm.IsChecklistItemSelected[i])
					{
						Guid selectedItemId = vm.ChecklistItems[i].Id;
						selectedItems.Add(_context.ChecklistItems.Single(ci => ci.Id == selectedItemId));
					}
				}
				foreach(var item in checklistInDb.ChecklistItems)
				{
					_context.ChecklistItems.Attach(item);
				}
				checklistInDb.ChecklistItems = selectedItems;

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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
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
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var checklist = _context.Checklists.Find(id);

				if (checklist == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
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
