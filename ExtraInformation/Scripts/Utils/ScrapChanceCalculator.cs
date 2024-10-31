using UnityEngine;

namespace ExtraInformation.Utils
{
	public static class ScrapChanceCalculator
	{
		public static void GetScrapChance(SpawnableItemWithRarity itemWithRarity, float totalWeight, SelectableLevel level, out float spawnChance, out float minLevelChance, out float maxLevelChance)
		{
			spawnChance = itemWithRarity.rarity / totalWeight;

			float scrapAmountMultiplier = RoundManager.Instance.scrapAmountMultiplier;

			minLevelChance = GetScrapPercentageInLevel(spawnChance, level.minScrap * scrapAmountMultiplier);
			maxLevelChance = GetScrapPercentageInLevel(spawnChance, level.maxScrap * scrapAmountMultiplier);
		}

		private static float GetScrapPercentageInLevel(float percentage, float scrapCountInLevel)
		{
			return 1 - Mathf.Pow(1 - percentage, scrapCountInLevel);
		}
	}
}