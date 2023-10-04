/* Copyright © Marek Ledvina, Foriero s.r.o. */
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MIDISettings))]
[CanEditMultipleObjects()]
public class MIDISettingsInspector : Editor
{
    MIDISettings o;

    private string[] channelMaskValues = new string[16] {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16"
    };

    public void OnEnable()
    {
        o = target as MIDISettings;

    }

    public enum Tab
    {
        Midi,
        Mixer,
        Synth,
        SoundFont,
        Sound,        
        Percussion
    }

    public static void DrawTabs(Tab tab)
    {
        GUILayout.BeginHorizontal();

        var c = GUI.backgroundColor;

        if(tab == Tab.Midi) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Midi", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(MIDISettings.instance);
            Selection.objects = new Object[1] { MIDISettings.instance };
            Selection.activeObject = MIDISettings.instance;
        }
        GUI.backgroundColor = c;

        if (tab == Tab.Mixer) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Mixer", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(MIDIMixerSettings.instance);
            Selection.objects = new Object[1] { MIDIMixerSettings.instance };
            Selection.activeObject = MIDIMixerSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Synth) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Synth", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(MIDISynthSettings.instance);
            Selection.objects = new Object[1] { MIDISynthSettings.instance };
            Selection.activeObject = MIDISynthSettings.instance;
        }
        GUI.backgroundColor = c;

        if (tab == Tab.SoundFont) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SoundFont", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(MIDISoundFontSettings.instance);
            Selection.objects = new Object[1] { MIDISoundFontSettings.instance };
            Selection.activeObject = MIDISoundFontSettings.instance;
        }
        GUI.backgroundColor = c;

        if (tab == Tab.Percussion) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Precussion", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(MIDIPercussionSettings.instance);
            Selection.objects = new Object[1] { MIDIPercussionSettings.instance };
            Selection.activeObject = MIDIPercussionSettings.instance;
        }
        GUI.backgroundColor = c;

        if (tab == Tab.Sound) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Sound", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(MIDISoundSettings.instance);
            Selection.objects = new Object[1] { MIDISoundSettings.instance };
            Selection.activeObject = MIDISoundSettings.instance;
        }
        GUI.backgroundColor = c;
              
        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTabs(Tab.Midi);

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Channel Masks", EditorStyles.boldLabel);

        o.synthChannelMask = EditorGUILayout.MaskField("Synthesizer Mask", o.synthChannelMask, channelMaskValues);
        o.channelMask = EditorGUILayout.MaskField("MidiOut Mask", o.channelMask, channelMaskValues);

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }
}