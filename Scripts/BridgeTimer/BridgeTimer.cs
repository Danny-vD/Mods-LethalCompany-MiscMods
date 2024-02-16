using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeTimer
	{
		private const float maximumSideTimer = 5f;
		
		private bool startedTotalTimer = false;
		private bool startedSideTimer = false;
		
		private float bridgeTotalTimer;
		private float bridgeSideTimer;

		public BridgeTimer(Vector3 enterPosition)
		{
			StartTotalTimer(enterPosition);
		}

		public void Update()
		{
			if (startedTotalTimer)
			{
				bridgeTotalTimer += Time.unscaledDeltaTime;
			}

			if (startedSideTimer)
			{
				bridgeSideTimer += Time.unscaledDeltaTime;

				if (bridgeSideTimer > maximumSideTimer) // Fell off the bridge
				{
					FellOffBridge(); //TODO add event
				}
			}
		}

		private void StartTotalTimer(Vector3 enterPosition)
		{
			bridgeTotalTimer  = 0;
			startedTotalTimer = true;

			bridgeEnteredPosition = enterPosition;
		}
		
		public void StopTotalTimer(Vector3 exitPosition, bool printResult = true)
		{
			startedTotalTimer = false;
			
			if (!printResult)
			{
				return;
			}
			
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
		}
		
		public void StartSideTimer()
		{
			LoggerUtil.LogWarning("Jumped off side!\n");

			jumpStartedDurability = bridgeTrigger.bridgeDurability;

			bridgeSideTimer  = 0;
			startedSideTimer = true;
		}

		public void StopSideTimer(bool success)
		{
			startedSideTimer = false;

			if (success)
			{
				float healthRegained = bridgeTrigger.bridgeDurability - jumpStartedDurability;
				
				LoggerUtil.LogWarning(
					$"Successful jump!\nJump time: {bridgeSideTimer} seconds\nHealth regained: {healthRegained * 100}%\nCurrent Health: {bridgeTrigger.bridgeDurability * 100}%\n");
			}
		}
		
		public void StopAllTimers()
		{
			if (startedSideTimer)
			{
				StopSideTimer(false);
			}

			if (startedTotalTimer)
			{
				StopTotalTimer(bridgeEnteredPosition);
			}
		}
	}
}