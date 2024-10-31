using UnityEngine;

namespace ExtraInformation.AssetRippers.Components
{
	public class AudioClipSaver : MonoBehaviour
	{
		public void SaveAudioClip(string objectName, AudioClip clip)
		{
			AudioClipToWavFile.Save($@"{NameUtils.RemoveCloneFromString(objectName)}\{clip.GetName()}", clip, true);
		}
	}
}