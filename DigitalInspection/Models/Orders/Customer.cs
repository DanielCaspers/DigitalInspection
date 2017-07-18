using DigitalInspection.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Customer
	{
		[Required]
		public string Id { get; set; }

		public string Name { get; set; }

		public Address Address { get; set; }

		public IList<PhoneNumber> PhoneNumbers { get; set; }

		public IList<string> Notes { get; set; }

		public Customer() { }

		public Customer(
			string id,
			string name,
			Address address,
			IList<PhoneNumber> phoneNumbers,
			IList<string> notes)
		{
			Id = id;
			Name = name?.ToTitleCase();
			Address = address;
			PhoneNumbers = phoneNumbers;
			Notes = notes;
		}
	}
}