using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSLicenseSettings))]
[CanEditMultipleObjects()]
public class NSLicenseSettingsInspector : OdinEditor
{
    private NSLicenseSettings o;
    public void OnEnable() { o = target as NSLicenseSettings; }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NSSettingsInspector.DrawTabs(NSSettingsInspector.Tab.License);
        DrawDefaultInspector();
        NSSettingsInspector.DrawLicense();
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
}