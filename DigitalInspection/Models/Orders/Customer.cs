﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Customer
	{
		[Required]
		public string Id { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[DisplayName("Name *")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public Address Address { get; set; }

		[Required(ErrorMessage = "One or more phone numbers are required")]
		public IList<PhoneNumber> PhoneNumbers { get; set; }
	}
}