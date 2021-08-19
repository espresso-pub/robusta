using System;
using Robusta.Interfaces;
using Robusta.Helpers;

namespace Robusta.Events
{
	public class ZeroRobustaSession : RobustaSession
	{
		public new TimeSpan SessionLength => TimeSpan.Zero;
	}
}