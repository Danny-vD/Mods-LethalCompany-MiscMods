using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MonsterSoda.Utils.ModUtil
{
	public static class AssetBundleUtil
	{
		private static readonly string assemblyDirectory;

		private static readonly Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

		static AssetBundleUtil()
		{
			assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		public static bool TryLoadAsset<TAssetType>(string bundleName, string assetPath, out TAssetType asset) where TAssetType : Object
		{
			if (!TryGetAssetBundle(bundleName, out AssetBundle assetBundle))
			{
				asset = default;
				return false;
			}

			asset = assetBundle.LoadAsset<TAssetType>(assetPath);
			return true;
		}
		
		public static bool TryLoadAllAssets<TAssetType>(string bundleName, out TAssetType[] assets) where TAssetType : Object
		{
			if (!TryGetAssetBundle(bundleName, out AssetBundle assetBundle))
			{
				assets = default;
				return false;
			}

			assets = assetBundle.LoadAllAssets<TAssetType>();

			return assets.Length > 0;
		}

		public static bool TryLoadAssetBundle(string bundleName, out AssetBundle assetBundle)
		{
			assetBundle = AssetBundle.LoadFromFile(GetPath(bundleName));

			if (ReferenceEquals(assetBundle, null))
			{
				return false;
			}

			assetBundles.Add(bundleName, assetBundle);
			return true;
		}
		
		public static bool TryUnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects)
		{
			if (assetBundles.TryGetValue(bundleName, out AssetBundle bundle))
			{
				bundle.Unload(unloadAllLoadedObjects);
				return true;
			}

			return false;
		}

		public static void UnloadAllAssetBundles(bool unloadAllLoadedObjects)
		{
			AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
		}

		private static bool TryGetAssetBundle(string bundleName, out AssetBundle assetBundle)
		{
			if (assetBundles.TryGetValue(bundleName, out AssetBundle bundle))
			{
				assetBundle = bundle;
				return true;
			}

			return TryLoadAssetBundle(bundleName, out assetBundle);
		}

		private static string GetPath(string path)
		{
			return Path.Combine(assemblyDirectory, path);
		}
	}
}