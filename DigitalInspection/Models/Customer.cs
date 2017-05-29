using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	public class Customer
	{
		[Required]
		public string Id { get; set; }

		[Required(ErrorMessage = "First name is required")]
		[DisplayName("First name *")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required")]
		[DisplayName("Last name *")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Address is required")]
		[DisplayName("Address *")]
		public string Address { get; set; }

		[Required(ErrorMessage = "City is required")]
		[DisplayName("City *")]
		public string City { get; set; }

		[Required(ErrorMessage = "State is required")]
		[DisplayName("State *")]
		public string State { get; set; }

		[Required(ErrorMessage = "ZIP is required")]
		[DisplayName("ZIP *")]
		public string ZIP { get; set; }

		// TODO: Address things
		// TODO: Phone number things
	}
}