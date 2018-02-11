using System.Security.Claims;

namespace DigitalInspection.Models.Web
{
	public class AuthenticationResponse : BaseResponse
	{
		public ClaimsIdentity ClaimsIdentity { get; set; }
	}
}
