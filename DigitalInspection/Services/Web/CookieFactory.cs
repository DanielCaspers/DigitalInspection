using System;
using System.Web;

namespace DigitalInspection.Services.Web
{
	public static class CookieFactory
	{
		public static readonly string CompanyCookieName = "DI_CompanyNumber";
		public static readonly string UserIdCookieName = "DI_UserId";

		public static HttpCookie CreateCompanyCookie(string value)
		{
			return Create(CompanyCookieName, value);
		}

		public static HttpCookie DeleteCompanyCookie()
		{
			return Delete(CompanyCookieName);
		}

		public static HttpCookie CreateUserIdCookie(string value)
		{
			return Create(UserIdCookieName, value);
		}

		public static HttpCookie DeleteUserIdCookie()
		{
			return Delete(UserIdCookieName);
		}

		private static HttpCookie Create(string cookieName, string value)
		{
			return new HttpCookie(cookieName)
			{
				Value = value,
				Expires = DateTime.Now.AddYears(10)
			};
		}

		private static HttpCookie Delete(string cookieName)
		{
			return new HttpCookie(cookieName)
			{
				Value = "",
				Expires = DateTime.Now.AddDays(-10)
			};
		}
	}

}
