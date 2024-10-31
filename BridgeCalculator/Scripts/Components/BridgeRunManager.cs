using System;
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
		public float BridgeLength { get; private set; }

		public Tuple<float, float> TriggerBounds { get; private set; }

		public bool BridgeAllignsWithGlobalZ { get; private set; }

		private float belowBridgeThreshold;

		private readonly Dictionary<Collider, BridgeRun> currentRuns = new Dictionary<Collider, BridgeRun>();

		private readonly Dictionary<Collider, PlayerRunStatistics> statisticsMap = new Dictionary<Collider, PlayerRunStatistics>();

		private Collider triggerCollider;
		private BridgeTrigger bridgeTrigger;

		private void Awake()
		{
			BridgeAllignsWithGlobalZ = RoundManager.Instance.currentLevel.name.Contains("Vow");

			bridgeTrigger = GetComponent<BridgeTrigger>();

			triggerCollider = GetComponent<Collider>();
			Bounds triggerBounds = triggerCollider.bounds;

			float triggerMinimumPositionValue = 0;
			float triggerMaximumPositionValue = 0;

			if (BridgeAllignsWithGlobalZ)
			{
				BridgeLength = triggerBounds.extents.z * 2;

				triggerMinimumPositionValue = triggerBounds.center.z - triggerBounds.extents.z;
				triggerMaximumPositionValue = triggerBounds.center.z + triggerBounds.extents.z;
			}
			else // Bridge alligns with global X
			{
				BridgeLength = triggerBounds.extents.x * 2;

				triggerMinimumPositionValue = triggerBounds.center.x - triggerBounds.extents.x;
				triggerMaximumPositionValue = triggerBounds.center.x + triggerBounds.extents.x;
			}

			TriggerBounds = new Tuple<float, float>(triggerMinimumPositionValue, triggerMaximumPositionValue);

			belowBridgeThreshold = transform.position.y - 1;

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

					run = new BridgeRun(bridgeTrigger, playerControllerB.playerUsername, enterPosition, statistics, this);
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
					bool runstopped = run.LeftBridgeTrigger(bridgeLeftPosition, other.transform);

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

		public bool IsOutsideOfBridge(Vector3 position, out bool outsideSideA, out bool outsideSideB)
		{
			if (BridgeAllignsWithGlobalZ)
			{
				outsideSideA = position.z <= TriggerBounds.Item1;
				outsideSideB = position.z >= TriggerBounds.Item2;
			}
			else // Bridge alligns with global X
			{
				outsideSideA = position.x <= TriggerBounds.Item1;
				outsideSideB = position.x >= TriggerBounds.Item2;
			}

			return outsideSideA || outsideSideB;
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