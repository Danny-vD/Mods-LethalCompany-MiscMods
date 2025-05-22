using System;
using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace ExtraInformation.Dependencies.LobbyCompatibility
{
	internal static class LobbyCompatibilityUtils
	{
		internal const string LOBBY_COMPATIBILITY_GUID = global::LobbyCompatibility.PluginInfo.PLUGIN_GUID;
		
		internal static void RegisterMod()
		{
			PluginHelper.RegisterPlugin(ExtraInformationPlugin.GUID, new Version(ExtraInformationPlugin.PLUGIN_VERSION), CompatibilityLevel.ClientOnly, VersionStrictness.None);
		}
	}
}