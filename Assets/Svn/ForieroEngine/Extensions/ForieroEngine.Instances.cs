using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace ForieroEngine
{
	public static class Instances
	{
		public static Dictionary<string, object> instances = new Dictionary<string, object>();

		public static T Instance<T>(string name, string assetPath = "") where T : ScriptableObject
		{
			if (!string.IsNullOrEmpty(assetPath))
			{
				assetPath = assetPath.StartsWith("/", System.StringComparison.Ordinal) ? assetPath.Remove(0, 1) : assetPath;
				assetPath = assetPath.EndsWith("/", System.StringComparison.Ordinal) ? assetPath : assetPath + "/";
			}

			var path = assetPath + name;

			T instance = default(T);

			if (instances.ContainsKey(path))
			{
				//Debug.Log ("FResources Loading Cached : " + path);
				instance = (T)instances[path];

			}
			else
			{
				//Debug.Log ("FResources Loading : " + path);
				instance = Resources.Load<T>(path);
				instances.Add(path, instance);
			}

			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<T>();
				if (Application.isPlaying) Debug.LogError(name + " does not exist!!! Creating one in memory!");				
#if UNITY_EDITOR
				if (!Application.isBatchMode)
				{
					var resourcesPath = "Assets/Resources/" + path + ".asset";
					//Debug.Log ("FResource Path : " + resourcesPath);

					var resourceDirectory = Application.dataPath + "/Resources/" + assetPath;
					//Debug.Log ("FResource Directory : " + resourceDirectory);

					if (!Directory.Exists(resourceDirectory))
					{
						System.IO.Directory.CreateDirectory(resourceDirectory);
					}

					AssetDatabase.CreateAsset(instance, resourcesPath);

					AssetDatabase.SaveAssets();
					
					instance = Resources.Load<T>(path);
					instances[path] = instance;
				}
#endif
			}

			return instance;
		}
	}
}