using System;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEngine;


namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        [Serializable]
        public class PlatformVersioniOS
        {
            public UnityEngine.Object iosIconsDirectory;
            public UnityEngine.Object iosAppIconsDirectory;

            List<Texture2D> iosIconsTextures = new List<Texture2D>();
            List<Texture2D> iosAppIconsTextures = new List<Texture2D>();

            public bool OnGUIBuild()
            {
                return false;
            }

            public void Build(string buildPath, string projectName, string[] scenes)
            {

            }

            public bool OnGUIIcons()
            {
                iosIconsDirectory = EditorGUILayout.ObjectField("ios", iosIconsDirectory, typeof(UnityEngine.Object), false);
                iosAppIconsDirectory = EditorGUILayout.ObjectField("ios_app_icons", iosAppIconsDirectory, typeof(UnityEngine.Object), false);
                return false;
            }

            public Texture2D[] GetIcons(IconKind iconKind)
            {
                List<Texture2D> iconList = new List<Texture2D>();
                int[] iconSizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.iOS, iconKind);

                if (iosIconsTextures == null || iosIconsTextures.Count == 0)
                {
                    string iosIconsTexturesPath = AssetDatabase.GetAssetPath(iosIconsDirectory);

                    var iosIconsTexturesGuids = AssetDatabase.FindAssets("t:texture2D", new string[1] { iosIconsTexturesPath });
                    foreach (var guid in iosIconsTexturesGuids)
                    {
                        iosIconsTextures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
                    }
                }

                if (iosAppIconsTextures == null || iosAppIconsTextures.Count == 0)
                {
                    string iosAppIconsTexturesPath = AssetDatabase.GetAssetPath(iosAppIconsDirectory);

                    var iosAppIconsTexturesGuids = AssetDatabase.FindAssets("t:texture2D", new string[1] { iosAppIconsTexturesPath });
                    foreach (var guid in iosAppIconsTexturesGuids)
                    {
                        iosAppIconsTextures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
                    }
                }

                foreach (var size in iconSizes)
                {
                    var t = iosIconsTextures.FirstOrDefault(i => i.width == size);
                    if (t == null)
                    {
                        t = iosAppIconsTextures.FirstOrDefault(i => i.width == size);
                    }

                    if (t == null) Debug.LogError("iOS icon size not found : " + size.ToString());
                    else iconList.Add(t);
                }

                return iconList.ToArray();
            }
        }
    }
}