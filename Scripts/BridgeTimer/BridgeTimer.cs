using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeTimer
	{
		private bool startedTotalTimer = false;

		private float bridgeTotalTimer;

		public BridgeTimer()
		{
			StartTimer();
		}

		public void Update()
		{
			if (startedTotalTimer)
			{
				bridgeTotalTimer += Time.unscaledDeltaTime;
			}
		}

		private void StartTimer()
		{
			bridgeTotalTimer  = 0;
			startedTotalTimer = true;

			//bridgeEnteredPosition = enterPosition;
		}
		
		public void StopTimer()
		{
			if (startedTotalTimer)
			{
				startedTotalTimer = false;
			}
		}

		public void StopTotalTimer()
		{
			/*
			// TODO: move out of this function (move to BridgeRun?)
			Vector3 bridgeLeftPosition = exitPosition;
			bridgeLeftPosition.y = 0;

			Vector3 perfectBridgeLeftPosition = bridgeLeftPosition;
			perfectBridgeLeftPosition.x = bridgeEnteredPosition.x;

			float perfectDistance = Vector3.Distance(bridgeEnteredPosition, perfectBridgeLeftPosition);

			float distance = Vector3.Distance(bridgeEnteredPosition, bridgeLeftPosition);

			bool disqualified = distance < DisqualificationDistance;

			string timeMessage = string.Empty;

			if (!disqualified)
			{
				if (fastestTime > bridgeTotalTimer)
				{
					timeMessage = "[NEW RECORD]";
					fastestTime = bridgeTotalTimer;
				}

				if (shortestDistance > distance)
				{
					shortestDistance = distance;
				}
			}

			if (longestTimeOnBridge < bridgeTotalTimer)
			{
				timeMessage         = "[NEW RECORD]";
				longestTimeOnBridge = bridgeTotalTimer;
			}

			if (longestDistance < distance)
			{
				longestDistance = distance;
			}

			int fastestTimeMinutes = (int)fastestTime / 60;
			float fastestTimeSeconds = fastestTime % 60;

			int longestTimeMinutes = (int)longestTimeOnBridge / 60;
			float longestTimeSeconds = longestTimeOnBridge % 60;

			LoggerUtil.LogError($"Player left bridge\n" +
								$"Time: {bridgeTotalTimer} seconds {timeMessage}\n" +
								$"Fastest Time: {fastestTime} [{fastestTimeMinutes:00}:{fastestTimeSeconds:00.0000}]\n" +
								$"Longest Time: {longestTimeOnBridge} [{longestTimeMinutes:00}:{longestTimeSeconds:00.0000}]\n" +
								$"Bridge Length: {bridgeLength}\tPerfect Distance: {perfectDistance}\n" +
								$"Distance: {distance} | Shortest: {shortestDistance} | Longest: {longestDistance}\n");

			*/
		}
	}
}