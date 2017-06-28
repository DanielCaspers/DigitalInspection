using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class PhoneNumber
	{
		[Required(ErrorMessage = "Number is required")]
		[DisplayName("Phone Number *")]
		public string Number { get; set; }

		[Required(ErrorMessage = "Contact Name is required")]
		[DisplayName("Contact Name *")]
		public string ContactName { get; set; }

		[Required(ErrorMessage = "Type is required")]
		[DisplayName("Type *")]
		public string Type { get; set; }

		[Required(ErrorMessage = "SMS Preferences are required")]
		[DisplayName("SMS Preferences *")]
		public string SMSPreferences { get; set; }

		public PhoneNumber() { }

		public PhoneNumber(string number, string contact, string type, string smsPreferences)
		{
			Number = number;
			ContactName = contact;
			Type = type;
			SMSPreferences = smsPreferences;
		}
	}
}