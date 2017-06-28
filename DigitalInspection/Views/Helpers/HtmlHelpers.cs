using DigitalInspection.Models;
using System;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace DigitalInspection.Views.Helpers
{
	public static class HtmlHelpers
	{
		// https://stackoverflow.com/a/13923013/2831961
		public static RouteValueDictionary ConditionalDisable(bool shouldDisable, object htmlAttributes = null)
		{
			var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (shouldDisable)
			{
				dictionary.Add("disabled", "disabled");
			}

			return dictionary;
		}

		// https://forums.asp.net/post/4517653.aspx
		public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
		{
			var repID = Guid.NewGuid().ToString();
			var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
			return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
		}

		public static string Pluralizer(string noun)
		{
			return noun.EndsWith("s") ? noun + "'" : noun + "'s";
		}
	}
}