using System;

namespace DigitalInspection.Utils
{
	/**
	 * All dates we handle for now are in Local Time, UTC -6
	 */
	public class DateTimeUtils
	{
		private static readonly int SECONDS_PER_HOUR = 3600;
		private static readonly int LOCAL_TIME_OFFSET_IN_HOURS = -6;

		public static DateTime FromUnixTime(long unixTimeInSeconds)
		{
			DateTimeOffset dt = DateTimeOffset.FromUnixTimeSeconds(unixTimeInSeconds);
			return dt.LocalDateTime;
		}

		public static long ToUnixTime(DateTime dateTime)
		{
			DateTimeOffset dt = dateTime;
			return dt.ToUnixTimeSeconds() + (LOCAL_TIME_OFFSET_IN_HOURS * SECONDS_PER_HOUR);
		}
	}
}