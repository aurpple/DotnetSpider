using System;

namespace Java2Dotnet.Spider.Lib
{
	public static class DateTimeUtil
	{
		/// <returns></returns>
		public static string GetCurrentTimeStampString()
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString("f0");
		}

		/// <returns></returns>
		public static double GetCurrentTimeStamp()
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}
