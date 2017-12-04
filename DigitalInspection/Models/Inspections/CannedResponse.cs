using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspection.Models.Orders;
using System.Collections.Generic;
using System.Linq;

namespace DigitalInspection.Models
{
	public class CannedResponse
	{
		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();

		// Foreign key - Nullable allows individual deletion of response
		public Guid? ChecklistItemId { get; set; }

		// Navigation properties for model binding
		public virtual ChecklistItem ChecklistItem { get; set; }

		public virtual IList<InspectionItem> InspectionItems { get; set; }

		[Required(ErrorMessage = "Canned response is required")]
		[DisplayName("Canned Response *")]
		public string Response { get; set; }

		[DisplayName("URL")]
		public string Url { get; set; } = String.Empty;

		[DisplayName("Description")]
		public string Description { get; set; } = String.Empty;

		[DisplayName("Levels of Concern *")]
		[Required(ErrorMessage = "One or more levels of concern are required")]
		public IList<RecommendedServiceSeverity> LevelsOfConcern { get; set; } = new List<RecommendedServiceSeverity>();

		// Trick to force DB to hold onto enum values
		public string LevelsOfConcernInDb
		{
			get { return string.Join(",", LevelsOfConcern); }
			set {
				if (String.IsNullOrEmpty(value))
				{
					value = "";
					LevelsOfConcern = new List<RecommendedServiceSeverity>();
				}
				else
				{
					IList<string> stringList = value.Split(',').ToList();
					LevelsOfConcern = (IList<RecommendedServiceSeverity>) stringList.Select(s => (RecommendedServiceSeverity) Enum.Parse(typeof(RecommendedServiceSeverity), s)).ToList();
				}
			}
		}
	}
}