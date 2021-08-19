using System.Collections.Generic;
using Robusta.Events;
using Robusta.Interfaces;
using UnityEngine;

namespace Robusta
{
	public class Robusta : IAnalytics
	{
		private RobustaSession _robustaSession;
		private RobustaSender _robustaSender;
		private string Level = "0";


		private void RegisterEvents()
		{
			RobustaAnalytics.OnApplicationPaused += ApplicationPaused;
			RobustaAnalytics.OnApplicationResumed += ApplicationResumed;
			RobustaAnalytics.OnCustomEvent += OnCustomEvent;
		}

		private void OnCustomEvent(string eventName, string value)
		{
			if (_robustaSession == null) return;

			switch (eventName)
			{
				case DefaultEvents.Level:
					Level = value;
					_robustaSession.Level = Level;
					break;
			}
		}

		private void ApplicationResumed()
		{
			_robustaSession = new RobustaSession();
			_robustaSession.Start();
		}

		private void ApplicationPaused()
		{
			if (_robustaSession == null)
			{
				Debug.Log("Robusta session empty on pause!");
				return;
			}

			_robustaSession.End();
			_robustaSender.SendEvents(new List<IRobustaEventPathGenerator> {_robustaSession});
		}


		public void Init(Settings settings)
		{
			Debug.Log("Robusta initializing");
			_robustaSender = new RobustaSender(SystemInfo.deviceUniqueIdentifier, settings);
			RegisterEvents();
			SendApplicationStart();
			_robustaSession = new RobustaSession();
		}

		private void SendApplicationStart()
		{
			SendZeroSession();
		}

		private void SendZeroSession()
		{
			var fakeSession = new ZeroRobustaSession {Level = Level};
			fakeSession.Start();
			fakeSession.End();
			_robustaSender.SendEvents(new List<IRobustaEventPathGenerator> {fakeSession});
		}
	}
}