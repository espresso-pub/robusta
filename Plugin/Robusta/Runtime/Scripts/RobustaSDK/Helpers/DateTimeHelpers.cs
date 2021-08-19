using System;

namespace Robusta.Helpers
{
	public static class DateTimeHelpers
	{
		/// <summary>
		/// Gets a Unix timestamp representing the current moment
		/// </summary>
		/// <param name="ignored">Parameter ignored</param>
		/// <returns>Now expressed as a Unix timestamp</returns>
		public static long UnixTimestamp(this DateTime ignored)
		{
			return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}
	}
}