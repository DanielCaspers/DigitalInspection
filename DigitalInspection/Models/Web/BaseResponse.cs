using System.Net;

namespace DigitalInspection.Models.Web
{
	public class BaseResponse
	{
		// TODO: Make derived implement generics
		//public T Entity { get; set; }

		public string ErrorMessage { get; set; }

		public HttpStatusCode HTTPCode { get; set; }

		public bool IsSuccessStatusCode { get; set; }
	}
}
