using System.Collections.Generic;
using BridgeCalculator.BridgeTimer.StaticClasses;
using BridgeCalculator.Components;
using BridgeCalculator.Data;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeRun
	{
		public static float BridgeDistance => BridgeRunManager.BridgeLength - 1; // BridgeLength - 1 to allow jumping off at the side at the end/beginning

		public string PlayerName { get; private set; }

		public BridgeTimer BridgeTimer { get; private set; }
		public BridgeTrigger BridgeTrigger { get; private set; }
		public PlayerRunStatistics Statistics { get; private set; }

		private readonly Vector3 bridgeEnteredPosition;
		private Vector3 bridgeLeftPosition;

		private List<SideJump> jumps;
		private SideJump currentSideJump;

		public BridgeRun(BridgeTrigger trigger, string playerName, Vector3 enterPosition, PlayerRunStatistics statistics)
		{
			BridgeTrigger = trigger;

			PlayerName = playerName;

			bridgeEnteredPosition = enterPosition;
			BridgeTimer           = new BridgeTimer();

			jumps = new List<SideJump>();

			Statistics = statistics;
		}

		public void Update()
		{
			BridgeTimer.Update();

			currentSideJump?.Update();
		}

		public void StopRun()
		{
			BridgeTimer.StopTimer();

			EndSideJump(false);
			
			LogInfo();
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

		public void LeftBridgeTrigger(Vector3 bridgeExitPosition)
		{
			float distance = Vector3.Distance(bridgeEnteredPosition, bridgeExitPosition);

			if (distance < BridgeDistance && distance > 2f) // 2 is a reasonable distance that prevents walking off the bridge at the same position from counting as a side jump
			{
				StartSideJump();
			}
			else
			{
				bridgeLeftPosition = bridgeExitPosition;
				StopRun();
			}
		}

		public void LogInfo()
		{
			string runInfo = StatisticsCalculator.GetStatisticsString(bridgeEnteredPosition, bridgeLeftPosition, Statistics, this);
			
			for (int i = 0; i < jumps.Count;)
			{
				SideJump jump = jumps[i];
				jump.LogInfo(++i);
			}
			
			BridgeRunLogger.EndRunStatistics(runInfo);
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