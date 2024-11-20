using System.Linq;
using System.Text;
using ExtraInformation.Utils;
using UnityEngine;

namespace ExtraInformation.InfoLoggers
{
	public static class MapObjectInfoLogger
	{
		public static void LogMapObjectsCurveData(SelectableLevel level)
		{
			StringBuilder stringBuilder = new StringBuilder("Map objects:\n");
			
			foreach (SpawnableMapObject spawnableMapObject in level.spawnableMapObjects)
			{
				stringBuilder.AppendLine(spawnableMapObject.prefabToSpawn.name);

				int count = 1;
				
				foreach (Keyframe keyframe in spawnableMapObject.numberToSpawn.keys)
				{
					stringBuilder.AppendLine($"Keyframe {count++}");
					stringBuilder.AppendLine($"({keyframe.time}, {keyframe.value})");
					stringBuilder.AppendLine($"InTangent: {keyframe.inTangent} | InWeight: {keyframe.inWeight}");
					stringBuilder.AppendLine($"OutTangent: {keyframe.outTangent} | OutWeight: {keyframe.outWeight}");
					stringBuilder.AppendLine($"{keyframe.time}, {keyframe.value}, {keyframe.inTangent}, {keyframe.inWeight}, {keyframe.outTangent}, {keyframe.outWeight}");
					
					stringBuilder.AppendLine();
				}

				stringBuilder.AppendLine();
			}
			
			LoggerUtil.LogInfo(stringBuilder.ToString());
		}

		public static void GetMaximumMapObjects(SelectableLevel level, out int maxTurrets, out int maxMines, out int maxSpikeTraps)
		{
			maxTurrets    = 0;
			maxMines      = 0;
			maxSpikeTraps = 0;
			
			foreach (SpawnableMapObject spawnableMapObject in level.spawnableMapObjects)
			{
				int maxNumber = (int)spawnableMapObject.numberToSpawn.GetKeys().Last().value;

				string prefabName = spawnableMapObject.prefabToSpawn.name.ToLowerInvariant();
				
				if (prefabName.Contains("turret"))
				{
					maxMines = maxNumber;
				}
				else if (prefabName.Contains("mine"))
				{
					maxTurrets = maxNumber;
				}
				else if (prefabName.Contains("trap"))
				{
					maxSpikeTraps = maxNumber;
				}
			}
		}
	}
}