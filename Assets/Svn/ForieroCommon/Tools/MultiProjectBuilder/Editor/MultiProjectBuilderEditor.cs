using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public class BuildVersionsEditor : EditorWindow
    {
        [MenuItem("Foriero/Tools/Multi Project Builder", false, -2000)]
        public static void Init()
        {
            window = (BuildVersionsEditor)EditorWindow.GetWindow(typeof(BuildVersionsEditor));

            window.titleContent = new GUIContent("Project Builder");

            InitInternal();

            EditorGUIUtility.PingObject(mpb);
            Selection.objects = new Object[1] { mpb };
        }

        static MultiProjectBuilderDummy wrapper;
        static MultiProjectBuilder _mpb;

        public static MultiProjectBuilder mpb
        {
            get
            {
                return _mpb;
            }
            set
            {
                _mpb = value;
                wrapper.mpb = value;
            }
        }

        static Editor e;
        static BuildVersionsEditor window;

        Vector2 scroll = Vector2.zero;
               
        public static void InitInternal()
        {
            wrapper = ScriptableObject.CreateInstance<MultiProjectBuilderDummy>();
            wrapper.mpb = MultiProjectBuilder.instance;
            e = Editor.CreateEditor(wrapper);
            mpb = MultiProjectBuilder.instance;
        }

        void OnEnable()
        {
            InitInternal();
        }

        void OnDisable()
        {
            mpb = null;
            e = null;
            if (BuildVersionsEditor.window != null)
            {
                BuildVersionsEditor.window.Repaint();
            }
        }

        BuildTargetGroup copyToTargetGroup = BuildTargetGroup.Unknown;

        bool apply = false;

        Color backgroundColor = Color.black;
                
        void OnGUI()
        {

            backgroundColor = GUI.backgroundColor;

            apply = false;

            if (wrapper == null && e == null)
                return;

            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("Settings"))
            {
                EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }

            GUI.enabled = mpb.platformVersions.selectedVersion >= 0 && mpb.platformVersions.selectedVersion < mpb.platformVersions.versionNames.Length;

            MultiProjectBuilder.PlatformVersion platformVersion = null;

            if (GUI.enabled)
            {
                string guid = mpb.platformVersions.versionGUIDs[mpb.platformVersions.selectedVersion];
                for (int i = mpb.platformVersions.versions.Count - 1; i >= 0; i--)
                {
                    if (mpb.platformVersions.versions[i].guid == guid)
                    {
                        platformVersion = mpb.platformVersions.versions[i];
                    }
                }
            }

            Color color = GUI.backgroundColor;

            if (GUI.enabled && !platformVersion.applied)
            {
                GUI.backgroundColor = Color.red;
            }

            if (GUILayout.Button("Apply"))
            {
                for (int i = mpb.platformVersions.versions.Count - 1; i >= 0; i--)
                {
                    mpb.platformVersions.versions[i].applied = false;
                }

                mpb.SetPlatform(platformVersion);

                apply = true;

                EditorUtility.SetDirty(mpb);
            }

            if (GUI.enabled && platformVersion.applied)
            {
                GUI.backgroundColor = Color.green;
            }

            if (GUILayout.Button("Build"))
            {
                if (platformVersion.applied)
                {
                    platformVersion.Build(mpb.platformVersions.buildPath);
                }
                else
                {
                    Debug.LogError("Apply platform first!");
                }
            }

            GUI.backgroundColor = color;

            if (GUILayout.Button("Set Dirty"))
            {
                EditorUtility.SetDirty(mpb);
            }

            GUI.enabled = true;

            if (GUILayout.Button("Generate Static Build Methods"))
            {
                GenerateStaticBuildMethods();
            }

            EditorGUILayout.EndHorizontal();

            #region copy to

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Copy projects to : "))
            {

            }

            copyToTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(copyToTargetGroup, GUILayout.Width(100));

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            #endregion

            EditorGUILayout.BeginHorizontal();
            mpb.platformVersions.buildPath = EditorGUILayout.TextField(mpb.platformVersions.buildPath);
            if (GUILayout.Button("..", GUILayout.Width(30)))
            {
                string path = "";
                if (!string.IsNullOrEmpty(path = EditorUtility.SaveFolderPanel("Build Path", mpb.platformVersions.buildPath, "")))
                {
                    mpb.platformVersions.buildPath = path;
                    EditorUtility.SetDirty(mpb);
                }
            }
            EditorGUILayout.EndHorizontal();
                        
            EditorGUILayout.BeginHorizontal();
            mpb.targetGroup = mpb.platformVersions.targetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(mpb.targetGroup, GUILayout.Width(100));
            if (mpb.targetGroup == BuildTargetGroup.Unknown)
                GUI.enabled = false;

            mpb.platformVersions.selectedVersion = EditorGUILayout.Popup(mpb.platformVersions.selectedVersion, mpb.platformVersions.versionNames);

            mpb.platformVersions.versionName = EditorGUILayout.TextField(mpb.platformVersions.versionName);
            GUI.SetNextControlName("ADD_BUTTON");
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                if (!string.IsNullOrEmpty(mpb.platformVersions.versionName))
                {
                    mpb.platformVersions.Add(mpb.targetGroup, mpb.platformVersions.versionName);
                    mpb.platformVersions.versionName = "";
                    GUI.FocusControl("ADD_BUTTON");
                    EditorUtility.SetDirty(mpb);
                }
            }

            GUI.enabled = mpb.platformVersions.selectedVersion >= 0 && mpb.platformVersions.selectedVersion < mpb.platformVersions.versionNames.Length;
            {
                if (GUILayout.Button("-"))
                {
                    if (EditorUtility.DisplayDialog("Delete item!",
                        "Delete item : " + mpb.platformVersions.versionNames[mpb.platformVersions.selectedVersion],
                        "Yes", "No"))
                    {
                        for (int i = mpb.platformVersions.versions.Count - 1; i >= 0; i--)
                        {
                            if (mpb.platformVersions.versions[i].guid == platformVersion.guid)
                            {
                                mpb.platformVersions.versions.RemoveAt(i);
                                EditorUtility.SetDirty(mpb);
                            }
                        }
                    }
                }

                // if (GUILayout.Button("Duplicate"))
                // {
                //     var v = mpb.platformVersions.versions[mpb.platformVersions.selectedVersion];
                //     var p = v.Clone();
                //     p.name += " NEW";
                //     mpb.platformVersions.versions.Add(p);
                //     EditorUtility.SetDirty(mpb);   
                // }
            }
            GUI.enabled = true;

            
            
            if (GUILayout.Button("Sort", GUILayout.ExpandWidth(true)))
            {
                mpb.platformVersions.Sort();
            }
            
            EditorGUILayout.EndHorizontal();

            if (platformVersion != null)
            {
                scroll = EditorGUILayout.BeginScrollView(scroll);
                if (platformVersion.OnGUI())
                {
                    EditorUtility.SetDirty(mpb);
                }
                EditorGUILayout.EndScrollView();
            }

            if (apply) AssetDatabase.Refresh();
        }

        string NewLine
        {
            get
            {
                return System.Environment.NewLine;
            }
        }

        string Tab
        {
            get
            {
                return "\t";
            }
        }

        void GenerateStaticBuildMethods()
        {
            string path = Path.Combine(Application.dataPath, "Editor/MultiProjectBuilderStaticMethods.cs");

            Debug.Log("Generating : " + path);

            string s = "";

            s += "using UnityEngine;" + NewLine;
            s += "using UnityEditor;" + NewLine;
            s += "using ForieroEditor.Tools.MultiProjectBuilder;" + NewLine;
            s += "public static class MultiProjectBuilderStaticMethods{" + NewLine + NewLine;

            foreach (var p in mpb.platformVersions.versions)
            {
                s += Tab + "[MenuItem(\"Foriero/Builds/" + p.targetGroup + "/" + p.name + "\")]" + NewLine;

                s += Tab + "public static void " + p.preBuildMethodName + "(){" + NewLine;

                s += Tab + Tab + "MultiProjectBuilder.instance.SetPlatform(\"" + p.guid + "\");" + NewLine;

                s += Tab + "}" + NewLine + NewLine;
            }

            s += "}";

            if (Directory.Exists(Path.GetDirectoryName(path)))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            File.WriteAllText(path, s);

            AssetDatabase.Refresh();
        }
    }
}