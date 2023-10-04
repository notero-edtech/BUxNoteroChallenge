using System.Collections.Generic;
using System.Reflection;
using ForieroEditor;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(SoundSettings))]
public class SoundSettingsEditor : Editor
{
    ReorderableList fxsList;
    ReorderableList musicList;
    ReorderableList selectedList;

    protected SerializedProperty m_Script;

    protected SerializedProperty log;
    protected SerializedProperty logUI;

    protected SerializedProperty createSoundFXPlaceholders;
    protected SerializedProperty soundFXPlaceholdersDirectory;

    protected SerializedProperty audioMixer;

    protected SerializedProperty tab;
    protected SerializedProperty fxs;
    protected SerializedProperty music;

    int FindMixerGroupIndex(AudioMixerGroup audioMixerGroup)
    {
        for (int i = 0; i < mixerGroups.Length; i++)
        {
            if (audioMixerGroup == mixerGroups[i])
            {
                return i;
            }
        }
        return 0;
    }

    protected virtual void OnEnable()
    {
        // soundSettings = target as SoundSettings;

        m_Script = serializedObject.FindProperty("m_Script");

        log = serializedObject.FindProperty("log");
        logUI = serializedObject.FindProperty("logUI");

        createSoundFXPlaceholders = serializedObject.FindProperty("createSoundFXPlaceholders");
        soundFXPlaceholdersDirectory = serializedObject.FindProperty("soundFXPlaceholdersDirectory");

        audioMixer = serializedObject.FindProperty("audioMixer");

        tab = serializedObject.FindProperty("tab");
        fxs = serializedObject.FindProperty("fxGroups");
        music = serializedObject.FindProperty("musicGroups");

        fxsList = new ReorderableList(serializedObject, fxs, true, true, true, true);
        musicList = new ReorderableList(serializedObject, music, true, true, true, true);

        fxsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "FX Groups (Id, Bank, AudioMixerGroup)");
        };

        fxsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = fxsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float width = 0f;
            float tw = 0f;

            width = (rect.width - 100) / 2f;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("id"), GUIContent.none);
            tw += width;

            width = (rect.width - 100) / 2f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bank"), GUIContent.none);
            tw += width;

            int mixerGroupIndex = FindMixerGroupIndex(element.FindPropertyRelative("audioMixerGroup").objectReferenceValue as AudioMixerGroup);
            int indexTmp = mixerGroupIndex;

            width = 100f;
            mixerGroupIndex = EditorGUI.Popup(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), mixerGroupIndex, mixerGroupsStrings);
            tw += width;

            if (indexTmp != mixerGroupIndex)
            {
                element.FindPropertyRelative("audioMixerGroup").objectReferenceValue = mixerGroups[mixerGroupIndex];
            }
        };

        fxsList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
                selectedList = null;
                bankSO = null;
            }
        };

        fxsList.onChangedCallback = (ReorderableList l) =>
        {

        };

        fxsList.onReorderCallback = (ReorderableList l) =>
        {

        };

        fxsList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("id").stringValue = "";
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("items").ClearArray();
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("audioMixerGroup").objectReferenceValue = null;
        };

        fxsList.onSelectCallback = (ReorderableList l) =>
        {
            SetFXsClips();
        };

        musicList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Music Groups (Id, AudioClip, Shufle, Crosfade, AudioMixerGroup, Volume)");
        };

        musicList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = musicList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float tw = 0;

            float width = Mathf.Clamp(rect.width - 280f, 50f, 1000f) / 2f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("id"), GUIContent.none);
            tw += width;

            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bank"), GUIContent.none);
            tw += width;
                       
            width = 20f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shufle"), GUIContent.none);
            tw += width;

            width = 30f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("crossfade"), GUIContent.none);
            tw += width;

            int mixerGroupIndex = FindMixerGroupIndex(element.FindPropertyRelative("audioMixerGroup").objectReferenceValue as AudioMixerGroup);
            int indexTmp = mixerGroupIndex;

            width = 100f;
            mixerGroupIndex = EditorGUI.Popup(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), mixerGroupIndex, mixerGroupsStrings);
            tw += width;

            width = 130f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("volume"), GUIContent.none);
            tw += width;

            if (indexTmp != mixerGroupIndex)
            {
                element.FindPropertyRelative("audioMixerGroup").objectReferenceValue = mixerGroups[mixerGroupIndex];
            }
        };

        musicList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
                selectedList = null;
                bankSO = null;
            }
        };

        musicList.onChangedCallback = (ReorderableList l) =>
        {

        };

        musicList.onReorderCallback = (ReorderableList l) =>
        {
            SetMusicClips();
        };

        musicList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("id").stringValue = "";
           // l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("loop").boolValue = true;
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("shufle").boolValue = true;
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("volume").floatValue = 1f;
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("crossfade").floatValue = 1f;
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("items").ClearArray();
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("audioMixerGroup").objectReferenceValue = null;
        };

        musicList.onSelectCallback = (ReorderableList l) =>
        {
            SetMusicClips();
        };
    }

    SerializedObject bankSO = null;

    void SetFXsClips()
    {
        if (fxsList.index >= 0 && fxsList.index < fxsList.count)
        {
            switch ((SM.Tab)fxsList.serializedProperty.GetArrayElementAtIndex(fxsList.index).FindPropertyRelative("tab").enumValueIndex)
            {
                case SM.Tab.Self:
                    FillSelectedList(fxsList.serializedProperty.GetArrayElementAtIndex(fxsList.index).FindPropertyRelative("items"));
                    bankSO = null;
                    break;
                case SM.Tab.Bank:
                    var bank = fxsList.serializedProperty.GetArrayElementAtIndex(fxsList.index).FindPropertyRelative("bank").objectReferenceValue;
                    if (bank)
                    {
                        bankSO = new SerializedObject(bank);
                        FillSelectedList(bankSO.FindProperty("items"));
                    }
                    else
                    {
                        bankSO = null;
                        selectedList = null;
                    }
                    break;
            }

        }
        else
        {
            selectedList = null;
            bankSO = null;
        }
    }

    void SetMusicClips()
    {
        if (musicList.index >= 0 && musicList.index < musicList.count)
        {
            switch ((SM.Tab)musicList.serializedProperty.GetArrayElementAtIndex(musicList.index).FindPropertyRelative("tab").enumValueIndex)
            {
                case SM.Tab.Self:
                    FillSelectedList(musicList.serializedProperty.GetArrayElementAtIndex(musicList.index).FindPropertyRelative("items"));
                    bankSO = null;
                    break;
                case SM.Tab.Bank:
                    var bank = musicList.serializedProperty.GetArrayElementAtIndex(musicList.index).FindPropertyRelative("bank").objectReferenceValue;
                    if (bank)
                    {
                        bankSO = new SerializedObject(bank);
                        FillSelectedList(bankSO.FindProperty("items"));
                    }
                    else
                    {
                        bankSO = null;
                        selectedList = null;
                    }
                    break;
            }
        }
        else
        {
            selectedList = null;
            bankSO = null;
        }
    }

    void FillSelectedList(SerializedProperty p)
    {
        selectedList = new ReorderableList(serializedObject, p, true, true, true, true);

        selectedList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Clips (Id, AudioClip, Loop, Play, Stop, Volume)");
        };

        selectedList.onAddCallback = (l) =>
        {
            var index = selectedList.serializedProperty.arraySize - 1;
            selectedList.serializedProperty.InsertArrayElementAtIndex(Mathf.Clamp(index, 0, int.MaxValue));
            var item = selectedList.serializedProperty.GetArrayElementAtIndex(index + 1);
            item.FindPropertyRelative("volume").floatValue = 1f;
            item.FindPropertyRelative("id").stringValue = "";
            item.FindPropertyRelative("clip").objectReferenceValue = null;
            l.index = index + 1;
        };

        selectedList.onReorderCallback = (ReorderableList l) =>
        {
            
        };

        selectedList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = selectedList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float tw = 0f;

            float width = (rect.width - 130f - 30f - 30f - 20f) / 2f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("id"), GUIContent.none);
            tw += width;

            width = (rect.width - 130f - 30f - 30f - 20f) / 2f;
            GUI.backgroundColor = element.FindPropertyRelative("clip").objectReferenceValue == null ? Color.red : GUI.backgroundColor;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"), GUIContent.none);
            tw += width;
            GUI.backgroundColor = backgroundColor;

            width = 20f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loop"), GUIContent.none);
            tw += width;

            width = 30f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), "P"))
            {
                (element.FindPropertyRelative("clip").objectReferenceValue as AudioClip).PlayClip();
            }
            tw += width;

            width = 30f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), "S"))
            {
                ForieroEditorExtensions.StopAllClips();
            }
            tw += width;

            width = 130f;
            PropertyFieldSlider(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("volume"), 0f, 1f);
            tw += width;
        };
    }

    void PropertyFieldSlider(Rect position, SerializedProperty property, float min, float max)
    {
        //EditorGUI.BeginProperty();

        EditorGUI.BeginChangeCheck();

        var newValue = EditorGUI.Slider(position, property.floatValue, min, max);

        if (EditorGUI.EndChangeCheck())
        {
            property.floatValue = newValue;
        }

        //EditorGUI.EndProperty();
    }
       
    AudioMixerGroup[] mixerGroups = null;
    string[] mixerGroupsStrings = new string[0];

    Color backgroundColor;

    List<AudioClip> draggedClips = new List<AudioClip>();

    bool isDirty = false;

    bool logsFoldout;
    bool placeholdersFoldout;

    public override void OnInspectorGUI()
    {
        isDirty = false;
        backgroundColor = GUI.backgroundColor;
        EditorGUI.BeginChangeCheck();
        {
            //EditorGUILayout.PropertyField (m_Script);
                        
            logsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(logsFoldout, "Log");
            if(logsFoldout)
            {
                EditorGUILayout.PropertyField(log);
                EditorGUILayout.PropertyField(logUI);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            placeholdersFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(placeholdersFoldout, "Placeholders ( Editor Only )");
            if(placeholdersFoldout)
            {
                EditorGUILayout.PropertyField(createSoundFXPlaceholders);
                EditorGUILayout.PropertyField(soundFXPlaceholdersDirectory);                           
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));                   
            EditorGUILayout.PropertyField(audioMixer);
            GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));

            if (audioMixer.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Select 'AudioMixer'", MessageType.Warning);
            }
            else
            {
                mixerGroups = (audioMixer.objectReferenceValue as AudioMixer).FindMatchingGroups(string.Empty);

                mixerGroupsStrings = new string[mixerGroups.Length];
                for (int i = 0; i < mixerGroups.Length; i++)
                {
                    mixerGroupsStrings[i] = mixerGroups[i].name;
                }

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = (tab.enumValueIndex == (int)SoundSettings.Tab.FXs ? Color.green : backgroundColor);
                if (GUILayout.Button("FXs"))
                {
                    tab.enumValueIndex = (int)SoundSettings.Tab.FXs;
                    SetFXsClips();
                }
                GUI.backgroundColor = (tab.enumValueIndex == (int)SoundSettings.Tab.Music ? Color.green : backgroundColor);
                if (GUILayout.Button("Music"))
                {
                    tab.enumValueIndex = (int)SoundSettings.Tab.Music;
                    SetMusicClips();
                }
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = backgroundColor;

                switch ((SoundSettings.Tab)tab.enumValueIndex)
                {
                    case SoundSettings.Tab.FXs:
                        fxsList.DoLayoutList();
                        DrawFXsTabs();
                        break;
                    case SoundSettings.Tab.Music:
                        musicList.DoLayoutList();
                        DrawMusicTabs();
                        break;
                }
                               
                if (selectedList != null)
                {
                    draggedClips.DropObjects();

                    foreach (AudioClip c in draggedClips)
                    {
                        var index = selectedList.serializedProperty.arraySize - 1;
                        selectedList.serializedProperty.InsertArrayElementAtIndex(Mathf.Clamp(index, 0, int.MaxValue));
                        var item = selectedList.serializedProperty.GetArrayElementAtIndex(index + 1);
                        item.FindPropertyRelative("clip").objectReferenceValue = c;
                        item.FindPropertyRelative("volume").floatValue = 1f;
                        item.FindPropertyRelative("id").stringValue = "";
                        selectedList.index = index + 1;
                        isDirty = true;
                    }

                    draggedClips.Clear();

                    selectedList.DoLayoutList();
                }
            }
        }

        if (EditorGUI.EndChangeCheck() || isDirty)
        {
            serializedObject.ApplyModifiedProperties();
            if (bankSO != null)
            {
                bankSO.ApplyModifiedProperties();
            }
        }
    }

    void DrawFXsTabs()
    {
        if (fxsList.index >= 0 && fxsList.index < fxsList.count)
        {
            var tab = fxsList.serializedProperty.GetArrayElementAtIndex(fxsList.index).FindPropertyRelative("tab");

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = (tab.enumValueIndex == (int)SM.Tab.Self ? Color.green : backgroundColor);
            if (GUILayout.Button("Self"))
            {
                tab.enumValueIndex = (int)SM.Tab.Self;
                SetFXsClips();
            }
            GUI.backgroundColor = (tab.enumValueIndex == (int)SM.Tab.Bank ? Color.green : backgroundColor);
            if (GUILayout.Button("Bank"))
            {
                tab.enumValueIndex = (int)SM.Tab.Bank;
                SetFXsClips();
            }
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.EndHorizontal();
        }
    }

    void DrawMusicTabs()
    {
        if (musicList.index >= 0 && musicList.index < musicList.count)
        {
            var tab = musicList.serializedProperty.GetArrayElementAtIndex(musicList.index).FindPropertyRelative("tab");

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = (tab.enumValueIndex == (int)SM.Tab.Self ? Color.green : backgroundColor);
            if (GUILayout.Button("Self"))
            {
                tab.enumValueIndex = (int)SM.Tab.Self;
                SetMusicClips();
            }
            GUI.backgroundColor = (tab.enumValueIndex == (int)SM.Tab.Bank ? Color.green : backgroundColor);
            if (GUILayout.Button("Bank"))
            {
                tab.enumValueIndex = (int)SM.Tab.Bank;
                SetMusicClips();
            }
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.EndHorizontal();
        }
    }
}
