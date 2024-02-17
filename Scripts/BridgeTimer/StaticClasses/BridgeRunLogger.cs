using BridgeCalculator.Utils;

namespace BridgeCalculator.BridgeTimer.StaticClasses
{
	public static class BridgeRunLogger
	{
		public static void EnteredBridge(string playerName)
		{
			LoggerUtil.LogError($"{playerName} entered bridge\n");
		}

		public static void JumpedOffSide(string playerName)
		{
			LoggerUtil.LogWarning($"{playerName} jumped off the side!\n");
		}

		public static void LeftBridgeNoRun()
		{
			LoggerUtil.LogError("A player left the bridge but did not start a run\n");
		}
	}
}