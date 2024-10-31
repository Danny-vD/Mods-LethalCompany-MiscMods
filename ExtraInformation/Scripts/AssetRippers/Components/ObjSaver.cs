using System.IO;
using ExtraInformation.Constants;
using UnityEngine;

namespace ExtraInformation.AssetRippers.Components
{
	public class ObjSaver : MonoBehaviour
	{
		public void SaveObj()
		{
			string objContents = MeshToObj.GetObj(gameObject, out string meshName);
			
			string objectname = NameUtils.RemoveCloneFromString(name);
			string directory = $@"{IOPaths.PATH}\{objectname}";
			Directory.CreateDirectory(directory);
			
			File.WriteAllText($@"{directory}\{NameUtils.ReplaceSlashesInName(meshName)}.obj", objContents);
		}
		
		public void SaveObjCurrentTransform()
		{
			string objContents = MeshToObj.GetObj(gameObject, out string meshName, true);
			
			string objectname = NameUtils.RemoveCloneFromString(name);
			string directory = $@"{IOPaths.PATH}\{objectname}";
			Directory.CreateDirectory(directory);
			
			File.WriteAllText($@"{directory}\{NameUtils.ReplaceSlashesInName(meshName)}.obj", objContents);
		}
	}
}