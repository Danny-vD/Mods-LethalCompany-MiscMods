using System;

namespace BridgeCalculator.Events
{
	public static class BridgeFallenEvent
	{
		public static event Action OnBridgeFallen = delegate { };

		public static void RaiseEvent()
		{
			OnBridgeFallen.Invoke();
		}
	}
}