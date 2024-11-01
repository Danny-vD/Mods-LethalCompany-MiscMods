﻿using BridgeCalculator.Utils;

namespace BridgeCalculator.BridgeTimer.StaticClasses
{
	public static class BridgeRunLogger
	{
		public static void EnteredBridge(string playerName)
		{
			LoggerUtil.LogError($"\n{playerName} entered bridge\n");
		}

		public static void JumpedOffSide(string playerName)
		{
			LoggerUtil.LogWarning($"{playerName} jumped off the side!\n");
		}

		public static void SuccessfulSideJump(string jumpInfo)
		{
			LoggerUtil.LogWarning($"Successful jump!\n{jumpInfo}\n");
		}

		public static void SideJumpStatistics(string jumpInfo, int jumpNumber)
		{
			LoggerUtil.LogWarning($"jump {jumpNumber}!\n{jumpInfo}\n");
		}

		public static void EndRunStatistics(string playerName, string runInfo, bool fellOffBridge)
		{	
			if (fellOffBridge)
			{
				LoggerUtil.LogError($"{playerName} fell off the bridge!\n{runInfo}\n");
			}
			else
			{
				LoggerUtil.LogError($"{playerName} left the bridge!\n{runInfo}\n");
			}
		}

		public static void AllJumpsStatistics(float totalJumpTime, int jumpCount, float longestJumpTime, float shortestJumpTime, float totalHealthRegained)
		{
			float averageJumpTime = totalJumpTime / jumpCount;
			
			LoggerUtil.LogError("Jump statistics:\n" +
								$"Total time: {totalJumpTime} seconds\n" +
								$"Longest jump: {longestJumpTime} seconds\n" +
								$"Shortest jump: {shortestJumpTime} seconds\n" +
								$"Average jump: {averageJumpTime} seconds\n" +
								$"Health regained: {totalHealthRegained}\n");
		}

		public static void LeftBridgeNoRun()
		{
			LoggerUtil.LogError("A player left the bridge but did not start a run\n");
		}
	}
}