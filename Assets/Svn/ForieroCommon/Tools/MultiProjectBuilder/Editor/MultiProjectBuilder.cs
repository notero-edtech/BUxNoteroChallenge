using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using ForieroEngine;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        public static MultiProjectBuilder instance
        {
            get
            {
                return Instances.EditorInstance<MultiProjectBuilder>(typeof(MultiProjectBuilder).Name);
            }
        }

        public BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
        public BuildTargetGroup targetGroupTmp = BuildTargetGroup.Unknown;

        public enum ReleaseVersion
        {
            NONE,
            DEMO,
            EARLY,
            FULL,
        }

        public enum PublishingPlatform
        {
            NONE,
            APPLEAPPSTORE,
            APPLEMACAPPSTORE,
            BETHESDA,
            EPICGAMES, 
            GOG,
            HUMBLEBUNDLE,
            ITCHIO,
            ORIGIN,
            STEAM,
            UPLAY,
        }

        public PlatformVersions platformVersions = new PlatformVersions();

        [Serializable]
        public class PlatformVersions
        {

            public BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;

            public string versionName = "";
            public string productName = "";

            public string buildPath = "";

            public Dictionary<BuildTargetGroup, int> selectedIndexes = new Dictionary<BuildTargetGroup, int>();

            public int selectedVersion
            {
                get
                {
                    if (selectedIndexes.ContainsKey(targetGroup))
                    {
                        int index = selectedIndexes[targetGroup];
                        if (index >= 0 && index < versionNames.Length)
                        {
                            return index;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        selectedIndexes.Add(targetGroup, -1);
                        return -1;
                    }
                }
                set
                {
                    if (selectedIndexes.ContainsKey(targetGroup))
                    {
                        selectedIndexes[targetGroup] = value;
                    }
                    else
                    {
                        selectedIndexes.Add(targetGroup, value);
                    }
                }
            }

            public List<PlatformVersion> versions = new List<PlatformVersion>();

            public string[] versionNames
            {
                get
                {
                    return versions.Where(i => i.targetGroup == targetGroup).Select(i => i.name).ToArray();
                }
            }

            public string[] versionGUIDs
            {
                get
                {
                    return versions.Where(i => i.targetGroup == targetGroup).Select(i => i.guid).ToArray();
                }
            }

            public List<PlatformVersion> versionItems
            {
                get
                {
                    return versions.Where(i => i.targetGroup == targetGroup).Select(i => i).ToList();
                }
            }

            public void Sort()
            {
                versions.Sort(
                   delegate (PlatformVersion x, PlatformVersion y)
                   {
                       return x.name.CompareTo(y.name);
                   }
                );
            }

            public void Add(BuildTargetGroup targetGroup, string name)
            {
                PlatformVersion platformVersion = new PlatformVersion(targetGroup, name);
                platformVersion.name = name;
                versions.Add(platformVersion);
            }
        }

        [Serializable]
        public class PlatformVersion
        {
            public BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;

            public string guid = System.Guid.NewGuid().ToString();

            public string preBuildMethodName
            {
                get
                {
                    return targetGroup.ToString() + "_" + name.Replace(" ", "").Replace("-", "_");
                }
            }

            public string name = "";
            public string productName = "";

            public bool applied = false;

            public Foriero.ProjectVersion projectVersion = Foriero.ProjectVersion.NONE;
            public bool inAppPurchases = false;

            public ReleaseVersion releaseVersion = ReleaseVersion.NONE;
            public PublishingPlatform publishingPlatform = PublishingPlatform.NONE;
            public ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
            public ApiCompatibilityLevel apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;

            public List<string> includedDefineList = new List<string>();
            public List<string> excludedDefineList = new List<string>();
            public List<ScriptableObject> settingsList = new List<ScriptableObject>();
            
            public bool convertedOldFields = false;

            public StoreSettings storeSetttings = null;
            public SceneSettings sceneSettings = null;
            public SpriteAtlasesSettings spriteAtlasesSettings = null;
            
            #region old propertiex

            public SocialSettings socialSettings = null;
            public AdsProxySettings adsSettings = null;
            public string defineSymbols = "";
            public string removeSymbols = "";
            #endregion

            public Texture2D icon;

            public string version = "1.0";
            public int build = 1;

            public PlatformVersionAndroid android = null;
            public PlatformVersioniOS ios = null;
            public PlatformVersionStandalone standalone = null;
            public PlatformVersionWSA wsa = null;

            public PlatformVersion(BuildTargetGroup targetGroup, string name)
            {
                this.targetGroup = targetGroup;
                this.name = name;
                switch (targetGroup)
                {
                    case BuildTargetGroup.iOS:
                        ios = new PlatformVersioniOS();
                        break;
                    case BuildTargetGroup.Android:
                        android = new PlatformVersionAndroid();
                        break;
                    case BuildTargetGroup.Standalone:
                        standalone = new PlatformVersionStandalone();
                        break;
                    case BuildTargetGroup.WSA:
                        wsa = new PlatformVersionWSA();
                        break;
                }
            }

            public void Build(string buildPath)
            {
                switch (targetGroup)
                {
                    case BuildTargetGroup.iOS:
                        if (ios != null)
                        {
                            //ios.Build ();
                        }
                        break;
                    case BuildTargetGroup.Android:
                        if (android != null)
                        {
                            //android.Build ();
                        }
                        break;
                    case BuildTargetGroup.Standalone:
                        if (standalone != null)
                        {
                            //
                        }
                        break;
                }
            }

            ReorderableList rlIncludeSymbols;
            ReorderableList rlExcludeSymbols;
            ReorderableList rlSettingsList;
            
            void InitReorderableLists()
            {
                if (rlSettingsList == null || rlSettingsList.list != settingsList)
                {
                    rlSettingsList = new ReorderableList(settingsList, typeof(ScriptableObject), true, true, true, true);

                    rlSettingsList.drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect, "Settings");
                    };

                    rlSettingsList.onAddCallback = (list) =>
                    {
                        settingsList.Add(null);
                    };

                    rlSettingsList.onRemoveCallback = (list) =>
                    {
                        settingsList.RemoveAt(rlSettingsList.index);
                    };

                    rlSettingsList.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.y += 2;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        settingsList[index] = EditorGUI.ObjectField(rect, settingsList[index], typeof(ScriptableObject), false) as ScriptableObject;
                    };
                }

                if (rlIncludeSymbols == null || rlIncludeSymbols.list != includedDefineList)
                {
                    rlIncludeSymbols = new ReorderableList(includedDefineList, typeof(string), true, true, true, true);

                    rlIncludeSymbols.drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect, "Included Defines");
                    };

                    rlIncludeSymbols.onAddCallback = (list) =>
                    {
                        includedDefineList.Add("");
                    };

                    rlIncludeSymbols.onRemoveCallback = (list) =>
                    {
                        includedDefineList.RemoveAt(rlIncludeSymbols.index);
                    };

                    rlIncludeSymbols.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.y += 2;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        includedDefineList[index] = EditorGUI.TextField(rect, includedDefineList[index]);
                    };
                }

                if (rlExcludeSymbols == null || rlExcludeSymbols.list != excludedDefineList)
                {
                    rlExcludeSymbols = new ReorderableList(excludedDefineList, typeof(string), true, true, true, true);

                    rlExcludeSymbols.drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect, "Excluded Defines");
                    };

                    rlExcludeSymbols.onAddCallback = (list) =>
                    {
                        excludedDefineList.Add("");
                    };

                    rlExcludeSymbols.onRemoveCallback = (list) =>
                    {
                        excludedDefineList.RemoveAt(rlExcludeSymbols.index);
                    };

                    rlExcludeSymbols.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.y += 2;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        excludedDefineList[index] = EditorGUI.TextField(rect, excludedDefineList[index]);
                    };
                }
            }

            void ConvertOldFields()
            {
                //if (storeSetttings != null) settingsList.Add(storeSetttings);
                if (socialSettings != null) settingsList.Add(socialSettings);
                if (adsSettings != null) settingsList.Add(adsSettings);

                string[] includes = defineSymbols.Split(';');
                if (includes.Length > 0) includedDefineList.AddRange(includes);

                string[] excludes = removeSymbols.Split(';');
                if (excludes.Length > 0) excludedDefineList.AddRange(excludes);

                convertedOldFields = true;
            }

            public bool OnGUI()
            {
                bool isDirty = false;

                if (!convertedOldFields)
                {
                    ConvertOldFields();
                    isDirty = true;
                }

                InitReorderableLists();

              
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("GUID");
                GUILayout.FlexibleSpace();
                GUILayout.Label(guid);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("PreBuildMethodName");
                GUILayout.FlexibleSpace();
                GUILayout.TextField(preBuildMethodName);
                if (GUILayout.Button("C", GUILayout.Width(30)))
                {
                    TextEditor te = new TextEditor();
                    te.text = preBuildMethodName;
                    te.SelectAll();
                    te.Copy();
                }
                EditorGUILayout.EndHorizontal();

                name = EditorGUILayout.TextField("Version Name", name);
                if (storeSetttings)
                {
                    EditorGUILayout.HelpBox($"Product Name is defined in StoreSettings: {storeSetttings.appName}", MessageType.Info);                    
                }
                else
                {
                    EditorGUILayout.HelpBox($"Project Name is defined in PlayerSettings: {PlayerSettings.productName}", MessageType.Info);
                }
                
                productName = EditorGUILayout.TextField("Product Name", productName);

                icon = EditorGUILayout.ObjectField("Icon", icon, typeof(Texture2D), false) as Texture2D;
                version = EditorGUILayout.TextField("Version", version);
                projectVersion = (Foriero.ProjectVersion)EditorGUILayout.EnumPopup("Project Version", projectVersion);
                releaseVersion = (ReleaseVersion)EditorGUILayout.EnumPopup("Release Version", releaseVersion);
                publishingPlatform = (PublishingPlatform)EditorGUILayout.EnumPopup("Publishing Platform", publishingPlatform);
                scriptingImplementation = (ScriptingImplementation)EditorGUILayout.EnumPopup("Scripting Implementation", scriptingImplementation);
                apiCompatibilityLevel = (ApiCompatibilityLevel)EditorGUILayout.EnumPopup("API Compatibility Level", apiCompatibilityLevel);
                
                inAppPurchases = EditorGUILayout.Toggle("In App Purchases", inAppPurchases);
                EditorGUILayout.BeginHorizontal();
                build = EditorGUILayout.IntField("Build", build);
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    build--;
                    isDirty = true;
                }
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    build++;
                    isDirty = true;
                }

                EditorGUILayout.EndHorizontal();

                if (storeSetttings == null)
                {
                    EditorGUILayout.HelpBox("You have to specify 'StoreSettings'!", MessageType.Error);
                }

                storeSetttings = EditorGUILayout.ObjectField("Store Settings", storeSetttings, typeof(StoreSettings), false) as StoreSettings;
                sceneSettings = EditorGUILayout.ObjectField("Scene Settings", sceneSettings, typeof(SceneSettings), false) as SceneSettings;
                spriteAtlasesSettings = EditorGUILayout.ObjectField("Sprite Atlases Settings", spriteAtlasesSettings, typeof(SpriteAtlasesSettings), false) as SpriteAtlasesSettings;

                switch (targetGroup)
                {
                    case BuildTargetGroup.iOS:
                        if (ios != null && ios.OnGUIBuild())
                        {
                            isDirty = true;
                        }
                        break;
                    case BuildTargetGroup.Android:
                        if (android != null && android.OnGUIBuild())
                        {
                            isDirty = true;
                        }
                        break;
                    case BuildTargetGroup.Standalone:
                        if (standalone != null && standalone.OnGUIBuild())
                        {
                            isDirty = true;
                        }
                        break;
                    case BuildTargetGroup.WSA:
                        if (wsa != null && wsa.OnGUIBuild())
                        {
                            isDirty = true;
                        }
                        break;
                }

                GUILayout.Label("Icons");

                switch (targetGroup)
                {
                    case BuildTargetGroup.iOS:
                        if (ios != null && ios.OnGUIIcons())
                        {
                            isDirty = true;
                        }
                        break;
                    case BuildTargetGroup.Android:
                        if (android != null && android.OnGUIIcons())
                        {
                            isDirty = true;
                        }
                        break;
                    case BuildTargetGroup.Standalone:
                        if (standalone != null && standalone.OnGUIIcons())
                        {
                            isDirty = true;
                        }
                        break;
                    case BuildTargetGroup.WSA:
                        if (wsa != null && wsa.OnGUIIcons())
                        {
                            isDirty = true;
                        }
                        break;
                }

                rlSettingsList.DoLayoutList();
                rlIncludeSymbols.DoLayoutList();
                rlExcludeSymbols.DoLayoutList();
                
                if (sceneSettings == null)
                {
                    EditorGUILayout.HelpBox("Currently active scenes in Build Settings are included into Build!", MessageType.Warning);
                }
               
                return isDirty;
            }
        }
    }
}