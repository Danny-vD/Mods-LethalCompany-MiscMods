using System;
using BepInEx.Logging;
using HarmonyLib;

namespace BridgeCalculator.Utils
{
	public static class PatchUtil
	{
		public static void PatchFunctions()
		{
			Harmony harmonyInstance = new Harmony(BridgeCalculatorPlugin.GUID);
			LoggerUtil.LogInfo("Attempting to patch with Harmony!");

			try
			{
				harmonyInstance.PatchAll(typeof(Patches));
				LoggerUtil.Log(LogLevel.Info, "Patching success!");
			}
			catch (Exception ex)
			{
				LoggerUtil.Log(LogLevel.Error, "Failed to patch: " + ex);
			}
		}
	}
}