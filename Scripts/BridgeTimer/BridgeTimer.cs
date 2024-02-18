using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeTimer
	{
		public bool IsTimerRunning { get; private set; }
		
		public float TimerValue { get; private set; }

		public BridgeTimer()
		{
			StartTimer();
		}

		public void Update()
		{
			if (IsTimerRunning)
			{
				TimerValue += Time.unscaledDeltaTime;
			}
		}

		private void StartTimer()
		{
			TimerValue  = 0;
			IsTimerRunning = true;
		}
		
		public void StopTimer()
		{
			if (IsTimerRunning)
			{
				IsTimerRunning = false;
			}
		}
	}
}