using System;
using System.Collections.Generic;
using System.Linq;
using Robusta.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

namespace Robusta
{
	public class RobustaSender
	{
		private readonly string _userId;
		private readonly Settings _settings;
		private static readonly TimeSpan SendTimeout = TimeSpan.FromSeconds(10);

		public RobustaSender(string userId, Settings settings)
		{
			_userId = userId;
			_settings = settings;
		}

		public void SendEvents(IEnumerable<IRobustaEventPathGenerator> robustaEvents)
		{
			Debug.Log("Robusta sending events");
			var apiRoot = ApiRoot();
			foreach (var path in robustaEvents.Select(robustaEvent => robustaEvent.GenerateEventPath(apiRoot)))
			{
				PingUri(path, b => { });
			}
		}

		private static void PingUri(string uri, Action<bool> complete)
		{
			Debug.Log($"Sending ping to {uri}");
			var request = UnityWebRequest.Get(uri);
			request.timeout = SendTimeout.Seconds;
			var asyncOperation = request.SendWebRequest();
			asyncOperation.completed += operation =>
			{
				if (!request.isNetworkError && !request.isHttpError)
				{
					Debug.Log("Pinged robusta event");
					complete(true);
				}
				else
				{
					complete(false);
					Debug.Log($"Failed to ping robusta {uri}: {request.error}");
				}

				request.Dispose();
			};
		}

		private string ApiRoot()
		{
			return $"{_settings.Url}event/{_settings.AppId}/{_userId}";
		}
	}
}