using System;
using BepInEx.Logging;
using HarmonyLib;

namespace MonsterSoda.Utils.ModUtil
{
	public static class PatchUtil
	{
		public static void PatchFunctions()
		{
			Harmony harmonyInstance = new Harmony(MonsterSodaPlugin.GUID);
			LoggerUtil.LogInfo("Attempting to patch with Harmony!");

			try
			{
				harmonyInstance.PatchAll(typeof(Patches));
				LoggerUtil.Log(LogLevel.Info, "Patching success!"); // Using the Log function circumvents the configuration option, this is by design
			}
			catch (Exception ex)
			{
				LoggerUtil.Log(LogLevel.Error, "Failed to patch: " + ex);
			}
		}
	}
}