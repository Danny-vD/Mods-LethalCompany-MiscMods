using System.Linq;
using GameNetcodeStuff;

namespace ExtraInformation.Utils
{
	public static class PlayerControllerBUtils
	{
		public static int GetTotalValueCarrying(PlayerControllerB player, out int currentSlotValue)
		{
			if (ReferenceEquals(player, null))
			{
				currentSlotValue = 0;
				return 0;
			}
			
			GrabbableObject currentItem = player.ItemSlots[player.currentItemSlot];

			currentSlotValue = 0;
			
			if (currentItem != null && currentItem.itemProperties.isScrap)
			{
				currentSlotValue = currentItem.scrapValue;
			}
			
			return player.ItemSlots.Sum(item => item == null || !item.itemProperties.isScrap ? 0 : item.scrapValue);
		}
	}
}