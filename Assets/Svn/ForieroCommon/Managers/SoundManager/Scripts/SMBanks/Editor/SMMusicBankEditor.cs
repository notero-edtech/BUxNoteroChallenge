using System.Collections.Generic;
using System.Reflection;
using ForieroEditor;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(SMMusicBank))]
public class SMMusicBankEditor : Editor
{
    ReorderableList musicList;

    protected SerializedProperty m_Script;
    protected SerializedProperty musicItems;

    protected virtual void OnEnable()
    {
        // soundSettings = target as SoundSettings;

        m_Script = serializedObject.FindProperty("m_Script");

        musicItems = serializedObject.FindProperty("items");

        musicList = new ReorderableList(serializedObject, musicItems, true, true, true, true);

        musicList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Clips (Id, AudioClip, Loop, Play, Stop, Volume)");
        };

        musicList.onAddCallback = (l) =>
        {
            var index = musicItems.arraySize - 1;
            musicItems.InsertArrayElementAtIndex(Mathf.Clamp(index, 0, int.MaxValue));
            var item = musicItems.GetArrayElementAtIndex(index + 1);
            item.FindPropertyRelative("volume").floatValue = 1f;
            item.FindPropertyRelative("id").stringValue = "";
            item.FindPropertyRelative("clip").objectReferenceValue = null;
            l.index = index + 1;
        };

        musicList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = musicList.serializedProperty.GetArrayElementAtIndex(index);
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
       
    //AudioMixerGroup[] mixerGroups = null;
    //string[] mixerGroupsStrings = new string[0];

    Color backgroundColor;

    List<AudioClip> draggedClips = new List<AudioClip>();

    bool isDirty = false;

    public override void OnInspectorGUI()
    {
        isDirty = false;
        backgroundColor = GUI.backgroundColor;
        EditorGUI.BeginChangeCheck();
        {
            draggedClips.DropObjects();

            foreach(AudioClip c in draggedClips){
                var index = musicItems.arraySize - 1;
                musicItems.InsertArrayElementAtIndex(Mathf.Clamp(index, 0, int.MaxValue));
                var item = musicItems.GetArrayElementAtIndex(index + 1);
                item.FindPropertyRelative("clip").objectReferenceValue = c;
                item.FindPropertyRelative("volume").floatValue = 1f;
                item.FindPropertyRelative("id").stringValue = "";
                musicList.index = index + 1;
                isDirty = true;
            }

            draggedClips.Clear();

            musicList?.DoLayoutList();
           
        }

        if (EditorGUI.EndChangeCheck() || isDirty)
        {
            serializedObject.ApplyModifiedProperties();
        
        }
    }
}
