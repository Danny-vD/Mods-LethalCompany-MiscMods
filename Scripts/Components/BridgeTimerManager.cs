using BridgeCalculator.BaseClasses;
using BridgeCalculator.Events;
using BridgeCalculator.Utils;
using UnityEngine;

namespace BridgeCalculator.Components
{
	public class BridgeTimerManager : BetterMonoBehaviour
	{
		private float DisqualificationDistance => bridgeLength - 1; // BridgeLength - 1 to allow jumping off at the side at the end/beginning
		
		private Collider triggerCollider;
		private BridgeTrigger bridgeTrigger;

		private bool startedTotalTimer = false;
		private bool startedSideTimer = false;

		private float jumpStartedDurability = 0;

		private float bridgeTotalTimer;
		private float bridgeSideTimer;

		private float fastestTime = float.MaxValue;
		private float longestTimeOnBridge = 0;

		private float shortestDistance = float.MaxValue;
		private float longestDistance = 0;

		private Vector3 bridgeEnteredPosition;

		private float bridgeLength = 0;

		private void Awake()
		{
			bridgeTrigger = GetComponent<BridgeTrigger>();

			triggerCollider = GetComponent<Collider>();
			bridgeLength    = triggerCollider.bounds.extents.z * 2;

			BridgeFallenEvent.OnBridgeFallen += StopTimer;
		}

		private void Update()
		{
			if (startedTotalTimer)
			{
				bridgeTotalTimer += Time.unscaledDeltaTime;
			}

			if (startedSideTimer)
			{
				bridgeSideTimer += Time.unscaledDeltaTime;

				if (bridgeSideTimer > 5f) // Fell off the bridge
				{
					FellOffBridge();
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Vector3 enterPosition = other.transform.position;

				if (CheckIfFellOffBridge(enterPosition))
				{
					FellOffBridge();
					return;
				}

				if (!startedTotalTimer)
				{
					enterPosition.y = 0;

					StartTotalTimer(enterPosition);
				}
				else if (startedSideTimer) // Should always be true in this case
				{
					StopSideTimer(true);
				}
				else
				{
					LoggerUtil.LogError("It is false afterall!!!\n");
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Vector3 bridgeLeftPosition = other.transform.position;

				if (CheckIfFellOffBridge(bridgeLeftPosition))
				{
					FellOffBridge();
					return;
				}

				bridgeLeftPosition.y = 0;

				float distance = Vector3.Distance(bridgeEnteredPosition, bridgeLeftPosition);
				
				if (distance < DisqualificationDistance && distance > 2f)
				{
					StartSideTimer();
				}
				else
				{
					StopTotalTimer(bridgeLeftPosition);
				}
			}
		}

		private void StartTotalTimer(Vector3 enterPosition)
		{
			LoggerUtil.LogError("Player entered bridge\n");

			bridgeTotalTimer  = 0;
			startedTotalTimer = true;

			bridgeEnteredPosition = enterPosition;
		}

		private void StopTotalTimer(Vector3 exitPosition, bool printResult = true)
		{
			startedTotalTimer = false;
			
			if (!printResult)
			{
				return;
			}
			
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

		private void StartSideTimer()
		{
			LoggerUtil.LogWarning("Jumped off side!\n");

			jumpStartedDurability = bridgeTrigger.bridgeDurability;

			bridgeSideTimer  = 0;
			startedSideTimer = true;
		}

		private void StopSideTimer(bool success)
		{
			startedSideTimer = false;

			if (success)
			{
				float healthRegained = bridgeTrigger.bridgeDurability - jumpStartedDurability;
				
				LoggerUtil.LogWarning(
					$"Successful jump!\nJump time: {bridgeSideTimer} seconds\nHealth regained: {healthRegained * 100}%\nCurrent Health: {bridgeTrigger.bridgeDurability * 100}%\n");
			}
		}

		private void StopTimer()
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

		private bool CheckIfFellOffBridge(Vector3 position)
		{
			float threshold = transform.position.y - 0.30f;

			bool fell = position.y < threshold;

			return fell;
		}

		private void FellOffBridge()
		{
			LoggerUtil.LogError("Fell off bridge!\n");
			StopTotalTimer(bridgeEnteredPosition, false);
			StopSideTimer(false);
		}

		public override void OnDestroy()
		{
			BridgeFallenEvent.OnBridgeFallen -= StopTimer;
			base.OnDestroy();
		}
	}
}