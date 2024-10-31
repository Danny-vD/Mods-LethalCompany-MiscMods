using MonsterSoda.Components;
using MonsterSoda.Components.TextureUpdaters;
using MonsterSoda.Utils.ModUtil;
using UnityEngine;

namespace MonsterSoda.PatchFunctions
{
	public static class RobotToyFunctions
	{
		private static readonly Texture2D normalTexture;
		private static readonly Texture2D sadTexture;

		private static readonly AudioClip grabAudio;
		private static readonly AudioClip dropAudio;
		private static readonly AudioClip grabSFX;
		private static readonly AudioClip pocketSFX;

		private static readonly Mesh normalMesh;
		private static readonly Mesh heldMesh;

		public static bool DidSetPrefab { get; private set; } = false;

		static RobotToyFunctions()
		{
			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Textures/SqueezyTexture.png", out normalTexture);
			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Textures/SqueezySadTexture.png", out sadTexture);

			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Audio/SqueezyLoop.wav", out grabAudio);
			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Audio/SqueezyDrop.wav", out dropAudio);
			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Audio/SqueezyPickUp.wav", out grabSFX);
			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Audio/SqueezySwitch.wav", out pocketSFX);

			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Models/Squeezy.obj", out normalMesh);
			AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Models/SqueezyHeld.obj", out heldMesh);
		}

		public static void SetPrefab(GameObject prefab)
		{
			DidSetPrefab = true;

			AnimatedItem animatedItem = prefab.GetComponent<AnimatedItem>();

			if (animatedItem)
			{
				Start(animatedItem, true);
			}
		}

		public static void Start(AnimatedItem robotToy, bool isPrefab)
		{
			robotToy.grabAudio                = grabAudio;
			robotToy.dropAudio                = dropAudio;
			robotToy.itemProperties.grabSFX   = grabSFX;
			robotToy.itemProperties.dropSFX   = dropAudio;
			robotToy.itemProperties.pocketSFX = pocketSFX;

			robotToy.normalMesh    = normalMesh;
			robotToy.alternateMesh = normalMesh;

			if (isPrefab)
			{
				SetHeldMesh(robotToy);
			}
			else
			{
				SetNormalMesh(robotToy);
			}

			SetNormalTexture(robotToy);

			ObjectNameSetter objectNameSetter = robotToy.gameObject.AddComponent<ObjectNameSetter>();
			objectNameSetter.SetName("Squeezy");

			foreach (Transform child in robotToy.transform)
			{
				if (child.name.ToLower().Contains("arm"))
				{
					child.GetComponent<Renderer>().forceRenderingOff = true;
					child.gameObject.SetActive(false);
				}
			}
		}

		public static void Discarditem(AnimatedItem robotToy)
		{
			SetNormalMesh(robotToy);
			SetNormalTexture(robotToy);
		}

		public static void SetNormalTexture(Component robotToy)
		{
			SetTexture(robotToy, normalTexture);
		}

		public static void SetSadTexture(Component robotToy)
		{
			SetTexture(robotToy, sadTexture);
		}

		public static void SetNormalMesh(AnimatedItem robotToy)
		{
			SetMesh(robotToy, normalMesh);
		}

		public static void SetHeldMesh(AnimatedItem robotToy)
		{
			SetMesh(robotToy, heldMesh);
		}

		private static void SetTexture(Component robotToy, Texture newTexture)
		{
			robotToy.gameObject.GetComponent<Renderer>().material.SetTexture(AbstractTextureUpdater.BaseColorMap, newTexture);
		}

		private static void SetMesh(Component robotToy, Mesh newMesh)
		{
			robotToy.gameObject.GetComponent<MeshFilter>().mesh = newMesh;
		}
	}
}