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

		// TODO: Change to enum when defined with client
		[Required(ErrorMessage = "Type is required")]
		[DisplayName("Type *")]
		public string Type { get; set; }

		// TODO: Determine how in the world prefs work
		//[Required(ErrorMessage = "Preferences is required")]
		//[DisplayName("Preferences *")]
		//public string Preferences { get; set; }
	}
}