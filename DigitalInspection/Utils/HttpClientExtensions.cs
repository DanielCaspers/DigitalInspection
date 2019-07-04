using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalInspection.Utils
{
	public static class HttpClientExtensions
	{
		public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
		{
			var method = new HttpMethod("PATCH");

			var request = new HttpRequestMessage(method, requestUri)
			{
				Content = content
			};

			return await client.SendAsync(request);
		}

		public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content)
		{
			var method = new HttpMethod("PATCH");

			var request = new HttpRequestMessage(method, requestUri)
			{
				Content = content
			};

			return await client.SendAsync(request);
		}

		public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			var method = new HttpMethod("PATCH");

			var request = new HttpRequestMessage(method, requestUri)
			{
				Content = content
			};

			return await client.SendAsync(request, cancellationToken);
		}

		public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			var method = new HttpMethod("PATCH");

			var request = new HttpRequestMessage(method, requestUri)
			{
				Content = content
			};

			return await client.SendAsync(request, cancellationToken);
		}
	}
}
