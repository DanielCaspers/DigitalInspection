using System;

namespace DigitalInspection.Utils
{
	/**
	 * All dates we handle for now are serialized on the server in UTC.
	 * We will always display them in views in local time, which is either
	 * -6 CST or -5 CDT
	 *  
	 *  https://github.com/scaspers/D3-API/issues/4#issuecomment-309319000
	 */
	public class DateTimeUtils
	{ 
		public static DateTime FromUnixTime(long unixTimeInSeconds)
		{
			DateTimeOffset dt = DateTimeOffset.FromUnixTimeSeconds(unixTimeInSeconds);
			return dt.LocalDateTime;
		}

		public static long ToUnixTime(DateTime dateTime)
		{
			DateTimeOffset dt = dateTime;
			return dt.ToUnixTimeSeconds();
		}
	}
}