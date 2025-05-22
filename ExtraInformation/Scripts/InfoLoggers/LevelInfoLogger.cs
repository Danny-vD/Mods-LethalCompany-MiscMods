using System.Linq;
using System.Text;
using DunGen.Graph;
using ExtraInformation.Utils;
using UnityEngine;

namespace ExtraInformation.InfoLoggers
{
	public static class LevelInfoLogger
	{
		public static void LogWeatherOfLevel(SelectableLevel level)
		{
			StringBuilder stringBuilder = new StringBuilder($"{level.PlanetName} weathers:\n");

			foreach (LevelWeatherType levelWeatherType in level.randomWeathers.Select(randomWeather => randomWeather.weatherType))
			{
				stringBuilder.AppendLine(levelWeatherType.ToString());
			}

			LoggerUtil.LogInfo(stringBuilder.ToString());
		}

		public static void LogDungeonFlowsOfLevel(SelectableLevel level, out float levelSize)
		{
			IntWithRarity[] dungeonFlowTypes = level.dungeonFlowTypes;
			StringBuilder stringBuilder;

			if (dungeonFlowTypes.Length == 0)
			{
				stringBuilder = new StringBuilder($"{level.PlanetName} ({level.levelID}) Info\n");
				DungeonFlow dungeonFlow = RoundManager.Instance.dungeonFlowTypes.First().dungeonFlow;

				stringBuilder.AppendLine($"100% {GetFlowName(dungeonFlow.name)}");
				levelSize = 0;
			}
			else
			{
				int totalWeight = dungeonFlowTypes.Sum(dungeonFlowType => dungeonFlowType.rarity);
				stringBuilder = new StringBuilder($"{level.PlanetName} ({level.levelID}) Info\nTotal Weight: {totalWeight}\n");
				

#if DEVELOPER_MODE
				IntWithRarity[] orderedFlowTypes = dungeonFlowTypes;
#else
				IOrderedEnumerable<IntWithRarity> orderedFlowTypes = dungeonFlowTypes.OrderByDescending(dungeonFlowType => dungeonFlowType.rarity);
#endif

				IndoorMapType[] dungeonFlows = RoundManager.Instance.dungeonFlowTypes;
				float mapSizeMultiplier = RoundManager.Instance.mapSizeMultiplier;
				levelSize = 0; // always overridden

				foreach (IntWithRarity orderedFlowType in orderedFlowTypes)
				{
					float chance = orderedFlowType.rarity / (float)totalWeight;

					IndoorMapType indoorMapType = dungeonFlows[orderedFlowType.id];
					DungeonFlow dungeonFlow = indoorMapType.dungeonFlow;

					float size;

					if (GetFlowName(dungeonFlow.name).Contains("Factory"))
					{
						size = level.factorySizeMultiplier * mapSizeMultiplier;
					}
					else
					{
						size = Mathf.Round(level.factorySizeMultiplier / indoorMapType.MapTileSize * mapSizeMultiplier * 100f) / 100f;
					}

					levelSize = size * indoorMapType.MapTileSize;
					stringBuilder.AppendLine($"{chance:P} [{orderedFlowType.rarity}] {GetFlowName(dungeonFlow.name)} (Size {size}) [TileSize {indoorMapType.MapTileSize}] <{levelSize}>");
				}
			}

			LoggerUtil.LogInfo(stringBuilder.ToString());
		}

		private static string GetFlowName(string name)
		{
			if (name.Contains("1"))
			{
				return "Factory";
			}

			if (name.Contains("2"))
			{
				return "Mansion";
			}

			if (name.Contains("3"))
			{
				return "Mineshaft";
			}

			return name;
		}
	}
}