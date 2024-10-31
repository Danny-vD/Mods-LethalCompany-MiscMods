using System.IO;
using ExtraInformation.Constants;
using UnityEngine;

namespace ExtraInformation.AssetRippers.Components
{
	public class TextureSaver : MonoBehaviour
	{
		private Renderer renderer;

		private void Awake()
		{
			renderer = GetComponent<Renderer>();
		}

		public void SaveTextures()
		{
			foreach (Material material in renderer.materials)
			{
				string objectname = NameUtils.RemoveCloneFromString(name);
				string directory = $@"{IOPaths.PATH}\{objectname}";
				Directory.CreateDirectory(directory);

				Shader shader = material.shader;
				string shaderSourceCode = shader.ToString();

				File.WriteAllText($@"{directory}\{NameUtils.ReplaceSlashesInName(material.name)}[{NameUtils.ReplaceSlashesInName(shader.name)}].shader", shaderSourceCode);

				string[] texturesPropertyNames = material.GetTexturePropertyNames();

				foreach (string propertyName in texturesPropertyNames)
				{
					Texture texture = material.GetTexture(propertyName);

					if (texture == null)
					{
						continue;
					}

					Texture2D readableTexture = CreateReadableTexture(texture);
					byte[] bytes = readableTexture.EncodeToPNG();

					string fullPath = $@"{directory}\[{propertyName}]{texture.name}.png";
					File.WriteAllBytes(fullPath, bytes);
				}
			}
		}

		private static Texture2D CreateReadableTexture(Texture texture)
		{
			RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

			Graphics.Blit(texture, renderTexture);
			RenderTexture previousActiveTexture = RenderTexture.active;
			RenderTexture.active = renderTexture;

			Texture2D readableTexture = new Texture2D(texture.width, texture.height);
			readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			readableTexture.Apply();

			RenderTexture.active = previousActiveTexture;
			RenderTexture.ReleaseTemporary(renderTexture);

			return readableTexture;
		}
	}
}