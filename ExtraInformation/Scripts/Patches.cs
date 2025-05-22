// ReSharper disable UnusedMember.Global // False positive, HarmonyX uses these to patch
// ReSharper disable InconsistentNaming // While true, Harmony wants these specific names

using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraInformation.InfoLoggers;
using ExtraInformation.Utils;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace ExtraInformation
{
	internal static class Patches
	{
// HUD PATCHES

		[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.Update)), HarmonyPostfix]
		internal static void HUDManagerUpdatePatch(HUDManager __instance)
		{
			PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;

			if (localPlayerController == null)
			{
				return;
			}

			float carryWeight = localPlayerController.carryWeight;

			float weightPounds = (carryWeight - 1f) * 105f;

			StringBuilder stringBuilder = new StringBuilder($"{weightPounds:0.###} lb ({carryWeight})\n");

			HUDInfoShower.ShowValueCarrying(stringBuilder, localPlayerController);
			HUDInfoShower.ShowSpeed(stringBuilder, localPlayerController);

			//HUDInfoShower.ShowEggInformation(stringBuilder, localPlayerController);

			// NOTE: TEMP
			//HUDInfoShower.ShowImmortality(stringBuilder, localPlayerController);
			//HUDInfoShower.ShowFallDamage(stringBuilder, localPlayerController);

			//HUDInfoShower.ShowFallInformation(stringBuilder, localPlayerController);
			//HUDInfoShower.ShowTzpInformation(stringBuilder, localPlayerController);
			//NOTE END TEMP

			__instance.weightCounter.text = stringBuilder.ToString();
		}

// LOG LEVELS

		[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
		internal static void StartOfRoundStartPatch(StartOfRound __instance)
		{
			Dissonance.Logs.SetLogLevel(Dissonance.LogCategory.Core, Dissonance.LogLevel.Error);
			Dissonance.Logs.SetLogLevel(Dissonance.LogCategory.Network, Dissonance.LogLevel.Error);
			Dissonance.Logs.SetLogLevel(Dissonance.LogCategory.Playback, Dissonance.LogLevel.Error);
			Dissonance.Logs.SetLogLevel(Dissonance.LogCategory.Recording, Dissonance.LogLevel.Error);

			NetworkManager.Singleton.LogLevel = LogLevel.Error;
		}

// LOOT CHANCES

		[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ChangeLevel)), HarmonyPostfix]
		internal static void StartOfRoundChangeLevelPatch(StartOfRound __instance)
		{
			SelectableLevel currentLevel = __instance.currentLevel;

			//TODO: Add support for challenge file. See RoundManager.SetChallengeFileRandomModifiers()

			LoggerUtil.LogError("");
			LevelInfoLogger.LogDungeonFlowsOfLevel(currentLevel, out float levelSize);
			LevelInfoLogger.LogWeatherOfLevel(currentLevel);

#if DEVELOPER_MODE
			MapObjectInfoLogger.LogMapObjectsCurveData(currentLevel);
#endif

			EnemyInfoLogger.LogEnemyInfoOfLevel(currentLevel);
			ScrapInfoLogger.LogScrapInfoOfLevel(currentLevel, levelSize);
			LoggerUtil.LogError("");
		}

// LEVEL FINISHED LOADING

		[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.PredictAllOutsideEnemies)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
		internal static void PredictAllOutsideEnemiesPatch()
		{
			StartOfRound startOfRound = StartOfRound.Instance;
			LoggerUtil.LogInfo($"\nCurrent map seed: {startOfRound.randomMapSeed}");

			int turretsInLevel = Object.FindObjectsByType<Turret>(FindObjectsSortMode.None).Length;
			int minesInLevel = Object.FindObjectsByType<Landmine>(FindObjectsSortMode.None).Length;
			int spikeTrapsInLevel = Object.FindObjectsByType<SpikeRoofTrap>(FindObjectsSortMode.None).Length;

			MapObjectInfoLogger.GetMaximumMapObjects(startOfRound.currentLevel, out int maxTurrets, out int maxMines, out int maxSpikeTraps);

			LoggerUtil.LogWarning($"\nMapObjects:\nTurrets: {turretsInLevel}/{maxTurrets}\nMines: {minesInLevel}/{maxMines}\nSpike Traps: {spikeTrapsInLevel}/{maxSpikeTraps}");

#if !DEVELOPER_MODE
			foreach (KeyItem keyItem in Object.FindObjectsOfType<KeyItem>())
			{
				keyItem.SetScrapValue(0);
			}
#endif
		}

