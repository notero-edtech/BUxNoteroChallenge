using System.Linq;
using ForieroEngine.Settings;
using MoreLinq;
using UnityEngine.U2D;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.U2D;
#endif

[SettingsManager]
public class SpriteAtlasesSettings : Settings<SpriteAtlasesSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/SpriteAtlas", false, -1000)] public static void SpriteAtlasSettingsMenu() => Select();
    
    public override void Apply()
    {
        if (this == instance) return;
        base.Apply();
        
        AssetDatabase.FindAssets("t:SpriteAtlas")
            .Select(guid=>AssetDatabase.GUIDToAssetPath(guid))
            .Select(path=> AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path))
            .ToList()
            .ForEach(e=>e.SetIncludeInBuild(false));
        
        atlases.ForEach(e=> e.SetIncludeInBuild(true));
    }
#endif

    public SpriteAtlas[] atlases;
}
