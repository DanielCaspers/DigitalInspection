using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspection.ViewModels.Base;

namespace DigitalInspection.ViewModels
{
	public class LoginViewModel: BaseViewModel
	{
		[Required(ErrorMessage = "Username is required")]
		[DisplayName("Username *")]
		public string Username { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password is required")]
		[DisplayName("Password *")]
		public string Password { get; set; }
	}
}
