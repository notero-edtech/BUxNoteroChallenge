using System.Collections.Generic;
using ForieroEditor;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SMFXBank))]
public class SMFXBankEditor : Editor
{
    ReorderableList fxsList;

    protected SerializedProperty m_Script;
    protected SerializedProperty fxsItems;

    protected virtual void OnEnable()
    {
        // soundSettings = target as SoundSettings;

        m_Script = serializedObject.FindProperty("m_Script");

        fxsItems = serializedObject.FindProperty("items");

        fxsList = new ReorderableList(serializedObject, fxsItems, true, true, true, true);

        fxsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Clips (Id, AudioClip, Volume)");
        };

        fxsList.onAddCallback = (l) =>
        {
            var index = fxsItems.arraySize - 1;
            fxsItems.InsertArrayElementAtIndex(Mathf.Clamp(index, 0, int.MaxValue));
            var item = fxsItems.GetArrayElementAtIndex(index + 1);
            item.FindPropertyRelative("volume").floatValue = 1f;
            item.FindPropertyRelative("id").stringValue = "";
            item.FindPropertyRelative("clip").objectReferenceValue = null;
            l.index = index + 1;
        };

        fxsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = fxsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float tw = 0f;

            float width = (rect.width - 130f - 30f - 30f) / 2f;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("id"), GUIContent.none);
            tw += width;

            width = (rect.width - 130f - 30f - 30f) / 2f;
            GUI.backgroundColor = element.FindPropertyRelative("clip").objectReferenceValue == null ? Color.red : GUI.backgroundColor;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"), GUIContent.none);
            tw += width;
            GUI.backgroundColor = backgroundColor;

            width = 30f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), "P"))
            {
                (element.FindPropertyRelative("clip").objectReferenceValue as AudioClip).PlayClip();
            }
            tw += 30f;

            width = 30f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), "S"))
            {
                ForieroEditorExtensions.StopAllClips();
            }
            tw += 30f;

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

        if(GUILayout.Button("Select", GUILayout.ExpandWidth(true))){
            List<Object> clips = new List<Object>();
            for (int i = 0; i < fxsItems.arraySize; i++){
                var item = fxsItems.GetArrayElementAtIndex(i);
                var clip = item.FindPropertyRelative("clip").objectReferenceValue;
                if (clip) clips.Add(clip);
            }
            Selection.objects = clips.ToArray();
        }

        backgroundColor = GUI.backgroundColor;
        EditorGUI.BeginChangeCheck();
        {
            draggedClips.DropObjects();

            foreach (AudioClip c in draggedClips)
            {
                var index = fxsItems.arraySize - 1;
                fxsItems.InsertArrayElementAtIndex(Mathf.Clamp(index, 0, int.MaxValue));
                var item = fxsItems.GetArrayElementAtIndex(index + 1);
                item.FindPropertyRelative("clip").objectReferenceValue = c;
                item.FindPropertyRelative("volume").floatValue = 1f;
                item.FindPropertyRelative("id").stringValue = "";
                fxsList.index = index + 1;
                isDirty = true;
            }

            draggedClips.Clear();

            fxsList?.DoLayoutList();
        }

        if (EditorGUI.EndChangeCheck() || isDirty)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
