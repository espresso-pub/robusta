using System;
using System.Collections.Generic;
#if ROBUSTA_FACEBOOK
using Facebook.Unity;
#endif
using Robusta.Interfaces;
using UnityEngine;

namespace Robusta
{
	public static class RobustaAnalytics
	{
		private static Settings _settings;

		private static readonly List<IAnalytics> Analytics = new List<IAnalytics> {new Robusta()};

		public static void Init(Settings settings)
		{
			Debug.Log("Initializing Robusta");

			_settings = settings;

			foreach (var analytic in Analytics)
			{
				analytic.Init(_settings);
			}
#if ROBUSTA_FACEBOOK
			Debug.Log("Initializing Facebook SDK");
			FB.Init();
#endif
		}

		internal static event Action OnApplicationPaused;
		internal static event Action OnApplicationStarted;
		internal static event Action OnApplicationResumed;
		internal static event Action<string, string> OnCustomEvent;

		internal static void ApplicationPaused()
		{
			Debug.Log("RobustaAnalytics -> ApplicationPaused");
			OnApplicationPaused?.Invoke();
		}

		internal static void ApplicationStarted()
		{
			Debug.Log("RobustaAnalytics -> ApplicationStarted");
			OnApplicationStarted?.Invoke();
		}

		internal static void ApplicationResumed()
		{
			Debug.Log("RobustaAnalytics -> ApplicationResumed");
			OnApplicationResumed?.Invoke();
		}

		public static void CustomEvent(string eventName, string value)
		{
			OnCustomEvent?.Invoke(eventName, value);
		}
		public static void SetLevel(int level)
		{
			CustomEvent(DefaultEvents.Level,level.ToString());
		}
	}
}