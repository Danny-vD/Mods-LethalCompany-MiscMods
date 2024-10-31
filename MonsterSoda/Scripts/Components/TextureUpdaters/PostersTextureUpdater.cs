using System.Collections.Generic;
using System.Linq;
using MonsterSoda.Utils.ModUtil;
using UnityEngine;

namespace MonsterSoda.Components.TextureUpdaters
{
	public class PostersTextureUpdater : AbstractTextureUpdater
	{
		private static bool shouldUpdateTexture = true;

		protected override bool ShouldUpdateTexture() => shouldUpdateTexture;

		protected override void SwapTextures()
		{
			shouldUpdateTexture = false;
			
			List<Material> materials = rendererComponent.materials.ToList();
			Material posters = materials[0];

			if (AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Textures/AllPosters.png", out Texture2D newTexture))
			{
				posters.SetTexture(BaseColorMap, newTexture);
				posters.SetTexture(MainTex, newTexture);
			}
			
			Material tipPoster = materials[1];
			
			if (AssetBundleUtil.TryLoadAsset("monstienergy.lem", "Assets/Mods/MonstiEnergy/Textures/BigPosterTexture.png", out newTexture))
			{
				tipPoster.SetTexture(BaseColorMap, newTexture);
				tipPoster.SetTexture(MainTex, newTexture);
			}
			
			rendererComponent.SetSharedMaterials(materials);
		}
	}
}