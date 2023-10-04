using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public partial class LangSettingsEditor : Editor
{
    void InitDictionariesList()
    {
        dictionariesList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Dictionaries (Alias Name, TextAsset, Stored)");
        };

        dictionariesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = dictionariesList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float width = (rect.width - 80f) / 2f;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("aliasName"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + width, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("textAsset"), GUIContent.none);
            EditorGUI.EndDisabledGroup();
            EditorGUI.PropertyField(new Rect(rect.x + 2f * width, rect.y, 80f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("stored"), GUIContent.none);           
        };

        dictionariesList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
                
            }
        };

        dictionariesList.onChangedCallback = (ReorderableList l) =>
        {

        };

        dictionariesList.onReorderCallback = (ReorderableList l) =>
        {

        };

        dictionariesList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
            //l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("name").stringValue = "";
            //l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("langActorVoices").ClearArray();
        };

        dictionariesList.onSelectCallback = (ReorderableList l) =>
        {
            
        };
    }

    void DrawDictionaryItem()
    {

    }
}
