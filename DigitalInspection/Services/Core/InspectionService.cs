using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using DigitalInspection.Models;
using DigitalInspection.Models.Inspections;
using DigitalInspection.Models.Orders;

namespace DigitalInspection.Services.Core
{
	public static class InspectionService
	{
		#region Inspection CRUD

		public static Inspection GetOrCreateInspection(ApplicationDbContext ctx, string workOrderId, Checklist checklist)
		{
			var inspection = GetOrCreateInspectionInternal(ctx, workOrderId, checklist);
			inspection = GetOrCreateInspectionItems(ctx, checklist, inspection);
			return GetOrCreateInspectionMeasurements(ctx, inspection);
		}

		public static bool DeleteInspection(ApplicationDbContext ctx, Inspection inspection)
		{
			var inspectionMeasurements = ctx.InspectionMeasurements.Where(im => im.InspectionItem.Inspection.Id == inspection.Id);
			ctx.InspectionMeasurements.RemoveRange(inspectionMeasurements);

			var inspectionImages = ctx.InspectionImages.Where(ii => ii.InspectionItem.Inspection.Id == inspection.Id).ToList();
			inspectionImages.ForEach(ImageService.DeleteImage);

			// TODO: Revise service when dependency injection framework is in use, and abstract into a service
			string path = HttpContext.Current.Server.MapPath($"~/Uploads/Inspections/{inspection.WorkOrderId}");
			try
			{
				Directory.Delete(path, true);
			}
			catch (IOException)
			{
				Directory.Delete(path, true);
			}
			catch (UnauthorizedAccessException)
			{
				Directory.Delete(path, true);
			}

			ctx.InspectionImages.RemoveRange(inspectionImages);

			var inspectionItems = ctx.InspectionItems.Where(ii => ii.Inspection.Id == inspection.Id);
			ctx.InspectionItems.RemoveRange(inspectionItems);

			ctx.Inspections.Remove(inspection);

			return TrySave(ctx);
		}

		#endregion

		#region InspectionItem CRUD

		// Return value indicates success of operation
		public static bool UpdateInspectionItemCondition(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			RecommendedServiceSeverity inspectionItemCondition)
		{
			// Only change condition and canned response if different from previous condition
			if (inspectionItem.Condition == inspectionItemCondition)
			{
				return true;
			}

			inspectionItem.Condition = inspectionItemCondition;

			// Clear canned response IDs when switching conditions.
			// This is because otherwise, the inspection table's select box and the DB (and thus the report) can get out of sync
			foreach (var cannedResponse in inspectionItem.CannedResponses)
			{
				ctx.CannedResponses.Attach(cannedResponse);
			}
			inspectionItem.CannedResponses = new List<CannedResponse>();

			return TrySave(ctx);
		}

		public static bool UpdateInspectionItemCannedResponses(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			IList<Guid> selectedCannedResponseIds)
		{
			foreach (var cannedResponse in inspectionItem.CannedResponses)
			{
				ctx.CannedResponses.Attach(cannedResponse);
			}

			inspectionItem.CannedResponses = 
				selectedCannedResponseIds
					.Select(crId => ctx.CannedResponses.Single(cr => cr.Id == crId))
					.ToList();

			return TrySave(ctx);
		}

		public static bool UpdateIsCustomerConcern(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			bool isCustomerConcern)
		{
			inspectionItem.IsCustomerConcern = isCustomerConcern;

			return TrySave(ctx);
		}

		public static bool UpdateInspectionItemNote(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			string note)
		{
			inspectionItem.Note = note;
			return TrySave(ctx);
		}

		public static bool UpdateInspectionItemMeasurements(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			IEnumerable<InspectionMeasurement> inspectionMeasurements)
		{
			foreach (var inspectionMeasurement in inspectionMeasurements)
			{
				ctx.InspectionMeasurements
					.Single(im => im.Id == inspectionMeasurement.Id)
					.Value = inspectionMeasurement.Value;
			}

			return TrySave(ctx);
		}

