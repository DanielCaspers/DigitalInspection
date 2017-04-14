using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Measurement
	{
		[Required]
		public Guid Id { get; set; }

		//[Required(ErrorMessage = "Label is required")]
		[Required]
		public string Label { get; set; }

		[Required]
		public uint MinValue { get; set; }

		[Required]
		public uint MaxValue { get; set; }

		[Required]
		public uint StepSize { get; set; }

		[Required]
		public string Unit { get; set; }
	}
}