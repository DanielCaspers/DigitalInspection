using System.Net;
using System.Net.Http;

namespace DigitalInspection.Models.Web
{
	public class HttpResponse<T>
	{
		public T Entity { get; set; }

		public string ErrorMessage { get; set; }

		public HttpStatusCode HTTPCode { get; set; }

		public bool IsSuccessStatusCode { get; set; }

		public HttpResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
			HTTPCode = httpResponse.StatusCode;
			ErrorMessage = httpResponse.IsSuccessStatusCode ? "" : responseContent;
		}

		public HttpResponse(T entity, string errorMessage, HttpStatusCode httpCode, bool isSuccess)
		{
			Entity = entity;
			ErrorMessage = errorMessage;
			HTTPCode = httpCode;
			IsSuccessStatusCode = isSuccess;
		}
	}
}
