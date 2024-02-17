using System.Collections.Generic;
using BridgeCalculator.BridgeTimer.StaticClasses;
using BridgeCalculator.Components;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeRun
	{
		public static float DisqualificationDistance => BridgeRunManager.BridgeLength - 1; // BridgeLength - 1 to allow jumping off at the side at the end/beginning

		public string PlayerName { get; private set; }

		private readonly Vector3 bridgeEnteredPosition;

		private List<SideJump> jumps;
		private SideJump currentSideJump;

		private BridgeTimer bridgeTimer;
		private BridgeTrigger bridgeTrigger;

		public BridgeRun(BridgeTrigger trigger, string playerName, Vector3 enterPosition)
		{
			bridgeTrigger = trigger;

			PlayerName = playerName;

			bridgeEnteredPosition = enterPosition;
			bridgeTimer           = new BridgeTimer();

			jumps = new List<SideJump>();
		}

		public void Update()
		{
			bridgeTimer.Update();

			currentSideJump?.Update();
		}

		public void StopRun()
		{
			bridgeTimer.StopTimer();
			
			EndSideJump(false);
		}

		private void StartSideJump()
		{
			BridgeRunLogger.JumpedOffSide(PlayerName);
			currentSideJump = new SideJump(this, bridgeTrigger);
		}

		public void EndSideJump(bool jumpSuccessful)
		{
			if (currentSideJump != null)
			{
				currentSideJump.EndJump(jumpSuccessful);

				if (jumpSuccessful)
				{
					jumps.Add(currentSideJump);
				}

				currentSideJump = null;
			}
		}

		public void LeftBridgeTrigger(Vector3 bridgeLeftPosition)
		{
			float distance = Vector3.Distance(bridgeEnteredPosition, bridgeLeftPosition);

			if (distance < DisqualificationDistance && distance > 2f) // 2 is a reasonable distance that prevents walking off the bridge at the same position from counting as a side jump
			{
				StartSideJump();
			}
			else
			{
				StopRun();
			}
		}

		public void OnDestroy()
		{
			bridgeTrigger = null;
			bridgeTimer   = null;

			currentSideJump?.OnDestroy();
			currentSideJump = null;

			foreach (SideJump jump in jumps)
			{
				jump.OnDestroy();
			}

			jumps.Clear();
		}
	}
}