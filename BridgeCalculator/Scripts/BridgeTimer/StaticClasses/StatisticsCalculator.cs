using BridgeCalculator.Data;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer.StaticClasses
{
	public static class StatisticsCalculator
	{
		public static string GetStatisticsString(Vector3 bridgeEnteredPosition, Vector3 bridgeLeftPosition, PlayerRunStatistics statistics, BridgeRun run)
		{
			Vector3 perfectBridgeLeftPosition = bridgeLeftPosition;
			perfectBridgeLeftPosition.x = bridgeEnteredPosition.x;

			float perfectDistance = Vector3.Distance(bridgeEnteredPosition, perfectBridgeLeftPosition);

			float distance = Vector3.Distance(bridgeEnteredPosition, bridgeLeftPosition);

			bool disqualified = distance < run.AllowableFullLength;

			string timeMessage = string.Empty;

			float timerValue = run.BridgeTimer.TimerValue;

			if (!disqualified)
			{
				if (timerValue < statistics.FastestTimeAcross)
				{
					timeMessage                  = "[NEW RECORD]";
					statistics.FastestTimeAcross = timerValue;
				}

				if (distance < statistics.ShortestDistance)
				{
					statistics.ShortestDistance = distance;
				}
			}

			if (timerValue > statistics.LongestTimeOnBridge)
			{
				timeMessage                    = "[NEW RECORD]";
				statistics.LongestTimeOnBridge = timerValue;
			}

			if (distance > statistics.LongestDistance)
			{
				statistics.LongestDistance = distance;
			}

			int fastestTimeMinutes = (int)statistics.FastestTimeAcross / 60;
			float fastestTimeSeconds = statistics.FastestTimeAcross % 60;

			int longestTimeMinutes = (int)statistics.LongestTimeOnBridge / 60;
			float longestTimeSeconds = statistics.LongestTimeOnBridge % 60;

			return $"Time: {timerValue} seconds {timeMessage}\n" +
				   $"Fastest Time: {statistics.FastestTimeAcross} [{fastestTimeMinutes:00}:{fastestTimeSeconds:00.0000}]\n" +
				   $"Longest Time: {statistics.LongestTimeOnBridge} [{longestTimeMinutes:00}:{longestTimeSeconds:00.0000}]\n" +
				   $"Perfect Distance: {perfectDistance} | Bridge Length: {run.BridgeLength}\n" +
				   $"Distance: {distance} | Shortest: {statistics.ShortestDistance} | Longest: {statistics.LongestDistance}";
		}

		public static string GetSideJumpString(float jumpTime, float healthRegained, float bridgeHealth)
		{
			return $"Jump time: {jumpTime} seconds\nHealth regained: {healthRegained:P2}\nCurrent Health: {bridgeHealth:P2}";
		}
	}
}