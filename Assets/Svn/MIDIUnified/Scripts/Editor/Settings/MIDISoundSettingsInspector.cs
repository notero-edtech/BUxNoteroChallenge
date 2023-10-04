using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MIDISoundSettings))]
[CanEditMultipleObjects()]
public class MIDISoundSettingsInspector : Editor
{
    MIDISoundSettings o;

    public void OnEnable()
    {
        o = target as MIDISoundSettings;
    }

    public override void OnInspectorGUI()
    {        
        serializedObject.Update();

        MIDISettingsInspector.DrawTabs(MIDISettingsInspector.Tab.Sound);

        EditorGUILayout.HelpBox("In case you don't need BASS24 for iOS use 'BASS24_DISABLED' in iOS projectsettings defines. That allows you to use BITCODE.", MessageType.Warning);

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }
}