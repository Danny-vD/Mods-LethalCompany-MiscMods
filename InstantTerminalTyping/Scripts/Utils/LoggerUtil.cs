using BepInEx.Logging;

namespace InstantTerminalTyping.Utils
{
	public static class LoggerUtil
	{
		private static ManualLogSource logger;

		internal static void Initialize(ManualLogSource logSource)
		{
			logger = logSource;
		}

		internal static void Log(LogLevel logLevel, object data)
		{
			logger.Log(logLevel, data);
		}
	}
}