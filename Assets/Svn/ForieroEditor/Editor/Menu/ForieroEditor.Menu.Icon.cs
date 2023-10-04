using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using MoreLinq;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
    {
	    [MenuItem("Assets/Foriero/Icon/Apply")] public static void ApplyIconToPlayerSettings() => ApplyIcon2PlayerSettingsInternal();
		
		private static void ApplyIcon2PlayerSettingsInternal()
        {
            var p = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]); 
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { AssetDatabase.LoadAssetAtPath<Texture2D>(p + "/icon.png") });
            SetStandalone(p + "/standalone");
            SetAndroid(p + "/android_26", p + "/android_25", p + "/android_legacy");
            SetIOS(p + "/ios", p + "/ios_app_icon");
            SetWSA(p + "/wsa_icon", p + "/wsa_wide_icon");
        } 
		
        private static void SetStandalone(string standalonePath)
        {
            var iconKinds = PlayerSettings.GetSupportedIconKindsForPlatform(BuildTargetGroup.Standalone);

            //if (iconKinds.Length == 0) Debug.LogError("IconKinds.Length is 0!!!");

            var sizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Standalone);
            System.Array.Sort(sizes);

            var guids = AssetDatabase.FindAssets("t:Texture2D", new string[1] { standalonePath });
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);

            List<Texture2D> foundIcons = new List<Texture2D>();
            
            if(sizes.Length == 0) Debug.LogError("ZERO");
            
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
        
        private static void SetWSA(string iconPath, string wideIconPath)
        {
           
            Regex re = new Regex(@"\d+");

            foreach (PlayerSettings.WSAImageType t in Enum.GetValues(typeof(PlayerSettings.WSAImageType)))
            {
                foreach (PlayerSettings.WSAImageScale s in Enum.GetValues(typeof(PlayerSettings.WSAImageScale)))
                {
                    MatchCollection typeMatches = re.Matches(t.ToString());
                    MatchCollection scaleMatches = re.Matches(s.ToString());

                    int size = 0;
                    int baseSize = 0;

                    if (typeMatches.Count == 0)
                    {
                        switch (t)
                        {
                            case PlayerSettings.WSAImageType.PackageLogo:
                                baseSize = 50;
                                break;
                            case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
                                baseSize = 44;
                                break;
                            case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
                                baseSize = 71;
                                break;
                            case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
                                baseSize = 150;
                                break;
                            case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
                                baseSize = 310;
                                break;
                            case PlayerSettings.WSAImageType.UWPWide310x150Logo:
                                baseSize = 310;
                                break;
                            case PlayerSettings.WSAImageType.SplashScreenImage:
                                baseSize = 620;
                                break;
                            default:
                                continue;
                        }
                    }
                    else
                    {
                        baseSize = int.Parse(typeMatches[0].Value);
                    }


                    if (s.ToString().Contains("Target"))
                    {
                        size = baseSize = int.Parse(scaleMatches[0].Value);
                    }
                    else
                    {
                        float sizeFloat = baseSize * float.Parse(scaleMatches[0].Value) / 100f;

                        float rest = sizeFloat - Mathf.FloorToInt(sizeFloat);

                        size = rest < 0.5f ? Mathf.FloorToInt(sizeFloat) : Mathf.CeilToInt(sizeFloat);
                    }

                    string assetPath = "";

                    if (t.ToString().ToLower().Contains("wide") || t.ToString().ToLower().Contains("splash"))
                    {
                        assetPath = wideIconPath;
                    }
                    else
                    {
                        assetPath = iconPath;
                    }

                    string file = size.ToString() + ".jpg";
                    string path = assetPath + "/" + file;

                    Texture2D o = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                    var log = "WSA Image SetVisualAssetsImage : " + path + " " + t.ToString() + " " + s.ToString();

                    if (o)
                    {
                        try {
                            Debug.Log(log);
                            PlayerSettings.WSA.SetVisualAssetsImage(path, t, s);
                            var setPath = PlayerSettings.WSA.GetVisualAssetsImage(t, s);
                            Debug.Log(setPath);
                        } catch (Exception) {
                            Debug.LogError(log);
                        }
                    }
                    else
                    {
                        Debug.LogError(log);
                        try
                        {
                            PlayerSettings.WSA.SetVisualAssetsImage("", t, s);
                        } catch (Exception)
                        {

                        }
                    }
                }
            }
        }
        
        private static void SetAndroid(string android26Path, string android25Path, string androidLegacyPath)
        {
            var iconKinds = PlayerSettings.GetSupportedIconKindsForPlatform(BuildTargetGroup.Android);

            foreach (var iconKind in iconKinds)
            {
                Debug.Log("IconKind : " + iconKind);
                               
                string directory = null;
                                
                if (iconKind.ToString().Contains("API 26")) directory = android26Path;
                else if (iconKind.ToString().Contains("API 25")) directory = android25Path;
                else directory = androidLegacyPath;

                if (directory == null) continue;

                var guids = AssetDatabase.FindAssets("t:Texture2D", new string[1] { directory });
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

        private static List<Texture2D> iosIconsTextures = new List<Texture2D>();
        private static List<Texture2D> iosAppIconsTextures = new List<Texture2D>();
        
        private static void SetIOS(string iosIconsPath, string iosAppIconsPath)
        {
            iosIconsTextures = new List<Texture2D>();
            iosAppIconsTextures = new List<Texture2D>();
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, GetIOSIcons(iosIconsPath, iosAppIconsPath,IconKind.Application), IconKind.Application);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, GetIOSIcons(iosIconsPath, iosAppIconsPath,IconKind.Notification), IconKind.Notification);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, GetIOSIcons(iosIconsPath, iosAppIconsPath,IconKind.Settings), IconKind.Settings);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, GetIOSIcons(iosIconsPath, iosAppIconsPath,IconKind.Spotlight), IconKind.Spotlight);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, GetIOSIcons(iosIconsPath, iosAppIconsPath,IconKind.Store), IconKind.Store);
        }
        
        private static Texture2D[] GetIOSIcons(string iosIconsPath, string iosAppIconsPath, IconKind iconKind)
        {
            List<Texture2D> iconList = new List<Texture2D>();
            int[] iconSizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.iOS, iconKind);
            
            if (iosIconsTextures == null || iosIconsTextures.Count == 0)
            {
                string iosIconsTexturesPath = iosIconsPath;

                var iosIconsTexturesGuids = AssetDatabase.FindAssets("t:texture2D", new string[1] { iosIconsTexturesPath });
                foreach (var guid in iosIconsTexturesGuids)
                {
                    iosIconsTextures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
                }
            }

            if (iosAppIconsTextures == null || iosAppIconsTextures.Count == 0)
            {
                string iosAppIconsTexturesPath = iosAppIconsPath;

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