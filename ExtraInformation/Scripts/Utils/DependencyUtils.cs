using BepInEx.Bootstrap;
using ExtraInformation.Dependencies.LobbyCompatibility;

namespace ExtraInformation.Utils
{
	public static class DependencyUtils
	{
		public static bool LethalConfigPresent { get; private set; }
		public static bool LobbyCompatibilityPresent { get; private set; }

		internal static void CheckDependencies()
		{
			CheckSoftDependencies();
		}

		private static void CheckSoftDependencies()
		{
			if (Chainloader.PluginInfos.TryGetValue(LobbyCompatibilityUtils.LOBBY_COMPATIBILITY_GUID, out BepInEx.PluginInfo pluginInfo))
			{
				LoggerUtil.LogInfo($"{pluginInfo.Metadata.Name} has been found!\nRegistering {ExtraInformationPlugin.PLUGIN_NAME}...");

				LobbyCompatibilityPresent = true;
				LobbyCompatibilityUtils.RegisterMod();
			}
		}
	}
}