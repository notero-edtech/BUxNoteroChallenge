using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSSystemsSettings))]
[CanEditMultipleObjects()]
public class NSSystemsSettingsInspector : OdinEditor
{
    private NSSystemsSettings o;
    public void OnEnable() { o = target as NSSystemsSettings; }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NSSettingsInspector.DrawTabs(NSSettingsInspector.Tab.Systems);
        DrawDefaultInspector();
        NSSettingsInspector.DrawLicense();
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
}