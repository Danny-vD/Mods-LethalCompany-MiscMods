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

		public static void SuccessfulSideJump(float jumpTime, float jumpStartedDurability, float bridgeHealth)
		{
			float healthRegained = bridgeHealth - jumpStartedDurability;
			
			LoggerUtil.LogWarning(
				$"Successful jump!\nJump time: {jumpTime} seconds\nHealth regained: {healthRegained * 100}%\nCurrent Health: {bridgeHealth * 100}%\n");
		}

		public static void FellOffBridge(string playerName)
		{
			LoggerUtil.LogError($"{playerName} fell off the bridge!\n");
		}

		public static void LeftBridgeNoRun()
		{
			LoggerUtil.LogError("A player left the bridge but did not start a run\n");
		}
	}
}