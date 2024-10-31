using MonsterSoda.Utils.ModUtil;
using UnityEngine;

namespace MonsterSoda.Components.TextureUpdaters
{
	public class SodaCanTextureUpdater : AbstractTextureUpdater
	{
		private static bool shouldUpdateTexture = true;

		protected override bool ShouldUpdateTexture() => shouldUpdateTexture;

		protected override void SwapTextures()
		{
			shouldUpdateTexture = false;
			
			Material material = rendererComponent.sharedMaterial;

			if (AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Textures/MonstiTexture.png", out Texture2D newTexture))
			{
				material.SetTexture(BaseColorMap, newTexture);
				material.SetTexture(MainTex, newTexture);
			}
		}
	}
}