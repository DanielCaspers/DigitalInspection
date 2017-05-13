using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

		[Required(ErrorMessage = "Canned response is required")]
		[DisplayName("Canned Response *")]
		public string Response { get; set; }

		[DisplayName("URL")]
		public string Url { get; set; } = String.Empty;

		[DisplayName("Description")]
		public string Description { get; set; } = String.Empty;
	}
}