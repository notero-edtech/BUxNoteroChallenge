using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        void SetPlatformStandalone(MultiProjectBuilder.PlatformVersion platformVersion)
        {
            if (platformVersion.targetGroup != BuildTargetGroup.Standalone)
                return;

            PlayerSettings.useMacAppStoreValidation = platformVersion.standalone.macAppStoreValidation;
            
            var iconKinds = PlayerSettings.GetSupportedIconKindsForPlatform(BuildTargetGroup.Standalone);

            //if (iconKinds.Length == 0) Debug.LogError("IconKinds.Length is 0!!!");

            var sizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Standalone);
            System.Array.Sort(sizes);

            var guids = AssetDatabase.FindAssets("t:Texture2D", new string[1] { AssetDatabase.GetAssetPath(platformVersion.standalone.standaloneDirectory) });
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);

            PlayerSettings.macOS.buildNumber = platformVersion.build.ToString();
            if (EditorUserBuildSettings.activeBuildTarget != platformVersion.standalone.GetBuildTarget())
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, platformVersion.standalone.GetBuildTarget());    
            
            PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1);
#if UNITY_EDITOR_OSX
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = (UnityEditor.OSXStandalone.MacOSArchitecture)platformVersion.standalone.architecture;
#endif
            
            List<Texture2D> foundIcons = new List<Texture2D>();

            foreach (int size in sizes)
            {
                var end = "x" + size.ToString() + ".png";
                paths.ForEach((p) =>
                {
                    if (p.Contains(end))
                    {
                        foundIcons.Insert(0, AssetDatabase.LoadAssetAtPath<Texture2D>(p));
                    }
                });
            }

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, foundIcons.ToArray());
        }
    }         
}