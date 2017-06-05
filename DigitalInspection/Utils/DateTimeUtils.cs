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
	}
}