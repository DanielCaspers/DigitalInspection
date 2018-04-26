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
		public static DateTime? FromUnixTime(long? unixTimeInSeconds)
		{
			if (unixTimeInSeconds.HasValue)
			{
				DateTimeOffset dt = DateTimeOffset.FromUnixTimeSeconds(unixTimeInSeconds.Value);
				return dt.LocalDateTime;
			}

			return null;
		}

		public static long? ToUnixTime(DateTime? dateTime)
		{
			if (dateTime.HasValue)
			{
				DateTimeOffset dt = dateTime.Value;
				return dt.ToUnixTimeSeconds();
			}

			return null;
		}

		public static string DeltaFromNow(DateTime dateTime)
		{
			var delta = DateTime.Now.Subtract(dateTime);
			string waitTime;
			if (delta.Hours > 0)
			{
				waitTime = string.Format("{0} hours, {1} minutes", delta.Hours, delta.Minutes);
			}
			else
			{
				waitTime = string.Format("{0} minutes", delta.Minutes);
			}

			return waitTime;
		}
	}
}