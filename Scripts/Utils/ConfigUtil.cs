using BepInEx.Configuration;
using BepInEx.Logging;

namespace BridgeCalculator.Utils
{
	public static class ConfigUtil
	{
		// GENERAL
		public static ConfigEntry<LogLevel> LoggingLevel;
		
		// BRIDGE
		public static ConfigEntry<bool> ShouldBridgeLogHealth;
		public static ConfigEntry<bool> PrintHealthWhenNotOnBridge;

		private static ConfigFile config;

		internal static void Initialize(ConfigFile configFile)
		{
			config = configFile;
		}

		public static void ReadConfig()
		{
			// GENERAL
			LoggingLevel = config.Bind("0. General", "logLevel", LogLevel.Fatal | LogLevel.Error | LogLevel.Warning,
				"What should be logged?\nYou can seperate the options by a ',' to enable multiple\nValid options:\nNone, Fatal, Error, Warning, Message, Info, Debug, All");

			
			ShouldBridgeLogHealth = config.Bind("1. Bridge", "shouldBridgeLogHealth", true, "Should the bridge print his health?");
			PrintHealthWhenNotOnBridge = config.Bind("1. Bridge", "printHealthWhenNotOnBridge", false, "Should the bridge health be printed when not on the bridge?");
		}
	}
}