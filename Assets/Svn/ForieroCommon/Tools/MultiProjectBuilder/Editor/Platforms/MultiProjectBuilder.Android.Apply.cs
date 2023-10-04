using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;

using Debug = UnityEngine.Debug;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
       void SetPlatformAndroid(MultiProjectBuilder.PlatformVersion platformVersion, StoreSettings storeSettings)
        {
            if (platformVersion.targetGroup != BuildTargetGroup.Android)
                return;

            PlayerSettings.Android.bundleVersionCode = platformVersion.build;

            if (storeSettings)
            {
                #if FORIERO_INAPP
                storeSettings._appStore = platformVersion.android.store.ToString();
                #endif
                EditorUtility.SetDirty(storeSettings);
                AssetDatabase.SaveAssets();
            }

            var iconKinds = PlayerSettings.GetSupportedIconKindsForPlatform(BuildTargetGroup.Android);

            foreach (var iconKind in iconKinds)
            {
                Debug.Log("IconKind : " + iconKind);
                               
                Object directory = null;
                                
                if (iconKind.ToString().Contains("API 26")) directory = platformVersion.android.android26Directory;
                else if (iconKind.ToString().Contains("API 25")) directory = platformVersion.android.android25Directory;
                else directory = platformVersion.android.androidLegacyDirectory;

                if (directory == null) continue;

                var guids = AssetDatabase.FindAssets("t:Texture2D", new string[1] { AssetDatabase.GetAssetPath(directory) });
                var paths = guids.Select(AssetDatabase.GUIDToAssetPath);

                var platformIcons = PlayerSettings.GetPlatformIcons(BuildTargetGroup.Android, iconKind);
                foreach (var platformIcon in platformIcons)
                {
                    List<Texture2D> foundIcons = new List<Texture2D>();
                    var end = "x" + platformIcon.width.ToString() + ".png";
                    paths.ForEach((p) =>
                    {
                        if (p.Contains(end))
                        {
                            foundIcons.Insert(0, AssetDatabase.LoadAssetAtPath<Texture2D>(p));
                        }
                    });

                    switch (foundIcons.Count)
                    {
                        case 1:
                            platformIcon.SetTexture(foundIcons[0]);
                            break;
                        case 2:
                            platformIcon.SetTextures(foundIcons.ToArray());
                            break;
                    }
                }

                PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android, iconKind, platformIcons);
            }
        }
    }
}