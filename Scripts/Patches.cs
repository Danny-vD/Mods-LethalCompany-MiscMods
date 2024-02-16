// ReSharper disable UnusedMember.Global // False positive, HarmonyX uses these to patch
// ReSharper disable InconsistentNaming // While true, Harmony wants these specific names

using BridgeCalculator.Components;
using BridgeCalculator.Events;
using BridgeCalculator.Utils;
using HarmonyLib;
using UnityEngine;

namespace BridgeCalculator
{
	internal static class Patches
	{
		[HarmonyPatch(typeof(BridgeTrigger), nameof(BridgeTrigger.OnEnable)), HarmonyPostfix]
		internal static void BridgeTriggerOnEnablePatch(BridgeTrigger __instance)
		{
			GameObject bridgeTriggerObj = __instance.gameObject;

			bridgeTriggerObj.AddComponent<BridgeTimerManager>();
			bridgeTriggerObj.AddComponent<BridgeResetter>();

			if (ConfigUtil.ShouldBridgeLogHealth.Value)
			{
				bridgeTriggerObj.AddComponent<BridgeHealthLogger>();
			}
		}

		[HarmonyPatch(typeof(BridgeTrigger), nameof(BridgeTrigger.BridgeFallClientRpc)), HarmonyPostfix]
		internal static void BridgeTriggerBridgeFallClientRpcPatch(BridgeTrigger __instance)
		{
			BridgeFallenEvent.RaiseEvent();
		}
		
		[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Update)), HarmonyPostfix]
		internal static void HUDManagerUpdatePatch(HUDManager __instance)
		{
			float carryWeight = GameNetworkManager.Instance.localPlayerController.carryWeight;

			float weightPounds = Mathf.RoundToInt(Mathf.Clamp(carryWeight - 1f, 0f, 100f) * 105f);
			__instance.weightCounter.text = $"{weightPounds} lb ({carryWeight})";
		}
	}
}