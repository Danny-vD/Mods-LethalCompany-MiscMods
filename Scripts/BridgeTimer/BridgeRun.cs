using System.Collections.Generic;
using BridgeCalculator.BridgeTimer.StaticClasses;
using BridgeCalculator.Data;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeRun
	{
		public float AllowableFullLength => BridgeLength - 1; // BridgeLength - 1 to allow jumping off at the side at the end/beginning
		
		public float BridgeLength { get; private set; }

		public string PlayerName { get; private set; }

		public BridgeTimer BridgeTimer { get; private set; }
		public BridgeTrigger BridgeTrigger { get; private set; }
		public PlayerRunStatistics Statistics { get; private set; }
		
		private readonly Vector3 bridgeEnteredPosition;
		private Vector3 bridgeLeftPosition;

		private List<SideJump> jumps;
		private SideJump currentSideJump;

		public BridgeRun(BridgeTrigger trigger, string playerName, Vector3 enterPosition, PlayerRunStatistics statistics, float fullBridgeLength)
		{
			BridgeTrigger = trigger;

			PlayerName = playerName;

			bridgeEnteredPosition = enterPosition;
			BridgeTimer           = new BridgeTimer();

			jumps = new List<SideJump>();

			Statistics = statistics;

			BridgeLength = fullBridgeLength;
		}

		public void Update()
		{
			BridgeTimer.Update();

			currentSideJump?.Update();
		}

		public void StopRun(bool fellOffBridge)
		{
			BridgeTimer.StopTimer();

			EndSideJump(false);
			
			LogInfo(fellOffBridge);
		}

		private void StartSideJump()
		{
			BridgeRunLogger.JumpedOffSide(PlayerName);
			currentSideJump = new SideJump(this, BridgeTrigger);
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

		public bool LeftBridgeTrigger(Vector3 bridgeExitPosition)
		{
			bool runStopped = false;
			float distance = Vector3.Distance(bridgeEnteredPosition, bridgeExitPosition);

			if (distance < AllowableFullLength && distance > 2f) // 2 is a reasonable distance that prevents walking off the bridge at the same position from counting as a side jump
			{
				StartSideJump();
			}
			else
			{
				bridgeLeftPosition = bridgeExitPosition;
				StopRun(false);
				
				runStopped = true;
			}

			return runStopped;
		}

		private void LogInfo(bool fellOffBridge)
		{
			string runInfo = StatisticsCalculator.GetStatisticsString(bridgeEnteredPosition, bridgeLeftPosition, Statistics, this);
			
			BridgeRunLogger.EndRunStatistics(PlayerName, runInfo, fellOffBridge);
			
			for (int i = 0; i < jumps.Count;)
			{
				SideJump jump = jumps[i];
				jump.LogInfo(++i);
			}
		}

		public void OnDestroy()
		{
			BridgeTrigger = null;
			BridgeTimer   = null;

			currentSideJump?.OnDestroy();
			currentSideJump = null;

			foreach (SideJump jump in jumps)
			{
				jump.OnDestroy();
			}

			jumps.Clear();

			Statistics = null;
		}
	}
}