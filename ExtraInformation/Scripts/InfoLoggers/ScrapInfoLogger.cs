using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraInformation.Utils;
using UnityEngine;

namespace ExtraInformation.InfoLoggers
{
	public static class ScrapInfoLogger
	{
		public static void LogScrapInfoOfLevel(SelectableLevel level)
		{
			StringBuilder stringBuilder = new StringBuilder($"Scrap chances on {level.PlanetName}:\n");

			List<SpawnableItemWithRarity> spawnableScrap = level.spawnableScrap;

			int totalWeight = spawnableScrap.Sum(itemWithRarity => itemWithRarity.rarity);
			float scrapAmountMultiplier = RoundManager.Instance.scrapAmountMultiplier;
			float scrapValueMultiplier = RoundManager.Instance.scrapValueMultiplier;

			int minScrapInLevel = (int)(level.minScrap * scrapAmountMultiplier);
			int maxScrapInLevel = (int)((level.maxScrap - 1) * scrapAmountMultiplier); // - 1 because the upper bound is exclusive

			//int maxScrapInLevel = (int)(level.maxScrap * scrapAmountMultiplier); // NOTE: Uncomment when adding data to code

			stringBuilder.AppendLine($"total weight: {totalWeight}\nScrap in level: {minScrapInLevel} - {maxScrapInLevel}");
			stringBuilder.AppendLine($"scrapAmountMultiplier = {scrapAmountMultiplier} | scrapValueMultiplier = {scrapValueMultiplier} | factorySizeMultiplier = {level.factorySizeMultiplier}\n");

			spawnableScrap = spawnableScrap.OrderByDescending(pair => pair.rarity).ToList(); // NOTE: Comment when adding data to code

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

				int maxValue = (int)(item.maxValue * scrapValueMultiplier); //NOTE: Comment when adding data to code
				int minValue = (int)(item.minValue * scrapValueMultiplier);

				averageMaxValuePerItem += maxValue * itemWithRarity.rarity;
				averageValuePerItem    += (int)(Mathf.Lerp(minValue, maxValue, 0.5f) * itemWithRarity.rarity);
				averageMinValuePerItem += minValue * itemWithRarity.rarity;

				stringBuilder.AppendLine($"{spawnChance:P} {item.itemName} {{{levelMinPercentage} - {levelMaxPercentage}}} [{itemWithRarity.rarity}] [${minValue} - ${maxValue}]"); // NOTE: For general purposes

				//stringBuilder.AppendLine($"{{{itemWithRarity.rarity}}} {item.itemName} {item.weight} [${minValue} - ${maxValue}] [conductive: {item.isConductiveMetal}] [2-handed {item.twoHanded}]"); // NOTE: for research purposes

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
			int averageMaxValueInLevel = averageValuePerItem * minScrapInLevel;
			
			worstMinValue *= minScrapInLevel;
			bestMaxValue  *= maxScrapInLevel;

			int averageValueGap = averageMaxValuePerItem - averageMinValuePerItem;

			stringBuilder.AppendLine();

			float twoHandedChance = twoHandedScrapWeight / (float)totalWeight;
			int minTwoHandedItems = Mathf.RoundToInt(twoHandedChance * minScrapInLevel);
			int maxTwoHandedItems = Mathf.RoundToInt(twoHandedChance * maxScrapInLevel);

			stringBuilder.AppendLine($"Chance for 2-handed scrap: {twoHandedChance:P} [{minTwoHandedItems} - {maxTwoHandedItems}]");
			stringBuilder.AppendLine($"Value in level: ${averageMinValueInLevel} - ${averageMaxValueInLevel} (${worstMinValue} - ${bestMaxValue})");
			stringBuilder.AppendLine($"\nAverage value gap in items: ${averageValueGap}\t| Average value per item: ${averageValuePerItem} [${averageMinValueInLevel} - ${averageMaxValuePerItem}]");

			LoggerUtil.LogInfo(stringBuilder.ToString());
		}
	}
}