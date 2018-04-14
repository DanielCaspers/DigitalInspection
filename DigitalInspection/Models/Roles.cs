using System.Web.Mvc;

namespace DigitalInspection.Models
{
	public static class Roles
	{
		// These must match D3-API Roles exactly to function correctly.
		public const string Admin = "Admin";
		public const string LocationManager = "Mgr";
		public const string ServiceAdvisor = "SvcAdvisor";
		public const string Technician = "Tech";
		public const string User = "User";
	}

	public class AuthorizeRolesAttribute : AuthorizeAttribute
	{
		public AuthorizeRolesAttribute(params string[] roles) : base()
		{
			Roles = string.Join(",", roles);
		}
	}
}
