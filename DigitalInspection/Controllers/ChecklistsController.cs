﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;

namespace DigitalInspection.Controllers
{
	public class ChecklistsController : Controller
	{
		//TODO temp code
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

		private List<Checklist> List
		{
			get
			{
				return new List<Checklist>
				{
					new Checklist
					{
						Name = "Mechanics",
						Items = Items
					},
					new Checklist
					{
						Name = "X-lube",
						Items = new List<ChecklistItem>{}
					}
				};
			}
			set { List = value;  }
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
				Resource = "Checklists",
				Checklists = checklists.ToList(),
				AddChecklistVM = new AddChecklistViewModel
				{
					Name = "",
					Picture = null
				}
			};
		}

		// GET: Checklists page and return response to index.cshtml
		public ActionResult Index()
		{
			var viewModel = GetChecklistViewModel();
			return PartialView(viewModel);
		}

		// GET: Checklists_ChecklistList partial and return it to _ChecklistList.cshtml 
		public ActionResult _ChecklistList()
		{
			var viewModel = GetChecklistViewModel();
			return PartialView(viewModel);
		}

		[HttpPost]
		public ActionResult Create(AddChecklistViewModel list)
		{
			Checklist newList = new Checklist
			{
				Name = list.Name,
				Id = Guid.NewGuid()
			};

			_context.Checklists.Add(newList);
			_context.SaveChanges();

			return RedirectToAction("_ChecklistList");
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
					return PartialView("Toasts/_Toast", new ToastViewModel
					{
						Icon = "error",
						Message = "Checklist could not be found.",
						Type = ToastType.Error,
						Action = ToastActionType.Refresh
					});
					//return HttpNotFound();
				}

				_context.Checklists.Remove(checklist);
				_context.SaveChanges();
			}
			catch
			{
				return PartialView("Toasts/_Toast", new ToastViewModel
				{
					Icon = "error",
					Message = "An unknown error occurred.",
					Type = ToastType.Error,
					Action = ToastActionType.Refresh
				});
			}

			return RedirectToAction("_ChecklistList");
		}
	}
}
