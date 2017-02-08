using System;
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


		// GET: Checklist
		public ActionResult Index()
		{

			var checklists = _context.Checklists;
			
			//Temp append to not mess with the db
			foreach(var checklist in checklists)
			{
				checklist.Items = Items;
			}

			var viewModel = new ManageChecklistMasterViewModel
			{
				Resource = "Checklists",
				Checklists = checklists.ToList()
			};

			return View(viewModel);
		}

		// GET: Checklist/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: Checklist/Create
		public ActionResult Create()
		{
			return PartialView("Toasts/_ErrorToast");
		}

		// POST: Checklist/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Checklist/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: Checklist/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add update logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// POST: Checklist/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var checklist = _context.Checklists.Find(id);

				if (checklist == null)
					return HttpNotFound();

				_context.Checklists.Remove(checklist);
				_context.SaveChanges();
			}
			catch
			{
				return PartialView("Toasts/_ErrorToast");
			}

			return RedirectToAction("Index");
		}
	}
}
