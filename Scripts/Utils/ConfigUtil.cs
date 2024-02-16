using BepInEx.Configuration;
using BepInEx.Logging;
using BridgeCalculator.Components;

namespace BridgeCalculator.Utils
{
	public static class ConfigUtil
	{
		// GENERAL
		public static ConfigEntry<LogLevel> LoggingLevel;
		
		// BRIDGE
		public static ConfigEntry<bool> ShouldBridgeLogHealth;
		public static ConfigEntry<bool> PrintHealthWhenNotOnBridge;
		public static ConfigEntry<bool> ShouldBridgeReset;

		private static ConfigFile config;

		internal static void Initialize(ConfigFile configFile)
		{
			config = configFile;
		}

		public static void ReadConfig()
		{
			// GENERAL
			LoggingLevel = config.Bind("0. General", "logLevel", LogLevel.Fatal | LogLevel.Error | LogLevel.Warning, 
				"What should be logged?\n" +
				"You can seperate the options by a ',' to enable multiple\n" +
				"Valid options:\n" +
				"None, Fatal, Error, Warning, Message, Info, Debug, All");
			
			ShouldBridgeLogHealth      = config.Bind("1. Bridge", "shouldBridgeLogHealth", true, "Should the bridge print his health?");
			PrintHealthWhenNotOnBridge = config.Bind("1. Bridge", "printHealthWhenNotOnBridge", false, "Should the bridge health be printed when not on the bridge?");
			ShouldBridgeReset          = config.Bind("1. Bridge", "shouldBridgeReset", true, $"If true, the bridge will restore itself after {BridgeResetter.RESET_TIME} seconds");
		}
	}
}