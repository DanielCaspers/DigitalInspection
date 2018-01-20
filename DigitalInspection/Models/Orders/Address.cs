using DigitalInspection.Utils;
using System.ComponentModel;

namespace DigitalInspection.Models
{
	public class Address
	{
		[DisplayName("Address Line 1")]
		public string Line1 { get; set; }

		[DisplayName("Address Line 2")]
		public string Line2 { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string ZIP { get; set; }

		public Address() { }

		public Address(string line1, string line2, string city, string state, string zip)
		{
			Line1 = line1?.ToTitleCase();
			Line2 = line2?.ToTitleCase();
			City = city?.ToTitleCase();
			State = state;
			ZIP = zip;
		}
	}
}