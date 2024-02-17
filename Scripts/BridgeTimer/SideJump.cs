using BridgeCalculator.BridgeTimer.StaticClasses;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class SideJump
	{
		private const float maximumSideJumpTimer = 5f;
		
		public bool IsJumping = false;
		
		private float jumpStartedDurability;
		
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
			if (IsJumping)
			{
				jumpTimer += Time.unscaledDeltaTime;

				if (jumpTimer > maximumSideJumpTimer) // Fell off the bridge
				{
					bridgeRun.StopRun();
				}
			}
		}

		private void StartTimer()
		{
			jumpStartedDurability = bridgeTrigger.bridgeDurability;

			jumpTimer  = 0;
			IsJumping = true;
		}

		public void EndJump(bool success)
		{
			IsJumping = false;

			if (success)
			{
				BridgeRunLogger.SuccessfulSideJump(jumpTimer, jumpStartedDurability, bridgeTrigger.bridgeDurability);
			}
		}
		
		public void OnDestroy()
		{
			bridgeRun     = null;
			bridgeTrigger = null;
		}
	}
}