using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

		// https://msdn.microsoft.com/en-us/library/tabh47cf(v=vs.110).aspx
		// NOTE: Cannot use Environment.NewLine since the filter will be less strict on Mono. 
		public static IList<string> GroupByLineEnding(this string value)
		{
			if (value == null)
			{
				return new List<string>();
			}
			else
			{
				return value.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.None).ToList();
			}
		}
	}
}