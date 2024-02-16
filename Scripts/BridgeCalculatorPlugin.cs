using BepInEx;
using BridgeCalculator.Utils;
using Dissonance;

namespace BridgeCalculator
{
	[BepInPlugin(GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	[BepInProcess("Lethal Company.exe")]
	public class BridgeCalculatorPlugin : BaseUnityPlugin
	{
		public const string GUID = $"DannyVD.mods.LethalCompany.{PLUGIN_NAME}";
		public const string PLUGIN_NAME = "BridgeCalculator";
		public const string PLUGIN_VERSION = "1.0.0";
		
		public const string DEPENDENCY_STRING = $"DannyVD-{PLUGIN_NAME}-{PLUGIN_VERSION}";

		private void Awake()
		{
			ConfigUtil.Initialize(Config);
			ConfigUtil.ReadConfig();
			
			LoggerUtil.Initialize(ConfigUtil.LoggingLevel, Logger);

			// Plugin startup logic
			LoggerUtil.Log(BepInEx.Logging.LogLevel.Info, $"Plugin {DEPENDENCY_STRING} is loaded!"); // Using the Log function circumvents the configuration option, this is by design

			PatchUtil.PatchFunctions();
			
			Logs.SetLogLevel(LogCategory.Core, Dissonance.LogLevel.Error);
			Logs.SetLogLevel(LogCategory.Network, Dissonance.LogLevel.Error);
			Logs.SetLogLevel(LogCategory.Playback, Dissonance.LogLevel.Error);
			Logs.SetLogLevel(LogCategory.Recording, Dissonance.LogLevel.Error);
		}
	}
}