using BridgeCalculator.BaseClasses;
using BridgeCalculator.Utils;
using UnityEngine;

namespace BridgeCalculator.Components
{
	public class BridgeHealthLogger : BetterMonoBehaviour
	{
		private BridgeTrigger bridgeTrigger;

		private bool IsPlayerOnBridge => playersCountOnBridge > 0;
		private bool damagedLastFrame = false;

		private int playersCountOnBridge = 0;

		private float timer;

		private void Awake()
		{
			bridgeTrigger = GetComponent<BridgeTrigger>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				++playersCountOnBridge;
			}
		}

		private void LateUpdate()
		{
			if (bridgeTrigger.bridgeDurability < 1)
			{
				timer -= Time.unscaledDeltaTime;

				if (timer > 0)
				{
					return;
				}

				timer = ConfigUtil.SecondsBetweenHealthLogs.Value;
				
				if (IsPlayerOnBridge || bridgeTrigger.giantOnBridge || ConfigUtil.PrintHealthWhenNotOnBridge.Value)
				{
					LoggerUtil.LogWarning($"Bridge health: {bridgeTrigger.bridgeDurability}");
				}

				damagedLastFrame = true;
			}
			else if (damagedLastFrame)
			{
				damagedLastFrame = false;
				LoggerUtil.LogWarning($"Bridge health: {bridgeTrigger.bridgeDurability}\n");
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				--playersCountOnBridge;
			}
		}
	}
}