using System.IO;
using ForieroEditor.Extensions;
using Sirenix.Serialization;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

using static RecorderEditorData;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

public partial class RecorderEditor : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Foriero/Tools/Recorder", false, -2000)]
    static void RecorderEditorMenu()
    {
        // Get existing open window or if none, make a new one:
        RecorderEditor window = (RecorderEditor)EditorWindow.GetWindow(typeof(RecorderEditor));
        window.titleContent = new GUIContent("Recorder");
        window.Show();
    }

    ReorderableList rlGroups;
    ReorderableList rlItems;
        
    RecordGroup selectedGroup = null;
    RecordItem selectedItem = null;

    static RecorderEditorData data => RecorderEditorData.data;
    
    private void OnEnable()
    {            
        InitGroups();
        EditorApplication.playModeStateChanged += (s) =>
        {
            if(s == PlayModeStateChange.EnteredEditMode) data.ProcessImage2MOV();
        };        
    }

    Color c;
    bool recording = false;

    void HSeparator(int height = 3) => GUILayout.Box("", GUILayout.Height(height), GUILayout.ExpandWidth(true));
            
    void OnGUI()
    {
        c = GUI.backgroundColor;
        recording = data.HasRecordFlag() || data.HasRecordingFlag();
               
        foreach(var g in data.groups)
        {
            if (g.HasRecordingFlag() && g != selectedGroup)
            {
                selectedGroup = g;
                InitItems(g);
            }
        }

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("RecordingControllerSettings"))
        {
            var s = ScriptableObject.CreateInstance<RecorderControllerSettings>();            
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Editor/Recorder/Settings/RecordingControllerSettings.asset");
            AssetDatabase.CreateAsset(s, path);
            EditorGUIUtility.PingObject(s);
        }

        if (GUILayout.Button("MovieRecorderSettings"))
        {
            var s = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Editor/Recorder/Settings/MovieRecorderSettings.asset");
            AssetDatabase.CreateAsset(s, path);
            EditorGUIUtility.PingObject(s);
        }

        if (GUILayout.Button("ImageRecorderSettings"))
        {
            var s = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Editor/Recorder/Settings/ImageRecorderSettings.asset");
            AssetDatabase.CreateAsset(s, path);
            EditorGUIUtility.PingObject(s);
        }

        if (GUILayout.Button("AudioRecorderSettings"))
        {
            var s = ScriptableObject.CreateInstance<RecorderAudioSettings>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Editor/Recorder/Settings/AudioRecorderSettings.asset");
            AssetDatabase.CreateAsset(s, path);
            EditorGUIUtility.PingObject(s);
        }

        EditorGUILayout.EndHorizontal();

        HSeparator();

        EditorGUILayout.BeginHorizontal();
        cloudPath = EditorGUILayout.TextField("Cloud Path : ", cloudPath);
        if (GUILayout.Button("...", EditorStyles.toolbarButton, GUILayout.Width(40)))
        {
            var p = EditorUtility.OpenFolderPanel("Cloud Path", "", "");
            cloudPath = string.IsNullOrEmpty(p) ? cloudPath : p;
        }
        EditorGUILayout.EndHorizontal();

        HSeparator();

        GUILayout.BeginHorizontal();
        {
            GUI.enabled = !recording;
            {
                if (GUILayout.Button("RECORD ALL", EditorStyles.toolbarButton))
                {
                    data.ClearRecordFlag();
                    data.ClearRecordingFlag();
                    foreach (var g in data.groups) if (g.included) g.FlagIncluded();
                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssets();
                }
            }
            GUI.enabled = true;

            GUI.enabled = recording;
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("STOP ALL", EditorStyles.toolbarButton))
                {
                    data.ClearRecordFlag();
                    data.ClearRecordingFlag();
                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssets();
                    Recorder.Stop(false);
                }
                GUI.backgroundColor = c;
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();

        HSeparator();

        rlGroups?.DoLayoutList();

        if (selectedGroup != null) GUILayout.Box(selectedGroup.GetPath(), EditorStyles.helpBox, GUILayout.ExpandWidth(true));
        if (selectedItem != null)
        {
            GUILayout.Box(selectedItem.GetPath(), EditorStyles.helpBox, GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            foreach(var w in System.Enum.GetNames(typeof(Wildcards)))
            {
                if(GUILayout.Button(w, EditorStyles.toolbarButton))
                {
                    string wc = "";                    
                    switch ((Wildcards)System.Enum.Parse(typeof(Wildcards),w))
                    {
                        case Wildcards.Recorder: wc = DefaultWildcard.Recorder; break;
                        case Wildcards.Time: wc = DefaultWildcard.Time; break;                            
                        case Wildcards.Take: wc = DefaultWildcard.Take; break;
                        case Wildcards.Date: wc = DefaultWildcard.Date; break;
                        case Wildcards.Project: wc = DefaultWildcard.Project; break;
                        case Wildcards.Product: wc = DefaultWildcard.Product; break;
                        case Wildcards.Scene: wc = DefaultWildcard.Scene; break;
                        case Wildcards.Resolution: wc = DefaultWildcard.Resolution; break;
                        case Wildcards.Frame: wc = DefaultWildcard.Frame; break;
                        case Wildcards.Extension: wc = DefaultWildcard.Extension; break;

                    }
                    selectedItem.name += "<" + w + ">";
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        rlItems?.DoLayoutList();

        if (!EditorApplication.isPlaying)
        {
            if (data.HasRecordFlag())
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(data.GetCurrent().scene), OpenSceneMode.Single);
                EditorApplication.isPaused = true;
                EditorApplication.isPlaying = true;
            }
        }

        if (GUI.changed) EditorUtility.SetDirty(data);
    }

    private void Update()
    {
        if (EditorApplication.isPlaying && EditorApplication.isPaused) RecorderInternal.Start();        
    }

    void InitGroups()
    {
        rlGroups = new ReorderableList(data.groups, typeof(RecordGroup), true, true, true, true);

        rlGroups.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Include, Record, Locate, Name, Path Settings, Record Settings"); };

        rlGroups.drawElementCallback = (rect, index, isactive, isfocused) =>
        {
            float tw = 0;
            float w = 25;

            data.groups[index].included = EditorGUI.Toggle(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].included);

            tw += w; w = 50;
            GUI.enabled = !EditorApplication.isPlaying;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Record", EditorStyles.toolbarButton))
            {
                data.ClearRecordFlag();

                data.groups[index].FlagIncluded();

                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
            GUI.enabled = true;

            tw += w; w = 25;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "F", EditorStyles.toolbarButton))
            {
                EditorUtility.OpenWithDefaultApp(data.groups[index].GetPath());
            }
            tw += w; w = 25;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "D", EditorStyles.toolbarButton))
            {                
                data.groups.Insert(index, (RecordGroup)SerializationUtility.CreateCopy(rlGroups.list[index]));
                EditorUtility.SetDirty(data);
            }

            tw += w; w = (rect.width - 250) / 4f;
            data.groups[index].name = EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].name);

            tw += w; w = 55;
            data.groups[index].recordPathOptions = (RecordPathOptions)EditorGUI.EnumPopup(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].recordPathOptions);

            tw += w; w = 20;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "...", EditorStyles.toolbarButton))
            {
                var p = EditorUtility.OpenFolderPanel("Path", data.groups[index].GetRootPath(), "");
                if (!string.IsNullOrEmpty(p))
                {
                    switch (data.groups[index].recordPathOptions)
                    {
                        case RecordPathOptions.Cloud:                            
                        case RecordPathOptions.Project:
                            data.groups[index].path = p.Replace(data.groups[index].GetRootPath(), "");
                            break;
                        case RecordPathOptions.Absolute:
                            data.groups[index].path = p;
                            break;
                    }
                }
                
            }

            tw += w; w = (rect.width - 250) / 4f;
            data.groups[index].path = EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].path);

            tw += w; w = (rect.width - 250) / 4f;
            switch (data.groups[index].recordingOptions)
            {
                case RecordingOptions.Movie:        
                case RecordingOptions.Image:
                case RecordingOptions.Audio:
                    data.groups[index].recorderControllerSettings = (RecorderControllerSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].recorderControllerSettings, typeof(RecorderControllerSettings), false);                    
                    break;
            }

            tw += w; w = 50;
            data.groups[index].recordingOptions = (RecordingOptions)EditorGUI.EnumPopup(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].recordingOptions);

            tw += w; w = (rect.width - 250) / 4f;
            switch (data.groups[index].recordingOptions)
            {
                case RecordingOptions.Movie:                    
                    data.groups[index].movieRecorderSettings = (MovieRecorderSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].movieRecorderSettings, typeof(MovieRecorderSettings), false);
                    break;
                case RecordingOptions.Image:                    
                    data.groups[index].imageRecorderSettings = (ImageRecorderSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].imageRecorderSettings, typeof(ImageRecorderSettings), false);                    
                    break;
                case RecordingOptions.Audio:                    
                    data.groups[index].audioRecorderSettings = (RecorderAudioSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), data.groups[index].audioRecorderSettings, typeof(RecorderAudioSettings), false);
                    break;
            }
        };

        rlGroups.onSelectCallback = (l) =>
        {
            if (l.index >= 0 && l.index < data.groups.Count)
            {              
                InitItems(data.groups[l.index]);
            }
            else
            {
                selectedGroup = null;
                selectedItem = null;
                rlItems = null;
            }

        };
    }

    void InitItems(RecordGroup recordGroup)
    {
        selectedGroup = recordGroup;

        rlItems = new ReorderableList(selectedGroup.items, typeof(RecordItem), true, true, true, true);

        rlItems.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Include, Record, Locate, Open, Name, Scene, Settings"); };

        rlItems.drawElementCallback = (rect, index, isactive, isfocused) =>
        {
            float tw = 0;
            float w = 25;

            selectedGroup.items[index].included = EditorGUI.Toggle(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].included);

            tw += w; w = 50;
            if (selectedGroup.items[index].recording)
            {
                if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Stop", EditorStyles.toolbarButton))
                {
                    selectedGroup.items[index].recording = false;
                    Recorder.Stop(false);                    
                }
            }
            else
            {
                GUI.enabled = !EditorApplication.isPlaying;
                if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Record", EditorStyles.toolbarButton))
                {
                    data.ClearRecordFlag();

                    selectedGroup.items[index].record = true;

                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssets();
                }
                GUI.enabled = true;
            }

            tw += w; w = 25;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "F", EditorStyles.toolbarButton))
            {
                EditorUtility.OpenWithDefaultApp(Path.GetDirectoryName(selectedGroup.items[index].GetPath()));
            }
            tw += w; w = 25;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "O", EditorStyles.toolbarButton))
            {
                string f = selectedGroup.items[index].GetPath() + ".mp4";
                if (File.Exists(f)) EditorUtility.OpenWithDefaultApp(f);
                else Debug.LogError("Movie does not exists : " + f);
            }
            tw += w; w = 25;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "D", EditorStyles.toolbarButton))
            {
                selectedGroup.items.Insert(index, (RecordItem)SerializationUtility.CreateCopy(rlItems.list[index]));
                EditorUtility.SetDirty(data);
            }


            tw += w; w = (rect.width - 150) / 3f;
            selectedGroup.items[index].name = EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].name);
            tw += w; w = (rect.width - 150) / 3f;
            selectedGroup.items[index].scene = (SceneAsset)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].scene, typeof(SceneAsset), false);
            
            switch (selectedGroup.recordingOptions)
            {
                case RecordingOptions.Movie:
                    tw += w; w = (rect.width - 75) / 3f;
                    selectedGroup.items[index].movieRecorderSettings = (MovieRecorderSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].movieRecorderSettings, typeof(MovieRecorderSettings), false);
                    break;
                case RecordingOptions.Image:
                    tw += w; w = (rect.width - 75) / 3f - 65f;
                    selectedGroup.items[index].imageRecorderSettings = (ImageRecorderSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].imageRecorderSettings, typeof(ImageRecorderSettings), false);
                    tw += w; w = 50;
                    if(GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].mov ? "MOV" : "IMAGE", EditorStyles.toolbarButton))
                    {
                        selectedGroup.items[index].mov = !selectedGroup.items[index].mov;
                    }
                    break;
                case RecordingOptions.Audio:
                    tw += w; w = (rect.width - 75) / 3f;
                    selectedGroup.items[index].audioRecorderSettings = (RecorderAudioSettings)EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), selectedGroup.items[index].audioRecorderSettings, typeof(RecorderAudioSettings), false);
                    break;
            }
        };

        rlItems.onSelectCallback = (l) =>
        {
            if (l.index >= 0 && l.index < selectedGroup.items.Count)
            {
                selectedItem = selectedGroup.items[l.index];
                
            }
            else
            {                
                selectedItem = null;                
            }

        };
    }
}