		public static bool AddInspectionItemImage(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			Image image)
		{
			var inspectionImage = new InspectionImage
			{
				Id = image.Id,
				Title = image.Title,
				CreatedDate = image.CreatedDate,
				ImageUrl = image.ImageUrl,
				InspectionItem = inspectionItem
			};

			inspectionItem.InspectionImages.Add(inspectionImage);
			ctx.InspectionImages.Add(inspectionImage);

			return TrySave(ctx);
		}

		public static bool DeleteInspectionItemImage(
			ApplicationDbContext ctx,
			InspectionImage image)
		{
			ctx.InspectionImages.Remove(image);
			return TrySave(ctx);
		}

		public static bool UpdateInspectionImageVisibility(
			ApplicationDbContext ctx,
			InspectionImage image,
			bool isVisibleToCustomer)
		{
			image.IsVisibleToCustomer = isVisibleToCustomer;

			return TrySave(ctx);
		}

		#endregion

		#region Private Helpers

		/**
		 * Gets inspection from DB if already exists, or create one in the DB and then return it. 
		 */
		private static Inspection GetOrCreateInspectionInternal(
			ApplicationDbContext ctx,
			string workOrderId,
			Checklist checklist)
		{
			var inspection = ctx.Inspections.SingleOrDefault(i => i.WorkOrderId == workOrderId);
			if (inspection == null)
			{
				inspection = new Inspection
				{
					WorkOrderId = workOrderId,
					Checklists = new List<Checklist> { checklist }
				};

				ctx.Inspections.Add(inspection);
			}
			// An inspection exists, but this checklist has not yet been performed on it
			else if (inspection.Checklists.Any(c => c.Id == checklist.Id) == false)
			{
				inspection.Checklists.Add(checklist);
			}

			TrySave(ctx);

			return inspection;
		}

		/**
		 * Gets all inspectionitems from DB if they already exists, and creates them if they don't already 
		 */
		private static Inspection GetOrCreateInspectionItems(
			ApplicationDbContext ctx,
			Checklist checklist,
			Inspection inspection)
		{
			foreach (var ci in checklist.ChecklistItems)
			{
				var inspectionItem = inspection.InspectionItems.SingleOrDefault(item => item.ChecklistItem.Id == ci.Id);
				if (inspectionItem == null)
				{
					inspectionItem = new InspectionItem
					{
						ChecklistItem = ci,
						Inspection = inspection
					};

					inspection.InspectionItems.Add(inspectionItem);
					ctx.InspectionItems.Add(inspectionItem);
				}
			}

			TrySave(ctx);

			return inspection;
		}

		/**
		 * Gets all inspectionmeasurements from DB if they already exist, and creates them if they don't already 
		 */
		private static Inspection GetOrCreateInspectionMeasurements(ApplicationDbContext ctx, Inspection inspection)
		{
			foreach (var item in inspection.InspectionItems)
			{
				var measurements = ctx.Measurements.Where(m => m.ChecklistItem.Id == item.ChecklistItem.Id).ToList();

				foreach (var measurement in measurements)
				{
					if (item.InspectionMeasurements.Any(im => im.Measurement.Id == measurement.Id) == false)
					{
						var inspectionMeasurement = new InspectionMeasurement
						{
							InspectionItem = item,
							Measurement = measurement,
							Value = null
						};
						ctx.InspectionMeasurements.Add(inspectionMeasurement);
					}
				}
			}

			TrySave(ctx);

			return ctx.Inspections.Single(i => i.Id == inspection.Id);
		}

		private static bool TrySave(DbContext ctx)
		{
			bool wasSuccessful = false;
			try
			{
				ctx.SaveChanges();
				wasSuccessful = true;
			}
			catch (DbEntityValidationException dbEx)
			{
				ExceptionHandlerService.HandleException(dbEx);
			}

			return wasSuccessful;
		}

		#endregion
	}

}
