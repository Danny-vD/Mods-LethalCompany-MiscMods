// ReSharper disable UnusedMember.Global // False positive, HarmonyX uses these to patch
// ReSharper disable InconsistentNaming // While true, Harmony wants these specific names

using System.Collections;
using HarmonyLib;

namespace InstantTerminalTyping
{
	internal static class Patches
	{
// TERMINAL PATCHES

		[HarmonyPatch(typeof(Terminal), nameof(Terminal.selectTextFieldDelayed)), HarmonyPrefix]
		internal static bool TerminalSelectTextFieldDelayedPatchPreFix()
		{
			return false;
		}
		
		[HarmonyPatch(typeof(Terminal), nameof(Terminal.selectTextFieldDelayed)), HarmonyPostfix]
		internal static void TerminalSelectTextFieldDelayedPatchPostFix(Terminal __instance)
		{
			__instance.StartCoroutine(selectTextFieldDelayed(__instance));
		}

		private static IEnumerator selectTextFieldDelayed(Terminal __instance)
		{
			__instance.screenText.ActivateInputField();
			yield return null;
			
			__instance.screenText.Select();
		}
	}
}