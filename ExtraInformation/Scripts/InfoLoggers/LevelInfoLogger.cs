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

		public static void LogDungeonFlowsOfLevel(SelectableLevel level)
		{
			IntWithRarity[] dungeonFlowTypes = level.dungeonFlowTypes;
			StringBuilder stringBuilder = new StringBuilder($"{level.PlanetName} ({level.levelID}) Info\n");

			if (dungeonFlowTypes.Length == 0)
			{
				DungeonFlow dungeonFlow = RoundManager.Instance.dungeonFlowTypes.First().dungeonFlow;

				stringBuilder.AppendLine($"100% {GetFlowName(dungeonFlow.name)}");
			}
			else
			{
				int totalWeight = dungeonFlowTypes.Sum(dungeonFlowType => dungeonFlowType.rarity);
				
#if DEVELOPER_MODE

				IntWithRarity[] orderedFlowTypes = dungeonFlowTypes;
#else
				IOrderedEnumerable<IntWithRarity> orderedFlowTypes = dungeonFlowTypes.OrderByDescending(dungeonFlowType => dungeonFlowType.rarity);
#endif

				IndoorMapType[] dungeonFlows = RoundManager.Instance.dungeonFlowTypes;
				float mapSizeMultiplier = RoundManager.Instance.mapSizeMultiplier;

				foreach (IntWithRarity orderedFlowType in orderedFlowTypes)
				{
					float chance = orderedFlowType.rarity / (float)totalWeight;

					IndoorMapType indoorMapType = dungeonFlows[orderedFlowType.id];
					DungeonFlow dungeonFlow = indoorMapType.dungeonFlow;

					float size = Mathf.Round(level.factorySizeMultiplier / indoorMapType.MapTileSize * mapSizeMultiplier * 100f) / 100f;

					stringBuilder.AppendLine($"{chance:P} [{orderedFlowType.rarity}] {GetFlowName(dungeonFlow.name)} (Size {size}) [TileSize {indoorMapType.MapTileSize}]");
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