// ReSharper disable UnusedMember.Global // False positive, HarmonyX uses these to patch
// ReSharper disable InconsistentNaming // While true, Harmony wants these specific names

using System.Collections.Generic;
using System.Linq;
using System.Text;
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

// HUD PATCHES

		[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Update)), HarmonyPostfix]
		internal static void HUDManagerUpdatePatch(HUDManager __instance)
		{
			float carryWeight = GameNetworkManager.Instance.localPlayerController.carryWeight;

			float weightPounds = Mathf.Clamp(carryWeight - 1f, 0f, 100f) * 105f;
			__instance.weightCounter.text = $"{weightPounds:0.###} lb ({carryWeight})";
		}

// INVINCIBILITY

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer)), HarmonyPrefix]
		internal static bool PlayerControllerBDamagePlayerPatch(PlayerControllerB __instance)
		{
			return !ConfigUtil.ShouldPlayerBeInvincible.Value; // Skips the original method
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

// LOOT CHANCES

		[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ChangeLevel)), HarmonyPostfix]
		internal static void StartOfRoundChangeLevelPatch(StartOfRound __instance)
		{
			SelectableLevel currentLevel = __instance.currentLevel;
			
			StringBuilder stringBuilder = new StringBuilder($"Scrap chances on {currentLevel.PlanetName}:\n");
			
			List<SpawnableItemWithRarity> spawnableScrap = currentLevel.spawnableScrap;
			
			int totalWeight = spawnableScrap.Sum(itemWithRarity => itemWithRarity.rarity);
			float scrapAmountMultiplier = RoundManager.Instance.scrapAmountMultiplier;

			stringBuilder.AppendLine($"total weight: {totalWeight}\nScrap in level: {currentLevel.minScrap * scrapAmountMultiplier} - {currentLevel.maxScrap * scrapAmountMultiplier}\n");
			
			spawnableScrap = spawnableScrap.OrderByDescending(pair => pair.rarity / (float)totalWeight).ToList();
			
			foreach (SpawnableItemWithRarity itemWithRarity in spawnableScrap)
			{
				ScrapChanceCalculator.GetScrapChance(itemWithRarity, totalWeight, currentLevel, out float spawnChance, out float minLevelChance, out float maxLevelChance);
				
				string levelMinPercentage = $"{minLevelChance:P}";
				string levelMaxPercentage = $"{maxLevelChance:P}";

				stringBuilder.AppendLine($"{spawnChance:P} {itemWithRarity.spawnableItem.itemName} {{{levelMinPercentage} - {levelMaxPercentage}}} [{itemWithRarity.rarity}]");
			}

			stringBuilder.AppendLine();
			
			LoggerUtil.LogError(stringBuilder.ToString());
		}
	}
}