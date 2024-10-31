using System.Reflection;
using System.Reflection.Emit;
using MonsterSoda.PatchFunctions;
using MonsterSoda.Utils.ReflectionUtil;

namespace MonsterSoda.FunctionOverrides
{
	public static class AnimatedItemRobotToyOverrides
	{
		private static readonly DynamicMethod baseEquipItem;

		static AnimatedItemRobotToyOverrides()
		{
			MethodInfo methodInfo = typeof(GrabbableObject).GetMethod("EquipItem", BindingFlags.Instance | BindingFlags.Public);
			baseEquipItem = DynamicMethodCreator.GetDynamicMethod(methodInfo, "EquipItem");
		}

		public static void EquipItem(AnimatedItem robotToy)
		{
			baseEquipItem.Invoke(null, [robotToy]);
			
			RobotToyFunctions.SetHeldMesh(robotToy);

			if (robotToy.itemAudioLowPassFilter != null)
			{
				robotToy.itemAudioLowPassFilter.cutoffFrequency = 20000f;
			}

			robotToy.itemAudio.volume = 1f;

			if (robotToy.chanceToTriggerAlternateMesh > 0)
			{
				if (robotToy.itemRandomChance.Next(0, 100) < robotToy.chanceToTriggerAlternateMesh)
				{
					RobotToyFunctions.SetSadTexture(robotToy);
					robotToy.itemAudio.Stop();
					return;
				}
			}

			if (!robotToy.wasInPocket)
			{
				if (robotToy.itemRandomChance.Next(0, 100) > robotToy.chanceToTriggerAnimation)
				{
					RobotToyFunctions.SetSadTexture(robotToy);
					robotToy.itemAudio.Stop();
					return;
				}
			}
			else
			{
				robotToy.wasInPocket = false;
			}

			RobotToyFunctions.SetNormalTexture(robotToy);

			if (robotToy.itemAnimator != null)
			{
				robotToy.itemAnimator.SetBool(robotToy.grabItemBoolString, true);
			}

			if (robotToy.itemAudio != null)
			{
				robotToy.itemAudio.clip = robotToy.grabAudio;
				robotToy.itemAudio.loop = robotToy.loopGrabAudio;
				robotToy.itemAudio.Play();
			}
		}
	}
}