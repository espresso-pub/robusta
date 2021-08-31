using System;
using Robusta.Interfaces;
using Robusta.Helpers;

namespace Robusta.Events
{
	public class RobustaSession : IRobustaEventPathGenerator
	{
		public DateTime StartTime { get; private set; }
		public DateTime CurrentEndTime { get; private set; }
		public string Level { get; set; } = "0";


		public void Start()
		{
			StartTime = DateTime.UtcNow;
		}

		public void Update()
		{
			CurrentEndTime = DateTime.UtcNow;
		}

		public TimeSpan SessionLength => CurrentEndTime - StartTime;

		public string GenerateEventPath(string apiRoot)
		{
			// https://{server}/api/v1/{app_code}/event/{user_id}/{session_start_time}/{session_length_seconds}/{level}
			return $"{apiRoot}/{StartTime.UnixTimestamp()}/{SessionLength.Seconds}/{Level}";
		}
	}
}