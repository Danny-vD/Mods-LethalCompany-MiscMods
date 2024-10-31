using System.Text;
using ExtraInformation.Utils;
using UnityEngine;

namespace ExtraInformation.InfoLoggers
{
	public static class MapObjectInfoLogger
	{
		public static void LogMapObjects(SelectableLevel level)
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
	}
}