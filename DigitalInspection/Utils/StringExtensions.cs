using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DigitalInspection.Utils
{
	public static class StringExtensions
	{
		const string WINDOWS_NEW_LINE = "\r\n";
		const string UNIX_NEW_LINE = "\n";

		public static string ToTitleCase(this string value)
		{
			TextInfo ti = new CultureInfo("en-US", false).TextInfo;

			// TODO: Tokenize by spaces, ignore title casing strings if they start with a number
			var lower = value.ToLower();
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
				return value.Split(new string[] { WINDOWS_NEW_LINE, UNIX_NEW_LINE }, StringSplitOptions.None).ToList();
			}
		}

		public static string JoinByLineEnding(IList<string> lines)
		{
			if (lines == null || lines.Count == 0)
			{
				return "";
			}
			else
			{
				// Note that windows line feeds are processed first, since the unix special character pattern is a strict substring
				var pass1 = string.Join(WINDOWS_NEW_LINE, lines);
				var pass2 = string.Join(UNIX_NEW_LINE, pass1);

				return pass2;
			}
		}
	}
}
