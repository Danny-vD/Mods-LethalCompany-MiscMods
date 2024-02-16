using BridgeCalculator.BaseClasses;
using BridgeCalculator.Utils;
using UnityEngine;

namespace BridgeCalculator.Components
{
	public class BridgeHealthLogger : BetterMonoBehaviour
	{
		private BridgeTrigger bridgeTrigger;

		private bool playerOnBridge = false;
		private bool damagedLastFrame = false;

		private void Awake()
		{
			bridgeTrigger = GetComponent<BridgeTrigger>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				playerOnBridge = true;
			}
		}

		private void LateUpdate()
		{
			if (bridgeTrigger.bridgeDurability < 1)
			{
				if (playerOnBridge || bridgeTrigger.giantOnBridge || ConfigUtil.PrintHealthWhenNotOnBridge.Value)
				{
					LoggerUtil.LogWarning("Bridge health: " + bridgeTrigger.bridgeDurability);
					damagedLastFrame = true;
				}
			}
			else if (damagedLastFrame)
			{
				damagedLastFrame = false;
				LoggerUtil.LogWarning("Bridge health: " + bridgeTrigger.bridgeDurability);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				playerOnBridge = false;
			}
		}
	}
}