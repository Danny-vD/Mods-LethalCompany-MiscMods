using System;
using System.Reflection;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;

namespace ExtraInformation.Utils
{
	public static class PatchUtil
	{
		public static void PatchFunctions()
		{
			Harmony harmonyInstance = new Harmony(ExtraInformationPlugin.GUID);
			LoggerUtil.LogInfo("Attempting to patch with Harmony!");

			try
			{
				harmonyInstance.PatchAll(typeof(Patches));
				PatchManually(harmonyInstance);
				
				LoggerUtil.Log(LogLevel.Info, "Patching success!"); // Using the Log function circumvents the configuration option, this is by design
			}
			catch (Exception ex)
			{
				LoggerUtil.Log(LogLevel.Error, "Failed to patch: " + ex);
			}
		}

		private static void PatchManually(Harmony harmonyInstance)
		{
			PatchExplicitInterface<EnemyAICollisionDetect, IHittable>(harmonyInstance, nameof(IHittable.Hit), nameof(ManualPatches.EnemyAICollisionDetectHitPatch));
			PatchExplicitInterface<PlayerControllerB, IHittable>(harmonyInstance, nameof(IHittable.Hit), nameof(ManualPatches.PlayerControllerBHitPatch));
		}

		private static void PatchExplicitInterface<TTypeToPatch, TInterface>(Harmony harmonyInstance,
			string                                                       methodName,
			string                                                       prefix        = null,
			string                                                       postfix       = null,
			string                                                       transpiler    = null,
			string                                                       finalizer     = null,
			string                                                       ilmanipulator = null) where TTypeToPatch : TInterface
		{
			MethodInfo method = typeof(TTypeToPatch).GetMethod(typeof(TInterface).Name + "." + methodName, BindingFlags.NonPublic | BindingFlags.Instance);

			HarmonyMethod prefixMethod = string.IsNullOrEmpty(prefix) ? null : new HarmonyMethod(typeof(ManualPatches), prefix);
			HarmonyMethod postfixMethod = string.IsNullOrEmpty(postfix) ? null : new HarmonyMethod(typeof(ManualPatches), postfix);
			HarmonyMethod transpilerMethod = string.IsNullOrEmpty(transpiler) ? null : new HarmonyMethod(typeof(ManualPatches), transpiler);
			HarmonyMethod finalizerMethod = string.IsNullOrEmpty(finalizer) ? null : new HarmonyMethod(typeof(ManualPatches), finalizer);
			HarmonyMethod ilmanipulatorMethod = string.IsNullOrEmpty(ilmanipulator) ? null : new HarmonyMethod(typeof(ManualPatches), ilmanipulator);

			harmonyInstance.Patch(method, prefixMethod, postfixMethod, transpilerMethod, finalizerMethod, ilmanipulatorMethod);
		}
	}
}