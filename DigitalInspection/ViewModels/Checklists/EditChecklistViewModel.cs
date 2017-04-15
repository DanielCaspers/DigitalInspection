using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalInspection.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DigitalInspection.ViewModels
{
	public class EditChecklistViewModel: BaseChecklistsViewModel
	{
		public Checklist Checklist { get; set; }
	}
}