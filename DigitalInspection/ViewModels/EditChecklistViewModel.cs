using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalInspection.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DigitalInspection.ViewModels
{
	public class EditChecklistViewModel: BaseViewModel
	{
		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }

		public Image Picture { get; set; }
	}
}