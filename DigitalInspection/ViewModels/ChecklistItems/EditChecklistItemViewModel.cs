using DigitalInspection.Models;
using DigitalInspection.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.ViewModels
{
	public class EditChecklistItemViewModel: BaseChecklistsViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		[DisplayName("Tags *")]
		public IList<Tag> Tags { get; set; }

		[Required(ErrorMessage = "One or more tags are required")]
		public IEnumerable<Guid> SelectedTagIds { get; set; }

		public IEnumerable<RecommendedServiceSeverity> RecommendedServiceSeverities { get; set; } = new List<RecommendedServiceSeverity>()
		{
			RecommendedServiceSeverity.OK,
			RecommendedServiceSeverity.IMMEDIATE,
			RecommendedServiceSeverity.MODERATE,
			RecommendedServiceSeverity.SHOULD_WATCH,
			RecommendedServiceSeverity.MAINTENANCE,
			RecommendedServiceSeverity.NOTES,
			RecommendedServiceSeverity.NOT_APPLICABLE
		};
	}
}
