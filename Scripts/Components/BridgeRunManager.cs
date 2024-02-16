using System.Collections.Generic;
using BridgeCalculator.BaseClasses;
using BridgeCalculator.BridgeTimer;
using BridgeCalculator.Events;
using BridgeCalculator.Utils;
using GameNetcodeStuff;
using UnityEngine;

namespace BridgeCalculator.Components
{
	public class BridgeRunManager : BetterMonoBehaviour
	{
		public static float BridgeLength = 0;

		private Dictionary<Collider, BridgeRun> currentRuns = new Dictionary<Collider, BridgeRun>();
		
		private Collider triggerCollider;
		private BridgeTrigger bridgeTrigger;

		private float fastestTime = float.MaxValue;
		private float longestTimeOnBridge = 0;

		private float shortestDistance = float.MaxValue;
		private float longestDistance = 0;

		private Vector3 bridgeEnteredPosition;

		private void Awake()
		{
			bridgeTrigger = GetComponent<BridgeTrigger>();

			triggerCollider = GetComponent<Collider>();
			BridgeLength    = triggerCollider.bounds.extents.z * 2;

			BridgeFallenEvent.OnBridgeFallen += StopAllRuns;
		}

		private void Update()
		{
			foreach (KeyValuePair<Collider, BridgeRun> pair in currentRuns)
			{
				pair.Value.Update();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Vector3 enterPosition = other.transform.position;

				if (CheckIfFellOffBridge(enterPosition))
				{
					FellOffBridge(other);
					return;
				}
				
				enterPosition.y = 0;

				if (currentRuns.TryGetValue(other, out BridgeRun run))
				{
					run.StopSideJump();
				}
				else
				{
					PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
					run = new BridgeRun(playerControllerB.playerUsername, enterPosition);

					currentRuns.Add(other, run);
				}

				//if (!startedTotalTimer)
				//{
				//	enterPosition.y = 0;
				//
				//	StartTotalTimer(enterPosition);
				//}
				//else if (startedSideTimer) // Should always be true in this case
				//{
				//	StopSideTimer(true);
				//}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Vector3 bridgeLeftPosition = other.transform.position;

				if (CheckIfFellOffBridge(bridgeLeftPosition))
				{
					FellOffBridge(other);
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

		private void StopAllRuns()
		{
			foreach (KeyValuePair<Collider, BridgeRun> pair in currentRuns)
			{
				pair.Value.FellOffBridge();
			}
			
			currentRuns.Clear();
		}

		private bool CheckIfFellOffBridge(Vector3 position)
		{
			float threshold = transform.position.y - 0.30f;

			bool fell = position.y < threshold;

			return fell;
		}

		private void FellOffBridge(Collider collider)
		{
			if (currentRuns.TryGetValue(collider, out BridgeRun run))
			{
				LoggerUtil.LogError($"{run.PlayerName} fell off bridge!\n");
				run.FellOffBridge();
				
				currentRuns.Remove(collider);
			}
		}

		public override void OnDestroy()
		{
			BridgeFallenEvent.OnBridgeFallen -= StopAllRuns;
			base.OnDestroy();
		}
	}
}