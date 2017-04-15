﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Measurement
	{
		private static readonly int DEFAULT_STEP_SIZE = 1;

		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();

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