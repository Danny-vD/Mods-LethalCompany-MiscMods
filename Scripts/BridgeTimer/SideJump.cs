using BridgeCalculator.BridgeTimer.StaticClasses;
using BridgeCalculator.Components;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class SideJump
	{
		private const float maximumSideJumpTimer = 5f;

		public bool IsJumping { get; private set; }
		
		public float JumpTimerValue { get; private set; }
		
		public float HealthRegained { get; private set; }

		private float jumpStartedDurability;

		private BridgeRun bridgeRun;
		private BridgeTrigger bridgeTrigger;

		private Transform jumpingTransform;
		private BridgeRunManager bridgeRunManager;

		private string jumpInfo;

		public SideJump(Transform transform, BridgeRun run, BridgeTrigger trigger, BridgeRunManager bridgeRunManager)
		{
			bridgeRun     = run;
			bridgeTrigger = trigger;

			StartTimer();
		}

		public void Update()
		{
			if (IsJumping)
			{
				JumpTimerValue += Time.unscaledDeltaTime;

				if (JumpTimerValue > maximumSideJumpTimer) // Fell off the bridge
				{
					bridgeRun.StopRun(true);
					return;
				}

				if (WentOutsideOfBridge())
				{
					bridgeRun.StopRun(false);
				}
			}
		}

		private void StartTimer()
		{
			jumpStartedDurability = bridgeTrigger.bridgeDurability;

			JumpTimerValue = 0;
			IsJumping      = true;
		}

		public void EndJump(bool success)
		{
			IsJumping = false;

			if (success)
			{
				HealthRegained = bridgeTrigger.bridgeDurability - jumpStartedDurability;
				jumpInfo       = StatisticsCalculator.GetSideJumpString(JumpTimerValue, HealthRegained, bridgeTrigger.bridgeDurability);
				LogInfo();
			}
		}

		public void LogInfo()
		{
			BridgeRunLogger.SuccessfulSideJump(jumpInfo);
		}

		public void LogInfo(int jumpNumber)
		{
			BridgeRunLogger.SideJumpStatistics(jumpInfo, jumpNumber);
		}

		private bool WentOutsideOfBridge()
		{
			Vector3 playerPosition = jumpingTransform.position;
			return playerPosition.z <= bridgeRunManager.TriggerBounds.Item1 || playerPosition.z >= bridgeRunManager.TriggerBounds.Item2;
		}

		public void OnDestroy()
		{
			bridgeRun        = null;
			bridgeTrigger    = null;
			jumpingTransform = null;
			bridgeRunManager = null;
		}
	}
}