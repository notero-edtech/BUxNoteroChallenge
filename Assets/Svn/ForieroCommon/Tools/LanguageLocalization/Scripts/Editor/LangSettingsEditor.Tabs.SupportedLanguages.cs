using UnityEditor;
using UnityEngine;

public partial class LangSettingsEditor : Editor
{
    void InitSupportedLanguagesList()
    {
        supportedLanguagesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = supportedLanguagesList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float width = rect.width;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("included"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 20, rect.y, width - 20, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("langCode"), GUIContent.none);
        };
    }
}
