using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Services;
using DigitalInspection.Services.Web;
using DigitalInspection.ViewModels.Tags;

namespace DigitalInspection.Controllers
{
	[AuthorizeRoles(Roles.Admin)]
	public class TagsController : BaseController
	{
		public TagsController()
		{
			ResourceName = "Tag";
		}

		private ManageTagsViewModel GetTagViewModel()
		{
			var task = Task.Run(async () => {
				return await TagService.GetTags();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return new ManageTagsViewModel
			{
				Tags = task.Result.Entity.ToList(),
				AddTagVM = new AddTagViewModel { Name = "" }
			};
		}

		// GET: Tags page and return response to index.cshtml
		public PartialViewResult Index()
		{
			return PartialView(GetTagViewModel());
		}

		// GET: _TagList partial and return it to _TagList.cshtml 
		public PartialViewResult _TagList()
		{
			return PartialView(GetTagViewModel());
		}

		//GET: Tags/Edit/:id
		public PartialViewResult Edit(Guid id)
		{
			var task = Task.Run(async () => {
				return await TagService.GetTag(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var tag = task.Result.Entity;

			if (tag == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			var viewModel = new EditTagViewModel
			{
				Tag = tag
			};
			return PartialView("_EditTag", viewModel);
		}

		[HttpPost]
		public ActionResult Update(Guid id, AddTagViewModel tag)
		{
			var task = Task.Run(async () => {
				return await TagService.UpdateTag(id, tag);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var wasSuccessful = task.Result.IsSuccessStatusCode;

			if (wasSuccessful == false)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			return RedirectToAction("Edit", new { id });
		}

		[HttpPost]
		public ActionResult Create(AddTagViewModel tag)
		{
			var task = Task.Run(async () => {
				return await TagService.CreateTag(tag);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return RedirectToAction("_TagList");
		}

		// POST: Tags/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			var task = Task.Run(async () => {
				return await TagService.DeleteTag(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return RedirectToAction("_TagList");
		}

	}
}
