using System.Collections.Generic;
using BridgeCalculator.Components;
using BridgeCalculator.Utils;
using UnityEngine;

namespace BridgeCalculator.BridgeTimer
{
	public class BridgeRun
	{
		public static float DisqualificationDistance => BridgeRunManager.BridgeLength - 1; // BridgeLength - 1 to allow jumping off at the side at the end/beginning
		
		public string PlayerName { get; private set; }
		
		public List<SideJump> Jumps;

		private BridgeTimer bridgeTimer;

		private SideJump currentSideJump;

		public BridgeRun(string playerName, Vector3 enterPosition)
		{
			PlayerName = playerName;
			
			LoggerUtil.LogError($"{PlayerName} entered bridge\n");
			
			bridgeTimer = new BridgeTimer(enterPosition);
		}

		public void Update()
		{
			bridgeTimer.Update();
			//TODO: move timer to SideJump?
		}

		public void StartSideJump()
		{
			
		}

		public void StopSideJump()
		{
			//TODO: use SideJump
			bridgeTimer.StopSideTimer(true);
		}

		public void FellOffBridge()
		{
			bridgeTimer.StopAllTimers();
		}
	}
}