using System.Text;
using GameNetcodeStuff;

namespace ExtraInformation.Utils
{
	public static class HUDInfoShower
	{
		public static void ShowValueCarrying(StringBuilder stringBuilder, PlayerControllerB player)
		{
			float totalValueCarrying = PlayerControllerBUtils.GetTotalValueCarrying(player, out int currentSlotValue);

			if (totalValueCarrying > 0)
			{
				if (currentSlotValue > 0)
				{
					stringBuilder.AppendLine($"Value: ${totalValueCarrying} [{currentSlotValue}]");
				}
				else
				{
					stringBuilder.AppendLine($"Value: ${totalValueCarrying}");
				}
			}
		}

		public static void ShowSpeed(StringBuilder stringBuilder, PlayerControllerB player)
		{
			float speed = player.thisController.velocity.magnitude;

			if (speed < 0.0001f)
			{
				speed = 0;
			}

			stringBuilder.AppendLine($"{speed:0.###} m/s");
			stringBuilder.Append($"{player.sprintMultiplier:0.#####} m/s");
		}

		public static void ShowImmortality(StringBuilder stringBuilder, PlayerControllerB player)
		{
			string immortal = !player.AllowPlayerDeath() ? "Yes" : "No";
			stringBuilder.AppendLine($"Immortal: {immortal}");
		}

		public static void ShowFallDamage(StringBuilder stringBuilder, PlayerControllerB player)
		{
			string fallDamage = player.takingFallDamage ? "Yes" : "No";

			if (player.takingFallDamage)
			{
				stringBuilder.AppendLine($"Fall damage: {fallDamage}");
			}
		}

		public static void ShowFallInformation(StringBuilder stringBuilder, PlayerControllerB player)
		{
			string fallingType = player.isJumping ? "Jumping" : player.isFallingFromJump ? "Jump Fall" : player.isFallingNoJump ? "Falling" : "Not falling";
			string grounded = player.thisController.isGrounded ? "Yes" : "No";

			stringBuilder.AppendLine($"\n{player.fallValue:0.###} | {player.fallValueUncapped:0.###}");
			stringBuilder.AppendLine($"\n{fallingType} [{grounded}]");
		}

		public static void ShowTzpInformation(StringBuilder stringBuilder, PlayerControllerB player)
		{
			stringBuilder.AppendLine($"\nDrunk: {player.drunkness:0.#####}");
			stringBuilder.AppendLine($"Inertia: {player.drunknessInertia:0.#####}");
		}

		public static void ShowEggInformation(StringBuilder stringBuilder, PlayerControllerB player)
		{
			GrabbableObject currentItem = player.ItemSlots[player.currentItemSlot];

			if (currentItem is StunGrenadeItem { chanceToExplode: < 100 } easterEgg) // The actual game logic to detect a easter egg
			{
				stringBuilder.AppendLine($"Egg explode: [{(easterEgg.explodeOnThrow ? "Y" : "N")}]");
			}
		}
	}
}