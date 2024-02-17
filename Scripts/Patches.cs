// ReSharper disable UnusedMember.Global // False positive, HarmonyX uses these to patch
// ReSharper disable InconsistentNaming // While true, Harmony wants these specific names

using BridgeCalculator.Components;
using BridgeCalculator.Events;
using BridgeCalculator.Utils;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace BridgeCalculator
{
	internal static class Patches
	{
		private static int originalHealth;
		
		[HarmonyPatch(typeof(BridgeTrigger), nameof(BridgeTrigger.OnEnable)), HarmonyPostfix]
		internal static void BridgeTriggerOnEnablePatch(BridgeTrigger __instance)
		{
			GameObject bridgeTriggerObj = __instance.gameObject;

			bridgeTriggerObj.AddComponent<BridgeRunManager>();

			if (ConfigUtil.ShouldBridgeReset.Value)
			{
				bridgeTriggerObj.AddComponent<BridgeResetter>();
			}

			if (ConfigUtil.ShouldBridgeLogHealth.Value)
			{
				bridgeTriggerObj.AddComponent<BridgeHealthLogger>();
			}

			if (ConfigUtil.StopTimeFromPassing.Value)
			{
				Object.FindAnyObjectByType<TimeOfDay>().enabled = false;
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

			float weightPounds = Mathf.Clamp(carryWeight - 1f, 0f, 100f) * 105f;
			__instance.weightCounter.text = $"{weightPounds} lb ({carryWeight})";
		}

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Start)), HarmonyPostfix]
		internal static void PlayerControllerBStartPatch(PlayerControllerB __instance)
		{
			originalHealth = __instance.health;
		}

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer)), HarmonyPostfix]
		internal static void PlayerControllerBDamagePlayerPatch(PlayerControllerB __instance)
		{
			if (ConfigUtil.ShouldPlayerBeInvincible.Value)
			{
				__instance.health = originalHealth;
			}
		}
	}
}