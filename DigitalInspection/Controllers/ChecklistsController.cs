using DigitalInspection.Models;
using DigitalInspection.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DigitalInspection.Services.Web;
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
			var task = Task.Run(async () => {
				return await ChecklistHttpService.GetChecklists();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return new ManageChecklistMasterViewModel
			{
				Checklists = task.Result.Entity.ToList(),
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
			var task = Task.Run(async () => {
				return await ChecklistHttpService.GetEdit(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var viewModel = task.Result.Entity;

			return viewModel == null ? 
				PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName)) :
				PartialView("_EditChecklist", viewModel);
		}

		[HttpPost]
		public ActionResult Update(Guid id, EditChecklistViewModel vm)
		{
			var task = Task.Run(async () => {
				return await ChecklistHttpService.Update(id, vm);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var wasSuccessful = task.Result.IsSuccessStatusCode;
			if(wasSuccessful == false)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}
			
			return RedirectToAction("Edit", new { id });
		}

		[HttpPost]
		public ActionResult Create(AddChecklistViewModel list)
		{
			var task = Task.Run(async () => {
				return await ChecklistHttpService.Create(list);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return RedirectToAction("Index");
		}

		// POST: Checklist/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistHttpService.Delete(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();
			return RedirectToAction("_ChecklistList");
		}

	}
}
