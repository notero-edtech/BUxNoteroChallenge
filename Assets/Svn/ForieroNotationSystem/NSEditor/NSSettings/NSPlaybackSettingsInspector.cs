using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSPlaybackSettings))]
[CanEditMultipleObjects()]
public class NSPlaybackSettingsInspector : OdinEditor
{
    private NSPlaybackSettings o;
    public void OnEnable() { o = target as NSPlaybackSettings; }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NSSettingsInspector.DrawTabs(NSSettingsInspector.Tab.Playback);
        DrawDefaultInspector();
        NSSettingsInspector.DrawLicense();
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
}