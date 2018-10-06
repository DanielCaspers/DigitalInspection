using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.Services;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Services.Web;
using DigitalInspection.ViewModels.ChecklistItems;

namespace DigitalInspection.Controllers
{
	[AuthorizeRoles(Roles.Admin)]
	public class ChecklistItemsController : BaseController
	{
		public ChecklistItemsController()
		{
			ResourceName = "Checklist item";
		}

		private ManageChecklistItemsViewModel GetChecklistItemViewModel()
		{
			var tagsTask = Task.Run(async () => {
				return await TagService.GetTags();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			tagsTask.Wait();

			var checklistItemsTask = Task.Run(async () => {
				return await ChecklistItemService.GetChecklistItems();
			});
			// Force Synchronous run for Mono to work. See Issue #37
			tagsTask.Wait();

			var checklistItems = checklistItemsTask.Result.Entity.ToList();
			var tags = tagsTask.Result.Entity.ToList();
			return new ManageChecklistItemsViewModel
			{
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

		public ViewResult Report()
		{
			return View("Report", GetChecklistItemViewModel());
		}

		//GET: ChecklistItems/Edit/:id
		public PartialViewResult Edit(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.GetEdit(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var viewModel = task.Result.Entity;

			return viewModel == null ?
				PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName)) :
				PartialView("_EditChecklistItem", viewModel);
		}

		[HttpPost]
		public ActionResult Create(AddChecklistItemViewModel checklistItem, IList<Guid> tags)
		{
			checklistItem.TagIds = tags;

			var task = Task.Run(async () => {
				return await ChecklistItemService.CreateChecklistItem(checklistItem);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return RedirectToAction("_ChecklistItemList");
		}

		[HttpPost]
		public ActionResult Update(Guid id, EditChecklistItemViewModel vm)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.UpdateChecklistItem(id, vm);
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
		public ActionResult Delete(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.DeleteChecklistItem(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			var wasSuccessful = task.Result.IsSuccessStatusCode;
			if (wasSuccessful == false)
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred(new Exception()));
			}

			return RedirectToAction("_ChecklistItemList");
		}

		[HttpPost]
		public ActionResult AddMeasurement(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.CreateMeasurement(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode == false)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			return RedirectToAction("Edit", new { id });
		}

		[HttpPost]
		public ActionResult DeleteMeasurement(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.DeleteMeasurement(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult AddCannedResponse(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.CreateCannedResponse(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			if (task.Result.IsSuccessStatusCode == false)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}

			return RedirectToAction("Edit", new { id });
		}

		[HttpPost]
		public ActionResult DeleteCannedResponse(Guid id)
		{
			var task = Task.Run(async () => {
				return await ChecklistItemService.DeleteCannedResponse(id);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return RedirectToAction("Index");
		}
	}
}
