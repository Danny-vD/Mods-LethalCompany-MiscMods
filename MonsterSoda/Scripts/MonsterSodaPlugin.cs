using BepInEx;
using MonsterSoda.Utils.ModUtil;

namespace MonsterSoda
{
	[BepInPlugin(GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	[BepInProcess("Lethal Company.exe")]
	public class MonsterSodaPlugin : BaseUnityPlugin
	{
		public const string GUID = $"LegaliseLeek.mods.LethalCompany.{PLUGIN_NAME}";
		public const string PLUGIN_NAME = "Monsti_Energy";
		public const string PLUGIN_VERSION = "1.0.0";
		
		public const string DEPENDENCY_STRING = $"LegaliseLeek-{PLUGIN_NAME}-{PLUGIN_VERSION}";

		private void Awake()
		{
			ConfigUtil.Initialize(Config);
			
			//ConfigUtil.ReadConfig();
			
			LoggerUtil.Initialize(ConfigUtil.LoggingLevel, Logger);

			// Plugin startup logic
			LoggerUtil.Log(BepInEx.Logging.LogLevel.Info, $"Plugin {DEPENDENCY_STRING} is loaded!"); // Using the Log function circumvents the configuration option, this is by design

			PatchUtil.PatchFunctions();
		}
	}
}