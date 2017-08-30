using DigitalInspection.Models.Web;
using System.Net.Http;
using System.Threading.Tasks;

namespace DigitalInspection.Services
{
	public class AuthenticationService : HttpClientService
	{
		// https://stackoverflow.com/questions/31129873/make-http-client-synchronous-wait-for-response
		public static async Task<AuthenticationResponse> Login(string username, string password)
		{
			using (HttpClient httpClient = InitializeHttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("x-username", username);
				httpClient.DefaultRequestHeaders.Add("x-password", password);

				HttpResponseMessage response = await httpClient.GetAsync("auth/team/logon");
				string responseJson = await response.Content.ReadAsStringAsync();

				return CreateAuthenticationResponse(response, responseJson);
			}
		}

		private static AuthenticationResponse CreateAuthenticationResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			AuthenticationResponse authenticationResponse = new AuthenticationResponse();
			authenticationResponse.IsSuccessStatusCode = httpResponse.IsSuccessStatusCode;
			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				// TODO: Append fields from JWT in responseContent to AuthResponse
				// TODO: Mutate future instances of HTTPClientService to append the username and password headers if always needed
				// Need this everywhere
				// httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token"); 
			}
			else
			{
				authenticationResponse.ErrorMessage = responseContent;
				authenticationResponse.HTTPCode = httpResponse.StatusCode;
			}
			return authenticationResponse;
		}
	}
}
