using System.Globalization;

namespace DigitalInspection.Utils
{
	public static class StringExtensions
	{
		public static string ToTitleCase(this string value)
		{
			TextInfo ti = new CultureInfo("en-US", false).TextInfo;

			// TODO: Tokenize by spaces, ignore title casing strings if they start with a number
			string lower = value.ToLower();
			return ti.ToTitleCase(lower);
		}
	}
}