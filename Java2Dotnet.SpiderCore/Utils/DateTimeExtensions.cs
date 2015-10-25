using System;

namespace Java2Dotnet.Spider.Core.Utils
{
	public static class DateTimeExtensions
	{
		private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long CurrentTimeMillis(this DateTime d)
		{
			return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds);
		}
	}
}
