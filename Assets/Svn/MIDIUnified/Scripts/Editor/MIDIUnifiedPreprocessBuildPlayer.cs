using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using ForieroEditor.Extensions;

public class MIDIUnifiedPreprocessBuildPlayer : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport buildReport)
    {
        MIDISoundFontSettingsInspector.CheckSoundFontInResources();
#if UNITY_IOS

#if BASS24_DISABLED
        var include = false;
#else
        var include = true;
#endif

        var pluginsA = Directory.GetFiles(Directory.GetCurrentDirectory(), "libbass*.a", SearchOption.AllDirectories);
        foreach (var p in pluginsA)
        {
            var a = p.GetAssetPathFromFullPath();
            var pluginA = (AssetImporter.GetAtPath(a) as PluginImporter);
            if (pluginA)
            {
                pluginA.SetCompatibleWithPlatform(BuildTarget.iOS, include);
                pluginA.SetCompatibleWithPlatform(BuildTarget.tvOS, include);
                //plugin.SetCompatibleWithPlatform(BuildTarget.iPadOS, MIDISoundSettings.instance.init);
            }
        }

        var pluginsH = Directory.GetFiles(Directory.GetCurrentDirectory(), "bass*.h", SearchOption.AllDirectories);
        foreach (var p in pluginsH)
        {
            var a = p.GetAssetPathFromFullPath();
            if (!a.Contains("iOS")) continue;
            var pluginH = (AssetImporter.GetAtPath(a) as PluginImporter);
            if (pluginH)
            {
                pluginH.SetCompatibleWithPlatform(BuildTarget.iOS, include);
                pluginH.SetCompatibleWithPlatform(BuildTarget.tvOS, include);
                //plugin.SetCompatibleWithPlatform(BuildTarget.iPadOS, MIDISoundSettings.instance.init);
            }
        }

        var net = Directory.GetFiles(Directory.GetCurrentDirectory(), "Bass.Net.iOS.dll", SearchOption.AllDirectories)[0].GetAssetPathFromFullPath();
        var pluginNet = (AssetImporter.GetAtPath(net) as PluginImporter);
        if (pluginNet)
        {
            pluginNet.SetCompatibleWithPlatform(BuildTarget.iOS, include);
            pluginNet.SetCompatibleWithPlatform(BuildTarget.tvOS, include);
        }
#endif
    }
    
    [InitializeOnLoad()] private class EditorStart
    {
        static EditorStart()
        {
            EditorApplication.playModeStateChanged += (s) => { if (s == PlayModeStateChange.ExitingEditMode) MIDISoundFontSettingsInspector.CheckSoundFontInResources(); };
        }
    }    	   
}