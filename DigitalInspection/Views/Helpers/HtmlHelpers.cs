using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalInspection.Views.Helpers
{
	public class HtmlHelpers
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
	}
}