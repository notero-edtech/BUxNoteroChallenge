using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSInstrumentsSettings))]
[CanEditMultipleObjects()]
public class NSInstrumentsSettingsInspector : OdinEditor
{
    private NSInstrumentsSettings o;
    public void OnEnable() { o = target as NSInstrumentsSettings; }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NSSettingsInspector.DrawTabs(NSSettingsInspector.Tab.Instruments);
        DrawDefaultInspector();
        NSSettingsInspector.DrawLicense();
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
}