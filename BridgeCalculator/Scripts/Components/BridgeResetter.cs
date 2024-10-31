using BridgeCalculator.BaseClasses;
using BridgeCalculator.Events;
using BridgeCalculator.Utils;
using UnityEngine;

namespace BridgeCalculator.Components
{
	public class BridgeResetter : BetterMonoBehaviour
	{
		public const float RESET_TIME = 3f;
		
		private BridgeTrigger bridgeTrigger;

		private int defaultAnimationStateID;
		private float defaultBridgeDurability;

		private static readonly int durabilityParameterID = Animator.StringToHash("durability");
		private static readonly int fallParameterID = Animator.StringToHash("Fall");

		private bool isInvoking = false;

		private void Awake()
		{
			bridgeTrigger = GetComponent<BridgeTrigger>();
		}

		private void Start()
		{
			BridgeFallenEvent.OnBridgeFallen += ResetBridgeAfterTime;

			defaultAnimationStateID = bridgeTrigger.bridgeAnimator.GetCurrentAnimatorStateInfo(-1).fullPathHash;
			defaultBridgeDurability = bridgeTrigger.bridgeDurability;
		}

		private void ResetBridgeAfterTime()
		{
			if (!isInvoking)
			{
				isInvoking = true;
				Invoke(nameof(ResetBridge), RESET_TIME);
			}
		}

		private void ResetBridge()
		{
			isInvoking = false;
			LoggerUtil.LogWarning("Resetting bridge!");

			bridgeTrigger.hasBridgeFallen  = false;
			bridgeTrigger.bridgeDurability = defaultBridgeDurability;

			DisableFallenBridgeColliders();

			bridgeTrigger.bridgeAnimator.SetFloat(durabilityParameterID, 0);
			bridgeTrigger.bridgeAnimator.ResetTrigger(fallParameterID);

			bridgeTrigger.bridgeAnimator.Play(defaultAnimationStateID);
			
			bridgeTrigger.bridgeAnimator.Rebind(true);
			bridgeTrigger.bridgeAnimator.Update(0);
		}

		private void DisableFallenBridgeColliders()
		{
			for (int i = 0; i < bridgeTrigger.fallenBridgeColliders.Length; i++)
			{
				bridgeTrigger.fallenBridgeColliders[i].enabled = false;
			}
		}

		public override void OnDestroy()
		{
			BridgeFallenEvent.OnBridgeFallen -= ResetBridgeAfterTime;
		}
	}
}