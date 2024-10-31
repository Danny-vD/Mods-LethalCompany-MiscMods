using UnityEngine;

namespace MonsterSoda.Components.TextureUpdaters
{
	public abstract class AbstractTextureUpdater : MonoBehaviour
	{
		public static readonly int BaseColorMap = Shader.PropertyToID("_BaseColorMap");
		public static readonly int MainTex = Shader.PropertyToID("_MainTex");

		protected Renderer rendererComponent;

		private void Awake()
		{
			rendererComponent = GetComponent<Renderer>();
		}

		private void Start()
		{
			if (ShouldUpdateTexture())
			{
				SwapTextures();
			}
		}

		protected abstract bool ShouldUpdateTexture();

		protected abstract void SwapTextures();
	}
}