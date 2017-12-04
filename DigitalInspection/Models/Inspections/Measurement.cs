using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Measurement
	{
		private static readonly int DEFAULT_STEP_SIZE = 1;

		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();

		// Foreign key - Nullable allows individual deletion of response
		public Guid? ChecklistItemId { get; set; }

		// Navigation properties for model binding
		public virtual ChecklistItem ChecklistItem { get; set; }

		public virtual IList<InspectionMeasurement> InspectionMeasurements { get; set; }

		[Required(AllowEmptyStrings = true)]
		[DisplayName("Label *")]
		public string Label { get; set; } = string.Empty;

		[Required(ErrorMessage = "Minimum is required")]
		[DisplayName("Minimum *")]
		public int MinValue { get; set; }

		[Required(ErrorMessage = "Maximum is required")]
		[DisplayName("Maximum *")]
		public int MaxValue { get; set; }

		[Required(ErrorMessage = "Step size is required")]
		[DisplayName("Step size *")]
		[Range(1, int.MaxValue)]
		public int StepSize { get; set; } = DEFAULT_STEP_SIZE;

		[Required(AllowEmptyStrings = true)]
		[DisplayName("Unit *")]
		public string Unit { get; set; } = string.Empty;
	}
}