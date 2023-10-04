using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MIDIMixerSettings))]
[CanEditMultipleObjects()]
public class MIDIMixerSettingsInspector : Editor
{
    MIDIMixerSettings o;

    public void OnEnable()
    {
        o = target as MIDIMixerSettings;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MIDISettingsInspector.DrawTabs(MIDISettingsInspector.Tab.Mixer);

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }
}