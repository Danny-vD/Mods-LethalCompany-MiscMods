using BepInEx.Configuration;
using BepInEx.Logging;

namespace ExtraInformation.Utils
{
	public static class LoggerUtil
	{
		private static ManualLogSource logger;

		private static ConfigEntry<LogLevel> configLoggingLevel;

		private static LogLevel CurrentLogLevel => LogLevel.All; // configLoggingLevel.value

		public static bool IsLoggerEnabled => CurrentLogLevel > 0;

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
			if ((CurrentLogLevel & LogLevel.Message) == 0)
			{
				return;
			}
		
			Log(LogLevel.Message, data);
		}

		internal static void LogInfo(object data)
		{
			if ((CurrentLogLevel & LogLevel.Info) == 0)
			{
				return;
			}
		
			Log(LogLevel.Info, data);
		}
	
		internal static void LogDebug(object data)
		{
			if ((CurrentLogLevel & LogLevel.Debug) == 0)
			{
				return;
			}
		
			Log(LogLevel.Debug, data);
		}

		internal static void LogError(object data)
		{
			if ((CurrentLogLevel & LogLevel.Error) == 0)
			{
				return;
			}
		
			Log(LogLevel.Error, data);
		}

		internal static void LogWarning(object data)
		{
			if ((CurrentLogLevel & LogLevel.Warning) == 0)
			{
				return;
			}
		
			Log(LogLevel.Warning, data);
		}

		internal static void LogFatal(object data)
		{
			if ((CurrentLogLevel & LogLevel.Fatal) == 0)
			{
				return;
			}
		
			Log(LogLevel.Fatal, data);
		}
	}
}