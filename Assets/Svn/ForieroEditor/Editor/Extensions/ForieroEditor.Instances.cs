using System.IO;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor
{
	public static class Instances
	{
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        public static T EditorInstance<T>(string name, string assetPath = "Assets/Editor/", bool newInstance = false) where T : ScriptableObject
		{
			if (!string.IsNullOrEmpty(assetPath))
			{
				assetPath = assetPath.StartsWith("/", System.StringComparison.Ordinal) ? assetPath.Remove(0, 1) : assetPath;
				assetPath = assetPath.EndsWith("/", System.StringComparison.Ordinal) ? assetPath : assetPath + "/";
			}

			var path = assetPath + name + ".asset";
			//Debug.Log ("FResources Loading : " + path);

			T instance = AssetDatabase.LoadAssetAtPath<T>(path);

			if (newInstance || instance == null)
			{
				if (Application.isPlaying)
				{
					Debug.LogError(name + " did not existed, creating one in memory!");
				}

				instance = ScriptableObject.CreateInstance<T>();

				var resourcesPath = path;
				//Debug.Log ("FResource Path : " + resourcesPath);

				var resourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), assetPath.FixOSPath());
				//Debug.Log ("FResource Directory : " + resourceDirectory);

				if (!Directory.Exists(resourceDirectory))
				{
					Directory.CreateDirectory(resourceDirectory);
				}

                var p = AssetDatabase.GenerateUniqueAssetPath(resourcesPath);

                AssetDatabase.CreateAsset(instance, AssetDatabase.GenerateUniqueAssetPath(resourcesPath));
                AssetDatabase.SaveAssets();

                instance = AssetDatabase.LoadAssetAtPath<T>(p);
			}

			return instance;
		}

        public static T CreateAsset<T>() where T : ScriptableObject
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(path), "");
            }

            return CreateAsset<T>(path, "New " + typeof(T).Name);
        }

        public static T CreateAsset<T>(string path, string name) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }

            if (!name.EndsWith(".asset", System.StringComparison.Ordinal))
            {
                name += ".asset";
            }

            var p = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name);

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, p);
            AssetDatabase.SaveAssets();

            asset = AssetDatabase.LoadAssetAtPath<T>(p);
            return asset;
        }

        public static void SelectAssetInProjectView(Object asset)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}