// QUOTA VARIABLES

		[HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
		internal static void SetNewProfitQuotaPatch(TimeOfDay __instance)
		{
			LoggerUtil.LogWarning($"Luck value: {__instance.luckValue}");
		}

		// [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SyncNewProfitQuotaClientRpc)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
		// internal static void SyncNewProfitQuotaClientRpcPatch()
		// {
		// 	List<UnlockableItem> unlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
		// 	float luck = Object.FindObjectsByType<AutoParentToShip>(FindObjectsSortMode.None).Sum(furniture => unlockableItems[furniture.unlockableID].luckValue);
		// 	LoggerUtil.LogWarning($"Luck value: {luck}\t(client)");
		// }

		[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SyncScrapValuesClientRpc)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
		internal static void SyncScrapValuesClientRpcPatch(TimeOfDay __instance)
		{
#if !DEVELOPER_MODE
			foreach (KeyItem keyItem in Object.FindObjectsOfType<KeyItem>())
			{
				keyItem.SetScrapValue(0);
			}
#endif
		}

// TEMPORARY PATCHES

/*
		[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.GetRandomWeightedIndex)), HarmonyPrefix]
		internal static bool RoundManagerGetRandomWeightedIndexPatch(RoundManager __instance, ref int __result, int[] weights, Random randomSeed = null)
		{
			if (randomSeed == null)
			{
				randomSeed = __instance.AnomalyRandom;
			}

			if (weights == null || weights.Length == 0)
			{
				LoggerUtil.LogError("Could not get random weighted index; array is empty or null.");
				__result = -1;
				return false;
			}

			int totalWeight = 0;

			for (int i = 0; i < weights.Length; i++)
			{
				if (weights[i] >= 0)
				{
					totalWeight += weights[i];
				}
			}

			if (totalWeight <= 0)
			{
				__result = randomSeed.Next(0, weights.Length);
				return false;
			}

			float threshold = (float)randomSeed.NextDouble();
			float currentTotal = 0f;

			__result = -1;

			for (int i = 0; i < weights.Length; i++)
			{
				if ((float)weights[i] > 0f)
				{
					//float normalizedRange = (float)weights[i] / (float)totalWeight;

					//LoggerUtil.LogWarning($"{currentTotal,-20} + {normalizedRange,-12} = ");
					currentTotal += (float)weights[i] / (float)totalWeight;

					LoggerUtil.LogWarning($"{currentTotal:N27}"); // NOTE: print range

					if (currentTotal >= threshold && __result == -1)
					{
						LoggerUtil.LogInfo($"Threshold reached! [{threshold:N15}]\tPicking index {i}: {weights[i]}");
						__result = i;
					}
				}
			}

			LoggerUtil.LogInfo("\n");

			if (__result != -1)
			{
				return false;
			}

			LoggerUtil.LogError($"No threshold reached! [{threshold:N15}]");

			LoggerUtil.LogError("Error while calculating random weighted index. Choosing randomly. Weights given:");

			for (int i = 0; i < weights.Length; i++)
			{
				LoggerUtil.LogError($"{weights[i]},");
			}

			__instance.InitializeRandomNumberGenerators();

			__result = randomSeed.Next(0, weights.Length);
			return false;
		}
		/**/

		/*
		[HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel)), HarmonyPostfix]
		internal static void ShovelHitShovelPatch(Shovel __instance, bool cancel)
		{
			if (cancel)
			{
				return;
			}

			bool hitAnything = false;
			StringBuilder builder = new StringBuilder("\nThese were hit by the shovel:\n");

			foreach (RaycastHit hit in __instance.objectsHitByShovelList)
			{
				int hittedLayer = hit.transform.gameObject.layer;

				if (hittedLayer is 8 or 11)
				{
					hitAnything = true;
					builder.AppendLine("• " + hit.collider.gameObject.name + " [played hitSurfaceSFX]");
				}
				else if (hit.transform.TryGetComponent(out IHittable _) &&
						 hit.transform != __instance.previousPlayerHeldBy.transform &&
						 (hit.point == Vector3.zero || !Physics.Linecast(__instance.previousPlayerHeldBy.gameplayCamera.transform.position, hit.point, out _, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
				{
					hitAnything = true;
					builder.AppendLine("• " + hit.collider.gameObject.name);
				}
			}

			if (!hitAnything)
			{
				builder.Clear();
				builder.AppendLine("\nShovel did not hit anything!");
			}

			builder.AppendLine("\n");

			LoggerUtil.LogError(builder.ToString());
		}

		[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemyOnLocalClient)), HarmonyPrefix]
		internal static void EnemyAIHitEnemyOnLocalClientPatch(EnemyAI __instance, PlayerControllerB playerWhoHit, int force)
		{
			if (playerWhoHit == null)
			{
				LoggerUtil.LogError("Hit by no one?");
			}

			LoggerUtil.LogError(__instance.GetType().Name + " hit on local client!");
		}

		[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemyServerRpc)), HarmonyPrefix]
		internal static void EnemyAIHitEnemyServerRpcPatch(EnemyAI __instance)
		{
			NetworkManager networkManager = __instance.NetworkManager;

			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}

			bool isServer = NetCodeUtil.IsRpcExecState(__instance, __RpcExecStage.Server);

			if (!isServer || !networkManager.IsServer && !networkManager.IsHost)
			{
				return;
			}

			LoggerUtil.LogError("Server hit " + __instance.GetType().Name);
		}

		[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemyClientRpc)), HarmonyPrefix]
		internal static void EnemyAIHitEnemyClientRpcPatch(EnemyAI __instance, int playerWhoHit)
		{
			NetworkManager networkManager = __instance.NetworkManager;

			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}

			bool isClient = NetCodeUtil.IsRpcExecState(__instance, __RpcExecStage.Client);

			if (!isClient || !networkManager.IsClient && !networkManager.IsHost)
			{
				return;
			}

			if (playerWhoHit == (int)GameNetworkManager.Instance.localPlayerController.playerClientId)
			{
				return;
			}

			LoggerUtil.LogError("Client hit " + __instance.GetType().Name);
		}

		[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemy)), HarmonyPrefix]
		internal static void EnemyAIHitEnemyPatch(EnemyAI __instance, int force)
		{
			LoggerUtil.LogError("Reduce health of enemy " + __instance.GetType().Name + "!\n\n");
		}

		[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy)), HarmonyPostfix]
		internal static void EnemyAIKillEnemyPatch(EnemyAI __instance)
		{
			LoggerUtil.LogError(__instance.GetType().Name + " died!\n\n\n\n");
		}

		// PLAYER HIT

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayerFromOtherClientServerRpc)), HarmonyPrefix]
		internal static void PlayerControllerBDamagePlayerFromOtherClientServerRpcPatch(PlayerControllerB __instance)
		{
			NetworkManager networkManager = __instance.NetworkManager;

			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}

			bool isServer = NetCodeUtil.IsRpcExecState(__instance, __RpcExecStage.Server);

			if (!isServer || !networkManager.IsServer && !networkManager.IsHost)
			{
				return;
			}

			LoggerUtil.LogError("Server hit " + __instance.playerUsername);
		}

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayerFromOtherClientClientRpc)), HarmonyPrefix]
		internal static void PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch(PlayerControllerB __instance)
		{
			NetworkManager networkManager = __instance.NetworkManager;

			if (networkManager == null || !networkManager.IsListening)
			{
				return;
			}

			bool isClient = NetCodeUtil.IsRpcExecState(__instance, __RpcExecStage.Client);

			if (!isClient || !networkManager.IsClient && !networkManager.IsHost)
			{
				return;
			}

			if (!__instance.AllowPlayerDeath())
			{
				return;
			}

			LoggerUtil.LogError("Client hit " + __instance.playerUsername);
		}
		*/
	}

	internal static class ManualPatches
	{
		//[HarmonyPatch(typeof(EnemyAICollisionDetect), nameof(EnemyAICollisionDetect.Hit)), HarmonyPrefix]
		internal static void EnemyAICollisionDetectHitPatch(EnemyAICollisionDetect __instance)
		{
			if (__instance.onlyCollideWhenGrounded)
			{
				LoggerUtil.LogError("Hit prevented on " + __instance.mainScript.GetType().Name + $" [{__instance.name}]");
				return;
			}

			LoggerUtil.LogError("Hit detected on " + __instance.mainScript.GetType().Name + $" [{__instance.name}]");
		}

		//[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Hit)), HarmonyPrefix]
		internal static void PlayerControllerBHitPatch(PlayerControllerB __instance, int force, PlayerControllerB playerWhoHit)
		{
			if (!__instance.AllowPlayerDeath())
			{
				return;
			}

			if (__instance.inAnimationWithEnemy)
			{
				return;
			}

			StringBuilder builder = new StringBuilder("Player " + __instance.playerUsername + $" was hit with {force} force by {playerWhoHit.playerUsername}!\n");

			int damage = force switch
			{
				<= 2 => 10,
				<= 4 => 30,
				_ => 100,
			};

			builder.AppendLine($"This did {damage} damage");
			LoggerUtil.LogError(builder.ToString());
		}
	}
}