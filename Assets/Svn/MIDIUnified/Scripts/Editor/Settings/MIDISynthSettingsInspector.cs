/* Copyright © Marek Ledvina, Foriero s.r.o. */
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MIDISynthSettings))]
[CanEditMultipleObjects()]
public class MIDISynthSettingsInspector : Editor
{
    MIDISynthSettings o;

    public void OnEnable()
    {
        o = target as MIDISynthSettings;

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MIDISettingsInspector.DrawTabs(MIDISettingsInspector.Tab.Synth);

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();           

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }
}