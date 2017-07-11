using AutoMapper;
using DigitalInspection.Models;
using DigitalInspection.Models.DTOs;
using DigitalInspection.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DigitalInspection.Services
{
	public class HttpClientService
	{
		protected static HttpClient InitializeHttpClient()
		{
			var httpClient = new HttpClient();
			// TODO: Pass in custom configuration from file for store number and app key
			httpClient.BaseAddress = new Uri("https://d3-devel.murphyauto.net/api/v2/004/");
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("x-appkey", "82kkf452j2lL41430SpqFd6Dwe027z");
			httpClient.DefaultRequestHeaders.Add("x-authtoken", "eyJhbGciOiJub25lIiwidHlwIjoiRDNKV1QiLCJwQnl0ZXMiOiJlOTRjYzE5NGI0M2UyNTdmMmFhZTI4OTI0ZTYxZThlNyJ9.eyJ1c2VySUQiOiJzY2FzcGVycyIsImlhdCI6IjE0OTQwNzk5MzAiLCJleHAiOiIxNDk0MTY2MzMwIiwiYWRtaW4iOiJ0cnVlIn0.MWMzZTg0ZDM3YWEwNTI1Yjg3OGU1ZmRkZjBmOGFhYTU1M2MyZDgyMWI1NWEwOGFiNzg2MDA3Nzg2NTYwZDg1OA==");
			return httpClient;
		}
	}
}