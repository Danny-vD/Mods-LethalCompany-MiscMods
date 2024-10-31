// ReSharper disable UnusedMember.Global // False positive, HarmonyX uses these to patch
// ReSharper disable InconsistentNaming // While true, Harmony wants these specific names

using System.Collections;
using HarmonyLib;
using MonsterSoda.Components;
using MonsterSoda.Components.TextureUpdaters;
using MonsterSoda.Constants;
using MonsterSoda.FunctionOverrides;
using MonsterSoda.PatchFunctions;
using MonsterSoda.Utils.ModUtil;
using UnityEngine;

namespace MonsterSoda
{
	internal static class Patches
	{
		[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start)), HarmonyPostfix]
		internal static void GrabbableObjectStartPatch(GrabbableObject __instance)
		{
			if (__instance.name.ToLower().Contains("soda"))
			{
				__instance.gameObject.AddComponent<SodaCanTextureUpdater>();

				__instance.gameObject.AddComponent<ObjectNameSetter>().SetName("Monsti Energy");
			}

			if (__instance.name.ToLower().Contains("robot") && __instance is AnimatedItem robotToy)
			{
				RobotToyFunctions.Start(robotToy, false);
				
				if (!RobotToyFunctions.DidSetPrefab)
				{
					RobotToyFunctions.SetPrefab(robotToy.itemProperties.spawnPrefab);
				}
			}
		}

		[HarmonyPatch(typeof(AnimatedItem), nameof(AnimatedItem.EquipItem)), HarmonyPrefix]
		internal static bool AnimatedItemEquipItemPatch(AnimatedItem __instance)
		{
			if (__instance.name.ToLower().Contains("robot"))
			{
				AnimatedItemRobotToyOverrides.EquipItem(__instance);

				return false; // Skips original method
			}

			return true;
		}

		[HarmonyPatch(typeof(AnimatedItem), nameof(AnimatedItem.DiscardItem)), HarmonyPostfix]
		internal static void AnimatedItemDiscardItemPatch(AnimatedItem __instance)
		{
			if (__instance.name.ToLower().Contains("robot"))
			{
				RobotToyFunctions.Discarditem(__instance);
			}
		}

		[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
		internal static void StartOfRoundStartpatch(StartOfRound __instance)
		{
			__instance.StartCoroutine(ChangePosters());
			return;

			IEnumerator ChangePosters()
			{
				yield return null; // Wait a couple frames (so that all Start() and Update() happened already)

				for (int i = 0; i < 24; i++)
				{
					yield return null;
				}

				ulong currentSteamID = __instance.localPlayerController.playerSteamId;
				
				if (currentSteamID != SteamUserIDs.EASTER_EGG && currentSteamID != SteamUserIDs.LEEK_ID)
				{
					yield break;
				}

				GameObject posters = GameObject.Find("Plane.001");

				if (ReferenceEquals(posters, null))
				{
					LoggerUtil.LogError("No posters found");
					yield break;
				}

				posters.AddComponent<PostersTextureUpdater>();
			}
		}
	}
}