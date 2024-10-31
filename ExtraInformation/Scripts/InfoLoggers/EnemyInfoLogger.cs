using System.Linq;
using System.Text;
using ExtraInformation.Utils;

namespace ExtraInformation.InfoLoggers
{
	public static class EnemyInfoLogger
	{
		public static void LogEnemyInfoOfLevel(SelectableLevel level)
		{
			StringBuilder stringBuilder = new StringBuilder($"Enemy chances on {level.PlanetName}:\n");

			int totalWeight = level.Enemies.Sum(spawnableEnemy => spawnableEnemy.rarity);
			int totalOutsideEnemyWeight = level.OutsideEnemies.Sum(spawnableEnemy => spawnableEnemy.rarity);
			int dayTimeEnemiesTotalWeight = level.DaytimeEnemies.Sum(spawnableEnemy => spawnableEnemy.rarity);

			stringBuilder.AppendLine($"Daytime enemies [{dayTimeEnemiesTotalWeight}] (max power {level.maxDaytimeEnemyPowerCount}):");

			foreach (SpawnableEnemyWithRarity daytimeEnemy in level.DaytimeEnemies.OrderByDescending(pair => pair.rarity))
			{
				float spawnChance = daytimeEnemy.rarity / (float)dayTimeEnemiesTotalWeight;

				stringBuilder.AppendLine($"{spawnChance:P} {daytimeEnemy.enemyType.enemyName} [{daytimeEnemy.rarity}] ({daytimeEnemy.enemyType.PowerLevel}) <{daytimeEnemy.enemyType.MaxCount}>");
			}

			stringBuilder.AppendLine($"\nInside enemies [{totalWeight}] (max power {level.maxEnemyPowerCount}):");

			//EnemyType increasedInsideEnemySpawnRateType = level.Enemies[RoundManager.Instance.increasedInsideEnemySpawnRateIndex].enemyType;

			foreach (SpawnableEnemyWithRarity insideEnemy in level.Enemies.OrderByDescending(pair => pair.rarity))
			{
				EnemyType insideEnemyType = insideEnemy.enemyType;

				//	float spawnChance = ReferenceEquals(increasedInsideEnemySpawnRateType, insideEnemyType) ? 100 : insideEnemy.rarity;
				float spawnChance = insideEnemy.rarity;

				spawnChance /= totalWeight;

				stringBuilder.AppendLine($"{spawnChance:P} {insideEnemyType.enemyName} [{insideEnemy.rarity}] ({insideEnemyType.PowerLevel}) <{insideEnemyType.MaxCount}>");
			}

			stringBuilder.AppendLine($"\nOutside enemies [{totalOutsideEnemyWeight}] (max power {level.maxOutsideEnemyPowerCount}):");

			foreach (SpawnableEnemyWithRarity outsideEnemy in level.OutsideEnemies.OrderByDescending(pair => pair.rarity))
			{
				float spawnChance = outsideEnemy.rarity / (float)totalOutsideEnemyWeight;

				stringBuilder.AppendLine($"{spawnChance:P} {outsideEnemy.enemyType.enemyName} [{outsideEnemy.rarity}] ({outsideEnemy.enemyType.PowerLevel}) <{outsideEnemy.enemyType.MaxCount}>");
			}

			LoggerUtil.LogInfo(stringBuilder.ToString());
		}
	}
}