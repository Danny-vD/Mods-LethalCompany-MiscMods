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
		}

		[HarmonyPatch(typeof(BridgeTrigger), nameof(BridgeTrigger.BridgeFallClientRpc)), HarmonyPostfix]
		internal static void BridgeTriggerBridgeFallClientRpcPatch(BridgeTrigger __instance)
		{
			BridgeFallenEvent.RaiseEvent();
		}

// INVINCIBILITY

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer)), HarmonyPrefix]
		internal static bool PlayerControllerBDamagePlayerPatch(PlayerControllerB __instance)
		{
			if (ConfigUtil.ShouldPlayerBeInvincible.Value)
			{
				__instance.takingFallDamage = false;
				return false; // Skips the original method
			}

			return true;
		}
		
		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath)), HarmonyPrefix]
		internal static bool PlayerControllerBAllowPlayerDeathPatch(PlayerControllerB __instance, ref bool __result)
		{
			if (ConfigUtil.ShouldPlayerBeInvincible.Value)
			{
				__result = false;
				return false; // Skips the original method
			}

			return true;
		}

// TIME STOPPED

		[HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Update)), HarmonyPrefix]
		internal static void TimeOfDayUpdatePatch(TimeOfDay __instance)
		{
			if (ConfigUtil.StopTimeFromPassing.Value)
			{
				__instance.globalTime = 365f;
			}
		}
	}
}