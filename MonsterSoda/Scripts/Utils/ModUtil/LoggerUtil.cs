using BepInEx.Configuration;
using BepInEx.Logging;

namespace MonsterSoda.Utils.ModUtil
{
	public static class LoggerUtil
	{
		private static ManualLogSource logger;

		private static ConfigEntry<LogLevel> configLoggingLevel;

		public static bool IsLoggerEnabled => true; //configLoggingLevel.Value > 0;

		internal static void Initialize(ConfigEntry<LogLevel> loggingLevelEntry, ManualLogSource logSource)
		{
			configLoggingLevel = loggingLevelEntry;

			logger = logSource;
		}

		internal static void Log(LogLevel logLevel, object data)
		{
			if (!IsLoggerEnabled)
			{
				return;
			}
		
			logger.Log(logLevel, data);
		}
	
		internal static void LogMessage(object data)
		{
			//if ((configLoggingLevel.Value & LogLevel.Message) == 0)
			//{
			//	return;
			//}
		
			Log(LogLevel.Message, data);
		}

		internal static void LogInfo(object data)
		{
			//if ((configLoggingLevel.Value & LogLevel.Info) == 0)
			//{
			//	return;
			//}
		
			Log(LogLevel.Info, data);
		}
	
		internal static void LogDebug(object data)
		{
			//if ((configLoggingLevel.Value & LogLevel.Debug) == 0)
			//{
			//	return;
			//}
		
			Log(LogLevel.Debug, data);
		}

		internal static void LogError(object data)
		{
			//if ((configLoggingLevel.Value & LogLevel.Error) == 0)
			//{
			//	return;
			//}
		
			Log(LogLevel.Error, data);
		}

		internal static void LogWarning(object data)
		{
			//if ((configLoggingLevel.Value & LogLevel.Warning) == 0)
			//{
			//	return;
			//}
		
			Log(LogLevel.Warning, data);
		}

		internal static void LogFatal(object data)
		{
			//if ((configLoggingLevel.Value & LogLevel.Fatal) == 0)
			//{
			//	return;
			//}
		
			Log(LogLevel.Fatal, data);
		}
	}
}