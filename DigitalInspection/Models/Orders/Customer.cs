using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspection.Utils;

namespace DigitalInspection.Models.Orders
{
	public class Customer
	{
		[Required]
		public string Id { get; set; }

		public string Name { get; set; }

		public Address Address { get; set; }

		public IList<PhoneNumber> PhoneNumbers { get; set; }

		[DisplayName("Customer notes")]
		public string Notes { get; set; }

		public DateTime? WaitingSinceDate { get; set; }

		public Customer() { }

		public Customer(
			string id,
			string name,
			Address address,
			IList<PhoneNumber> phoneNumbers,
			IList<string> notes,
			long? waitingSince)
		{
			Id = id;
			Name = name?.ToTitleCase();
			Address = address;
			PhoneNumbers = phoneNumbers;
			Notes = StringExtensions.JoinByLineEnding(notes);
			WaitingSinceDate = DateTimeUtils.FromUnixTime(waitingSince);
		}
	}
}