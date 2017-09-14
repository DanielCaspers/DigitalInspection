using System.Web.Mvc;

namespace DigitalInspection.Views.Helpers
{
	public static class RecommendedServiceHelpers
	{

		public static MvcHtmlString Immediate(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "warning", "Immediate", "immediate");
		}

		public static MvcHtmlString Moderate(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "alarm", "Moderate", "moderate");
		}

		public static MvcHtmlString Watch(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "remove_red_eye", "Should watch", "watch");
		}

		public static MvcHtmlString Maintenance(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "date_range", "Maintenance", "maintenance");
		}

		public static MvcHtmlString Notes(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "description", "Notes", "notes");
		}

		public static MvcHtmlString Unknown(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "assignment_late", "Unknown", "unknown");
		}

		private static MvcHtmlString CreateIndicator(
			bool hasIcon, bool hasLabel, string iconName, string labelText, string cssClass)
		{
			TagBuilder container = new TagBuilder("div");
			if (hasIcon)
			{
				var icon = CreateIcon(iconName);
				icon.AddCssClass(cssClass);
				container.InnerHtml += icon.ToString();
			}

			if (hasLabel)
			{
				var label = CreateLabel(labelText);
				label.AddCssClass(cssClass);
				container.InnerHtml += label.ToString();
			}

			return MvcHtmlString.Create(container.ToString());
		}

		private static TagBuilder CreateIcon(string iconName)
		{
			var icon = new TagBuilder("i");
			icon.AddCssClass("material-icons recommended-service");
			icon.InnerHtml = iconName;
			return icon;
		}

		private static TagBuilder CreateLabel(string labelText)
		{
			var label = new TagBuilder("span");
			label.AddCssClass("label recommended-service");
			label.InnerHtml = labelText;
			return label;
		}
	}
}
