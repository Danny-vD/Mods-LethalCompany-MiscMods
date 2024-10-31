using System;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace ExtraInformation.Utils
{
	public static class ConfigUtil
	{
		// GENERAL
		public static ConfigEntry<LogLevel> LoggingLevel;

		private static ConfigFile config;

		internal static void Initialize(ConfigFile configFile)
		{
			config = configFile;

			config.SettingChanged += ReadConfig;
			config.ConfigReloaded += ReadConfig;
		}

		public static void ReadConfig()
		{
			// GENERAL
			LoggingLevel = config.Bind("0. General", "logLevel", LogLevel.All, 
				"What should be logged?\n" +
				"You can seperate the options by a ',' to enable multiple\n" +
				"Valid options:\n" +
				"None, Fatal, Error, Warning, Message, Info, Debug, All");
		}
		
		private static void ReadConfig(object sender, EventArgs e)
		{
			ReadConfig();
		}

		private static void ReadConfig(object sender, SettingChangedEventArgs e)
		{
			ReadConfig();
		}
	}
}