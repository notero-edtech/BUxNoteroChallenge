using System;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSDisplaySettings))]
[CanEditMultipleObjects()]
public class NSDisplaySettingsInspector : OdinEditor
{
    private NSDisplaySettings o;

    public void OnEnable()
    {
        o = target as NSDisplaySettings;
        articulations = serializedObject.FindProperty("articulations");
        ornaments = serializedObject.FindProperty("ornaments");
        dynamics = serializedObject.FindProperty("dynamics");
        chords = serializedObject.FindProperty("chords");
    }

    private enum Tab 
    {
        Articulations,
        Ornaments,
        Dynamics,
        Chords
    }

    private static Tab _tab = Tab.Articulations;

    private SerializedProperty articulations;
    private SerializedProperty ornaments;
    private SerializedProperty dynamics;
    private SerializedProperty chords;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NSSettingsInspector.DrawTabs(NSSettingsInspector.Tab.Display);
        DrawDefaultInspector();

        // EditorGUILayout.BeginHorizontal();
        // var c = GUI.backgroundColor;
        // foreach (var v in System.Enum.GetValues(typeof(Tab)))
        // {
        //     GUI.backgroundColor = _tab == (Tab)v ? Color.green : c;
        //     if (GUILayout.Button(v.ToString(), EditorStyles.toolbarButton))
        //     {
        //         _tab = (Tab)v;
        //     }
        //     GUI.backgroundColor = c;
        // }
        // EditorGUILayout.EndHorizontal();
        //
        // switch (_tab)
        // {
        //     case Tab.Articulations: EditorGUILayout.PropertyField(articulations); break;
        //     case Tab.Ornaments: EditorGUILayout.PropertyField(ornaments); break;
        //     case Tab.Dynamics: EditorGUILayout.PropertyField(dynamics); break;
        //     case Tab.Chords: EditorGUILayout.PropertyField(chords); break;
        // }
        
        NSSettingsInspector.DrawLicense();
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
}