using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(MidiKeyboardInputBinding))]
[CanEditMultipleObjects()]
public class MidiKeyboardInputBindingInspector : Editor
{
    MidiKeyboardInputBinding o;

    ReorderableList list;

    SerializedProperty keyBindings = null;

    public void OnEnable()
    {
        o = target as MidiKeyboardInputBinding;

        keyBindings = serializedObject.FindProperty("keyBindings").FindPropertyRelative("keyBindings");

        list = new ReorderableList(serializedObject, keyBindings, true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "KeyCode, ToneEnum, OctaveShift");
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var item = keyBindings.GetArrayElementAtIndex(index);
            var w = rect.width / 3f;
            var h = rect.height;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, w, h), item.FindPropertyRelative("keyCode"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + w, rect.y, w, h), item.FindPropertyRelative("toneEnum"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + w + w, rect.y, w, h), item.FindPropertyRelative("octaveShift"), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginChangeCheck();

        list.DoLayoutList();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}