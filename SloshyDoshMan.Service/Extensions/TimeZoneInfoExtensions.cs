using System;

namespace SloshyDoshMan
{
	internal static class TimeZoneInfoExtensions
	{
		internal static DateTimeOffset CurrentTime(this TimeZoneInfo timeZoneInfo)
		{
			return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
		}
	}
}
