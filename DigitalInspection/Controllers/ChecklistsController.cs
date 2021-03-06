﻿using DigitalInspection.Models;
using DigitalInspection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Services.Core;
using DigitalInspection.ViewModels.Checklists;

namespace DigitalInspection.Controllers
{
	[AuthorizeRoles(Roles.Admin)]
	public class ChecklistsController : BaseController
	{
		private static readonly string IMAGE_SUBDIRECTORY = "Checklists";

		public ChecklistsController()
		{
			ResourceName = "Checklist";
		}

		private ManageChecklistMasterViewModel GetChecklistViewModel()
		{
			var checklists = _context.Checklists;

			return new ManageChecklistMasterViewModel
			{
				Checklists = checklists.OrderBy(c => c.Name).ToList(),
				AddChecklistVM = new AddChecklistViewModel
				{
					Name = ""
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
			IList<ChecklistItem> checklistItems = _context.ChecklistItems.OrderBy(c => c.Name).ToList();
			IList<bool> isChecklistItemSelected = new List<bool>();
			
			foreach(var checklistItem in checklistItems)
			{
				isChecklistItemSelected.Add(checklist.ChecklistItems.Contains(checklistItem));
			}


			if (checklist == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
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
		public ActionResult Update(Guid id, EditChecklistViewModel vm)
		{
			var checklistInDb = _context.Checklists.SingleOrDefault(c => c.Id == id);
			if(checklistInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}
			else
			{
				checklistInDb.Name = vm.Checklist.Name;

				IList<ChecklistItem> selectedItems = new List<ChecklistItem>();
				// Using plain for loop for parallel array data reference
				for(var i = 0; i < vm.IsChecklistItemSelected.Count; i++)
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

			// TODO DJC Remove fully once .NET core migration is complete.
			// Cannot remove now due to difficulties with EF ORM migration steps getting
			// stuck applying the migration. It seems its internal representation of 
			// SQL steps to perform, though it makes correct migration assets. 
			newList.Image = new Image { };

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
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
				}

				_context.Checklists.Remove(checklist);
				_context.SaveChanges();
			}
			catch (Exception e)
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred(e));
			}
			return RedirectToAction("_ChecklistList");
		}

	}
}
