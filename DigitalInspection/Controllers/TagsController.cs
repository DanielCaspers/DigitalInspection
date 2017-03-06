﻿using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;

namespace DigitalInspection.Controllers
{
	public class TagsController : Controller
	{
		private static readonly string RESOURCE = "Tag";

		private ApplicationDbContext _context;


		public TagsController()
		{
			_context = new ApplicationDbContext();
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
		}

		private ManageTagsViewModel GetTagViewModel()
		{
			var tags = _context.Tags;
			return new ManageTagsViewModel
			{
				Resource = "Checklists",
				Tags = tags.ToList(),
				AddTagVM = new AddTagViewModel { Name = "" }
			};
		}

		// GET: Tags page and return response to index.cshtml
		public PartialViewResult Index()
		{
			return PartialView(GetTagViewModel());
		}

		// GET: Checklists_TagList partial and return it to _TagList.cshtml 
		public PartialViewResult _TagList()
		{
			return PartialView(GetTagViewModel());
		}

		[HttpPost]
		public ActionResult Update(Guid id, Tag tag)
		{
			var tagInDb = _context.Tags.SingleOrDefault(t => t.Id == id);
			if(tagInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
			}
			else
			{
				tagInDb.Name = tag.Name;

				_context.SaveChanges();
				return RedirectToAction("_TagList");
			}
		}

		[HttpPost]
		public ActionResult Create(AddTagViewModel tag)
		{
			Tag newTag = new Tag
			{
				Name = tag.Name,
				Id = Guid.NewGuid()
			};

			_context.Tags.Add(newTag);
			_context.SaveChanges();

			return RedirectToAction("_TagList");
		}

		// POST: Checklist/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var tagInDb = _context.Tags.Find(id);

				if (tagInDb == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(RESOURCE));
				}

				_context.Tags.Remove(tagInDb);
				_context.SaveChanges();
			}
			catch
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred());
			}
			return RedirectToAction("_TagList");
		}

	}
}
