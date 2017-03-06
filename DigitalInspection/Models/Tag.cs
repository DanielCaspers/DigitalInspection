using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Tag
	{
		[Required(ErrorMessage = "Tag name is required")]
		[DisplayName("Tag name *")]
		public string Name { get; set; }

		[Required]
		public Guid Id { get; set; }
	}
}