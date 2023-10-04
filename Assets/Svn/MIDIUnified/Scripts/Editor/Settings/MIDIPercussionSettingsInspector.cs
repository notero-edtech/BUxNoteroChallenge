using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MIDIPercussionSettings))]
[CanEditMultipleObjects()]
public class MIDIPercussionSettingsInspector : Editor
{
    MIDIPercussionSettings o;

    public void OnEnable()
    {
        o = target as MIDIPercussionSettings;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MIDISettingsInspector.DrawTabs(MIDISettingsInspector.Tab.Percussion);

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }
}