﻿using DigitalInspection.Models;
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
	}
}