using System;

namespace Robusta.Helpers
{
	public static class DateTimeHelpers
	{
		/// <summary>
		/// Gets a Unix timestamp representing the current moment
		/// </summary>
		/// <param name="dt"></param>
		/// <returns>Now expressed as a Unix timestamp</returns>
		public static long UnixTimestamp(this DateTime dt)
		{
			return new DateTimeOffset(dt).ToUnixTimeSeconds();
		}
	}
}