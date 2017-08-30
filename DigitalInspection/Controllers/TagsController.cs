using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;

namespace DigitalInspection.Controllers
{
	public class TagsController : BaseController
	{
		public TagsController()
		{
			_resource = "Tag";
		}

		private ManageTagsViewModel GetTagViewModel()
		{
			var tags = _context.Tags;
			return new ManageTagsViewModel
			{
				Tags = tags.OrderBy(tag => tag.Name).ToList(),
				AddTagVM = new AddTagViewModel { Name = "" }
			};
		}

		// GET: Tags page and return response to index.cshtml
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult Index()
		{
			return PartialView(GetTagViewModel());
		}

		// GET: _TagList partial and return it to _TagList.cshtml 
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult _TagList()
		{
			return PartialView(GetTagViewModel());
		}

		//GET: Tags/Edit/:id
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public PartialViewResult Edit(Guid id)
		{
			var tag = _context.Tags.SingleOrDefault(t => t.Id == id);

			if (tag == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
			}
			else
			{
				var viewModel = new EditTagViewModel
				{
					Tag = tag
				};
				return PartialView("_EditTag", viewModel);
			}
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult Update(Guid id, AddTagViewModel tag)
		{
			var tagInDb = _context.Tags.SingleOrDefault(t => t.Id == id);
			if(tagInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
			}
			else
			{
				tagInDb.Name = tag.Name;

				_context.SaveChanges();
				return RedirectToAction("Edit", new { id = tagInDb.Id });
			}
		}

		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
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

		// POST: Tags/Delete/5
		[HttpPost]
		[Authorize(Roles = AuthorizationRoles.ADMIN)]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var tagInDb = _context.Tags.Find(id);

				if (tagInDb == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(_resource));
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
