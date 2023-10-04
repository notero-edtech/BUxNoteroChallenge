#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using ForieroEditor.Extensions;

public static class SMEditor 
{
    static AudioClip emptyAudioClip = null;

    static AudioClip InitEmptyAudioClip()
    {
        if (!emptyAudioClip)
        {
            string[] assets = AssetDatabase.FindAssets("t:AudioClip empty");
            if (assets != null && assets.Length >= 1)
            {
                emptyAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(assets[0]));
            }
        }

        return emptyAudioClip;
    }

    [MenuItem("Assets/Sound Manager/Create SoundFX Placeholder %#e")]
    public static void CreateSoundFXPlaceholder()
    {
        if (!InitEmptyAudioClip()) return;
        CreateSoundFXPlaceholder("placeholder");        
    }

    public static void CreateSoundFXPlaceholder(string fileNameWithoutExtension, string assetPath = "")
    {
        if (!InitEmptyAudioClip()) return;
        emptyAudioClip?.SaveObject(fileNameWithoutExtension + ".ogg", assetPath);
    }

    public static void SaveObject<T>(this T t, string fileName, string assetPath = "") where T : Object
    {
        if (string.IsNullOrEmpty(assetPath)) assetPath = GetSelectedPathOrFallback(); 
                
        assetPath += (assetPath.EndsWith("/", System.StringComparison.Ordinal) ? "" : "/") + fileName;

        string directory = Path.GetDirectoryName(assetPath.GetFullPathFromAssetPath());
                
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
              
        if (AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath)) return;

        string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                      
        Debug.Log("<color=pink>Creating placeholder : " + uniqueAssetPath + "</color>");

        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(t), uniqueAssetPath);

        if (!Application.isPlaying)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

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
}

#endif
