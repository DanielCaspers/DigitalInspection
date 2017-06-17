using System;

namespace DigitalInspection.Utils
{
	public class DateTimeUtils
	{
		public static DateTime FromUnixTime(long unixTimeInSeconds)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
			return epoch.AddSeconds(unixTimeInSeconds).ToLocalTime();
		}

		public static long ToUnixTime(DateTime dateTime)
		{
			// Requires ASP.NET 4.6 target framework. May not be worth upgrading...
			// return DateTimeOffset.ToUnixTimeSeconds();
			long timeInSeconds = 0;
			return timeInSeconds;
		}
	}
}