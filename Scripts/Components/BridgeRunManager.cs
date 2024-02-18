using System.Collections.Generic;
using BridgeCalculator.BaseClasses;
using BridgeCalculator.BridgeTimer;
using BridgeCalculator.BridgeTimer.StaticClasses;
using BridgeCalculator.Data;
using BridgeCalculator.Events;
using BridgeCalculator.Utils;
using GameNetcodeStuff;
using UnityEngine;

namespace BridgeCalculator.Components
{
	public class BridgeRunManager : BetterMonoBehaviour
	{
		public float BridgeLength = 0;

		private float belowBridgeThreshold;

		private readonly Dictionary<Collider, BridgeRun> currentRuns = new Dictionary<Collider, BridgeRun>();

		private readonly Dictionary<Collider, PlayerRunStatistics> statisticsMap = new Dictionary<Collider, PlayerRunStatistics>();
		
		private Collider triggerCollider;
		private BridgeTrigger bridgeTrigger;

		private void Awake()
		{
			bridgeTrigger = GetComponent<BridgeTrigger>();

			triggerCollider = GetComponent<Collider>();
			BridgeLength    = triggerCollider.bounds.extents.z * 2;

			belowBridgeThreshold = transform.position.y - 2;

			BridgeFallenEvent.OnBridgeFallen += BridgeCollapsed;
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
					// If a run already started and they entered the trigger, means that they side-jumped off and managed to get back on
					run.EndSideJump(true);
				}
				else
				{
					PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();

					if (!statisticsMap.TryGetValue(other, out PlayerRunStatistics statistics))
					{
						statistics = new PlayerRunStatistics();
						statisticsMap.Add(other, statistics);
					}
					
					run = new BridgeRun(bridgeTrigger, playerControllerB.playerUsername, enterPosition, statistics, BridgeLength);
					BridgeRunLogger.EnteredBridge(playerControllerB.playerUsername);

					currentRuns.Add(other, run);
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
					FellOffBridge(other);
					return;
				}

				bridgeLeftPosition.y = 0;

				if (currentRuns.TryGetValue(other, out BridgeRun run))
				{
					bool runstopped = run.LeftBridgeTrigger(bridgeLeftPosition);

					if (runstopped)
					{
						currentRuns.Remove(other);
					}
				}
				else
				{
					// Technically should never happen
					BridgeRunLogger.LeftBridgeNoRun();
				}
			}
		}

		private void BridgeCollapsed()
		{
			foreach (KeyValuePair<Collider, BridgeRun> pair in currentRuns)
			{
				pair.Value.StopRun(true);
			}
			
			currentRuns.Clear();
		}

		private bool CheckIfFellOffBridge(Vector3 position)
		{
			//LoggerUtil.LogWarning($"TRY TO FIND LOWEST POSSIBLE THRESHOLD\nThreshold: {belowBridgeThreshold}\nPosition: [{position.y}]\n"); //TODO REMOVE

			bool fell = position.y < belowBridgeThreshold;

			return fell;
		}

		private void FellOffBridge(Collider collider)
		{
			if (currentRuns.Remove(collider, out BridgeRun run))
			{
				run.StopRun(true);
			}
		}

		public override void OnDestroy()
		{
			foreach (KeyValuePair<Collider, BridgeRun> pair in currentRuns)
			{
				pair.Value.OnDestroy();
			}
			
			currentRuns.Clear();
			BridgeFallenEvent.OnBridgeFallen -= BridgeCollapsed;
			base.OnDestroy();
		}
	}
}