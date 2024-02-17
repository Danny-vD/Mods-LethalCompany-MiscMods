using BridgeCalculator.Utils;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class SideJump
	{
		private const float maximumSideJumpTimer = 5f;
		
		private float jumpStartedDurability;

		private bool startedTimer = false;
		private float jumpTimer = 0;

		private BridgeRun bridgeRun;
		private BridgeTrigger bridgeTrigger;
		
		public SideJump(BridgeRun run, BridgeTrigger trigger)
		{
			bridgeRun     = run;
			bridgeTrigger = trigger;
			
			StartTimer();
		}

		public void Update()
		{
			if (startedTimer)
			{
				jumpTimer += Time.unscaledDeltaTime;

				if (jumpTimer > maximumSideJumpTimer) // Fell off the bridge
				{
					bridgeRun.FellOffBridge();
				}
			}
		}

		private void StartTimer()
		{
			jumpStartedDurability = bridgeTrigger.bridgeDurability;

			jumpTimer  = 0;
			startedTimer = true;
		}

		public void EndJump(bool success)
		{
			startedTimer = false;

			if (success)
			{
				float healthRegained = bridgeTrigger.bridgeDurability - jumpStartedDurability;
				
				LoggerUtil.LogWarning(
					$"Successful jump!\nJump time: {jumpTimer} seconds\nHealth regained: {healthRegained * 100}%\nCurrent Health: {bridgeTrigger.bridgeDurability * 100}%\n");
			}
		}
		
		public void OnDestroy()
		{
			bridgeRun     = null;
			bridgeTrigger = null;
		}
	}
}