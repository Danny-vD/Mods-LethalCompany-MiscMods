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
			//int maxScrapInLevel = (int)(level.maxScrap * scrapAmountMultiplier); // TODO: uncomment

			stringBuilder.AppendLine($"total weight: {totalWeight}\nScrap in level: {minScrapInLevel} - {maxScrapInLevel}");
			stringBuilder.AppendLine($"scrapAmountMultiplier = {scrapAmountMultiplier} | scrapValueMultiplier = {scrapValueMultiplier} | factorySizeMultiplier = {level.factorySizeMultiplier}\n");

			spawnableScrap = spawnableScrap.OrderByDescending(pair => pair.rarity).ToList(); // TODO: Uncomment

			int usualMinValue = 0;
			int usualMaxValue = 0;

			int worstMinValue = int.MaxValue;
			int bestMaxValue = 0;

			int twoHandedScrapWeight = 0;

			foreach (SpawnableItemWithRarity itemWithRarity in spawnableScrap)
			{
				ScrapChanceCalculator.GetScrapChance(itemWithRarity, totalWeight, level, out float spawnChance, out float minLevelChance, out float maxLevelChance);

				string levelMinPercentage = $"{minLevelChance:P}";
				string levelMaxPercentage = $"{maxLevelChance:P}";

				Item item = itemWithRarity.spawnableItem;

				int maxValue = (int)(item.maxValue * scrapValueMultiplier); //TODO: UNCOMMENT
				int minValue = (int)(item.minValue * scrapValueMultiplier);

				stringBuilder.AppendLine($"{spawnChance:P} {item.itemName} {{{levelMinPercentage} - {levelMaxPercentage}}} [{itemWithRarity.rarity}] [${minValue} - ${maxValue}]"); // TODO: Uncomment
				//stringBuilder.AppendLine($"{{{itemWithRarity.rarity}}} {item.itemName} {item.weight} [${minValue} - ${maxValue}] [conductive: {item.isConductiveMetal}] [2-handed {item.twoHanded}]");

				if (item.twoHanded)
				{
					twoHandedScrapWeight += itemWithRarity.rarity;
				}

				usualMinValue += item.minValue * itemWithRarity.rarity;
				usualMaxValue += item.maxValue * itemWithRarity.rarity;

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
				usualMinValue = (int)(usualMinValue * minScrapInLevel * scrapValueMultiplier) / totalWeight;
				usualMaxValue = (int)(usualMaxValue * maxScrapInLevel * scrapValueMultiplier) / totalWeight;
			}
			else
			{
				usualMinValue = 0;
				usualMaxValue = 0;
			}

			worstMinValue *= minScrapInLevel;
			bestMaxValue  *= maxScrapInLevel;

			stringBuilder.AppendLine();

			float twoHandedChance = twoHandedScrapWeight / (float)totalWeight;
			int minTwoHandedItems = Mathf.RoundToInt(twoHandedChance * minScrapInLevel);
			int maxTwoHandedItems = Mathf.RoundToInt(twoHandedChance * maxScrapInLevel);
			
			stringBuilder.AppendLine($"Chance for 2-handed scrap: {twoHandedChance:P} [{minTwoHandedItems} - {maxTwoHandedItems}]");
			stringBuilder.AppendLine($"Value in level: ${usualMinValue} - ${usualMaxValue} (${worstMinValue} - ${bestMaxValue})");

			LoggerUtil.LogInfo(stringBuilder.ToString());
		}
	}
}