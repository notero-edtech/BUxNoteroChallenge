using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSSessionSettings))]
[CanEditMultipleObjects()]
public class NSSessionSettingsInspector : OdinEditor
{
    private NSSessionSettings o;
    public void OnEnable() { o = target as NSSessionSettings; }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NSSettingsInspector.DrawTabs(NSSettingsInspector.Tab.Session);
        DrawDefaultInspector();
        NSSettingsInspector.DrawLicense();
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
}