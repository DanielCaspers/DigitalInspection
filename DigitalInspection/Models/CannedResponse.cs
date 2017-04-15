using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class CannedResponse
	{
		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required(ErrorMessage = "Canned response is required")]
		[DisplayName("Canned Response *")]
		public string Response { get; set; }

		[DisplayName("URL")]
		public string Url { get; set; } = String.Empty;

		[DisplayName("Description")]
		public string Description { get; set; } = String.Empty;
	}
}