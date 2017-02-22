using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalInspection.Models
{
	public class Checklist
	{
		[Required]
		public string Name { get; set; }
		public IList<ChecklistItem> Items { get; set; }

		//[Required]
		public Image Image { get; set; }

		[Required]
		public Guid Id { get; set; }
	}
}