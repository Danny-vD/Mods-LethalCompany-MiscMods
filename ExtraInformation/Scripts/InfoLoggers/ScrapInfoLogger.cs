using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraInformation.Utils;
using UnityEngine;

namespace ExtraInformation.InfoLoggers
{
	public static class ScrapInfoLogger
	{
		public static void LogScrapInfoOfLevel(SelectableLevel level, float levelSize)
		{
			StringBuilder stringBuilder = new StringBuilder($"Scrap chances on {level.PlanetName}:\n");

			List<SpawnableItemWithRarity> spawnableScrap = level.spawnableScrap;

			int totalWeight = spawnableScrap.Sum(itemWithRarity => itemWithRarity.rarity);
			float scrapAmountMultiplier = RoundManager.Instance.scrapAmountMultiplier;
			float scrapValueMultiplier = RoundManager.Instance.scrapValueMultiplier;

#if DEVELOPER_MODE
			int maxScrapInLevel = level.maxScrap;
			int minScrapInLevel = level.minScrap;
#else
			int maxScrapInLevel = (int)((level.maxScrap - 1) * scrapAmountMultiplier); // - 1 because the upper bound is exclusive
			int minScrapInLevel = (int)(level.minScrap * scrapAmountMultiplier);
#endif

			float averageScrapInLevel = (minScrapInLevel + maxScrapInLevel) / 2f;

			stringBuilder.AppendLine($"total weight: {totalWeight}\nScrap in level: {minScrapInLevel} - {maxScrapInLevel} ({averageScrapInLevel})");
			stringBuilder.AppendLine($"scrapAmountMultiplier = {scrapAmountMultiplier} | scrapValueMultiplier = {scrapValueMultiplier} | factorySizeMultiplier = {level.factorySizeMultiplier}\n");

#if !DEVELOPER_MODE
			spawnableScrap = spawnableScrap.OrderByDescending(pair => pair.rarity).ToList();
#endif

			int averageMinValuePerItem = 0;
			int averageValuePerItem = 0;
			int averageMaxValuePerItem = 0;

			int worstMinValue = int.MaxValue;
			int bestMaxValue = 0;

			int twoHandedScrapWeight = 0;

			foreach (SpawnableItemWithRarity itemWithRarity in spawnableScrap)
			{
				ScrapChanceCalculator.GetScrapChance(itemWithRarity, totalWeight, level, out float spawnChance, out float minLevelChance, out float maxLevelChance);

				string levelMinPercentage = $"{minLevelChance:P}";
				string levelMaxPercentage = $"{maxLevelChance:P}";

				Item item = itemWithRarity.spawnableItem;


#if DEVELOPER_MODE
				int maxValue = item.maxValue;
				int minValue = item.minValue;
#else
				int maxValue = (int)(item.maxValue * scrapValueMultiplier);
				int minValue = (int)(item.minValue * scrapValueMultiplier);
#endif

				averageMaxValuePerItem += maxValue * itemWithRarity.rarity;
				averageValuePerItem    += (int)(Mathf.Lerp(minValue, maxValue, 0.5f) * itemWithRarity.rarity);
				averageMinValuePerItem += minValue * itemWithRarity.rarity;

#if DEVELOPER_MODE
				stringBuilder.AppendLine($"{{{itemWithRarity.rarity}}} {item.itemName} {item.weight} [${minValue} - ${maxValue}] [conductive: {item.isConductiveMetal}] [2-handed {item.twoHanded}]");
#else
				stringBuilder.AppendLine($"{spawnChance:P} [{itemWithRarity.rarity}] {item.itemName} {{{levelMinPercentage} - {levelMaxPercentage}}} [${minValue} - ${maxValue}]");
#endif

				if (item.twoHanded)
				{
					twoHandedScrapWeight += itemWithRarity.rarity;
				}

				if (worstMinValue > minValue)
				{
					worstMinValue = minValue;
				}

				if (bestMaxValue < maxValue)
				{
					bestMaxValue = maxValue;
				}
			}

			if (totalWeight > 0)
			{
				averageMinValuePerItem /= totalWeight;
				averageValuePerItem    /= totalWeight;
				averageMaxValuePerItem /= totalWeight;
			}
			else
			{
				averageMinValuePerItem = 0;
				averageValuePerItem    = 0;
				averageMaxValuePerItem = 0;
			}

			int averageMinValueInLevel = averageValuePerItem * minScrapInLevel;
			int averageMaxValueInLevel = averageValuePerItem * maxScrapInLevel;

			worstMinValue *= minScrapInLevel;
			bestMaxValue  *= maxScrapInLevel;

			int averageValueGap = averageMaxValuePerItem - averageMinValuePerItem;

			stringBuilder.AppendLine();

			float twoHandedChance = twoHandedScrapWeight / (float)totalWeight;
			int minTwoHandedItems = Mathf.FloorToInt(twoHandedChance * minScrapInLevel);
			int maxTwoHandedItems = Mathf.FloorToInt(twoHandedChance * maxScrapInLevel);

			float averageValueInLevel = averageScrapInLevel * averageValuePerItem;
			float normalizedValueInLevel = averageValueInLevel / levelSize;

			stringBuilder.AppendLine($"Chance for 2-handed scrap: {twoHandedChance:P} [{minTwoHandedItems} - {maxTwoHandedItems}]");
			stringBuilder.AppendLine($"Value in level: ${averageValueInLevel} [${averageMinValueInLevel} - ${averageMaxValueInLevel}] (${worstMinValue} - ${bestMaxValue})");

			stringBuilder.AppendLine($"\nAverage value gap in items: ${averageValueGap}\t| Average value per item: ${averageValuePerItem} [${averageMinValuePerItem} - ${averageMaxValuePerItem}]");
			stringBuilder.AppendLine($"Normalized value: ${normalizedValueInLevel}");

			LoggerUtil.LogInfo(stringBuilder.ToString());
		}
	}
}