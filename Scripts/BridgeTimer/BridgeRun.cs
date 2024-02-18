using System;
using System.Collections.Generic;
using BridgeCalculator.BridgeTimer.StaticClasses;
using BridgeCalculator.Components;
using BridgeCalculator.Data;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeRun
	{
		public float AllowableFullLength => BridgeLength - 2; // BridgeLength - 1 to allow jumping off at the side at the end/beginning

		public float BridgeLength => BridgeRunManager.BridgeLength;

		public string PlayerName { get; private set; }

		public BridgeTimer BridgeTimer { get; private set; }
		public BridgeTrigger BridgeTrigger { get; private set; }
		public PlayerRunStatistics Statistics { get; private set; }
		
		public BridgeRunManager BridgeRunManager { get; private set; }
		
		private readonly Vector3 bridgeEnteredPosition;
		private Vector3 bridgeLeftPosition;

		private List<SideJump> jumps;
		private SideJump currentSideJump;

		public BridgeRun(BridgeTrigger trigger, string playerName, Vector3 enterPosition, PlayerRunStatistics statistics, BridgeRunManager bridgeRunManager)
		{
			BridgeTrigger = trigger;

			PlayerName = playerName;

			bridgeEnteredPosition = enterPosition;
			BridgeTimer           = new BridgeTimer();

			jumps = new List<SideJump>();

			Statistics = statistics;

			BridgeRunManager = bridgeRunManager;
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

		private void StartSideJump(Transform transform)
		{
			BridgeRunLogger.JumpedOffSide(PlayerName);
			currentSideJump = new SideJump(transform, this, BridgeTrigger, BridgeRunManager);
			
			BridgeTimer.SideJumpStarted();
		}

		public void EndSideJump(bool jumpSuccessful)
		{
			if (currentSideJump != null)
			{
				BridgeTimer.EndSideJump(jumpSuccessful);
				currentSideJump.EndJump(jumpSuccessful);

				if (jumpSuccessful)
				{
					jumps.Add(currentSideJump);
				}

				currentSideJump = null;
			}
		}

		public bool LeftBridgeTrigger(Vector3 bridgeExitPosition, Transform transform)
		{
			bool runStopped = false;

			bool leftPastA = bridgeExitPosition.z <= BridgeRunManager.TriggerBounds.Item1;
			bool leftPastB = bridgeExitPosition.z >= BridgeRunManager.TriggerBounds.Item2;
			
			if (leftPastA || leftPastB) // 2 is a reasonable distance that prevents walking off the bridge at the same position from counting as a side jump
			{
				bridgeLeftPosition = bridgeExitPosition;
				StopRun(false);
				
				runStopped = true;
			}
			else
			{
				StartSideJump(transform);
			}

			return runStopped;
		}

		private void LogInfo(bool fellOffBridge)
		{
			string runInfo = StatisticsCalculator.GetStatisticsString(bridgeEnteredPosition, bridgeLeftPosition, Statistics, this);
			
			BridgeRunLogger.EndRunStatistics(PlayerName, runInfo, fellOffBridge);

			float totalJumpTime = 0;
			float longestJumpTime = float.NegativeInfinity;
			float shortestJumpTime = float.PositiveInfinity;
			float totalHealthRegained = 0;
			
			for (int i = 0; i < jumps.Count;)
			{
				SideJump jump = jumps[i];
				jump.LogInfo(++i);

				float jumpTime = jump.JumpTimerValue;

				totalJumpTime       += jumpTime;
				totalHealthRegained += jump.HealthRegained;

				if (jumpTime > longestJumpTime)
				{
					longestJumpTime = jumpTime;
				}

				if (jumpTime < shortestJumpTime)
				{
					shortestJumpTime = jumpTime;
				}
			}
			
			BridgeRunLogger.AllJumpsStatistics(totalJumpTime, jumps.Count, longestJumpTime, shortestJumpTime, totalHealthRegained);
		}

		public void OnDestroy()
		{
			BridgeRunManager = null;
			
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