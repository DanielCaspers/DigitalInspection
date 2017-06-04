using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Address
	{
		[Required(ErrorMessage = "Address is required")]
		[DisplayName("Address Line 1 *")]
		public string Line1 { get; set; }

		[Required(ErrorMessage = "Address is required")]
		[DisplayName("Address Line 2 *")]
		public string Line2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[DisplayName("City *")]
		public string City { get; set; }

		[Required(ErrorMessage = "State is required")]
		[DisplayName("State *")]
		public string State { get; set; }

		[Required(ErrorMessage = "ZIP is required")]
		[DisplayName("ZIP *")]
		public string ZIP { get; set; }
	}
}