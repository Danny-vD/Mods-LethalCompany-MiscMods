using BepInEx;
using BepInEx.Logging;
using ExtraInformation.Utils;

namespace ExtraInformation
{
	[BepInPlugin(GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	[BepInProcess("Lethal Company.exe")]
	public class ExtraInformationPlugin : BaseUnityPlugin
	{
		public const string GUID = $"DannyVD.mods.LethalCompany.{PLUGIN_NAME}";
		public const string PLUGIN_NAME = "ExtraInformation";
		public const string PLUGIN_VERSION = "2.0.0";
		
		public const string DEPENDENCY_STRING = $"DannyVD-{PLUGIN_NAME}-{PLUGIN_VERSION}";

		private void Awake()
		{
			//ConfigUtil.Initialize(Config);
			//ConfigUtil.ReadConfig();
			
			LoggerUtil.Initialize(ConfigUtil.LoggingLevel, Logger);

			// Plugin startup logic
			LoggerUtil.Log(LogLevel.Info, $"Plugin {DEPENDENCY_STRING} is loaded!"); // Using the Log function circumvents the configuration option, this is by design

			PatchUtil.PatchFunctions();
		}
	}
}