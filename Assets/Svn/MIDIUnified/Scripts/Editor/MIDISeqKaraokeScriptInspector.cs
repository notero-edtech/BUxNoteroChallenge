using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MidiSeqKaraokeScript))]
public class MidiSeqKaraokeScriptInspector : Editor
{
    MidiSeqKaraokeScript o;
             
    public void OnEnable()
    {
        o = target as MidiSeqKaraokeScript;
        EditorApplication.update += this.Repaint;
    }

    public static void DrawMidiSeqKaraokeInfo(MidiSeqKaraoke o)
    {
        if (o == null)
        {
            GUILayout.Label("");
            return;
        }
        GUILayout.Label($"Id : {o.Id} | Name : {o.Name}");
        GUILayout.Label($"State : {o.State} | Synced to : {o.synchronizationContext}");
        GUILayout.Label($"Bar : {o.bar} | Beat : {o.beat}");
        
        GUILayout.Label($"Midi Time : {o.timeString} / {o.ticks:0.00} / {o.durationString} / Finished : {o.midiFinished}");
        GUILayout.Label($"Music Time : {TimeSpan.FromSeconds(o.MusicInterface.GetTime()).ToString(@"mm\:ss\.fff")} / Finished : {o.musicFinished}");
        GUILayout.Label($"Vocals Time : {TimeSpan.FromSeconds(o.VocalsInterface.GetTime()).ToString(@"mm\:ss\.fff")} / Finished : {o.vocalsFinished}");
        
        GUILayout.Label($"Music Volume : {o.musicVolume} | Vocals Volume : {o.vocalsVolume}");
        GUILayout.Label($"Music : {o.music} | Vocals : {o.vocals}");
    }
    
    public override void OnInspectorGUI()
    {
        GUI.enabled = Application.isPlaying;
        EditorGUILayout.BeginHorizontal();
        switch (o.State)
        {
            case MidiSeqStates.Playing:
            case MidiSeqStates.PickUpBar:
                if (GUILayout.Button("Pause")) { o.Pause(); }
                break;
            case MidiSeqStates.None:
            case MidiSeqStates.Finished:
                if (GUILayout.Button("Play")) { o.Play(o.pickUpBar); }
                break;
            case MidiSeqStates.Pausing:
                if (GUILayout.Button("Continue")) { o.Continue(); }
                break;
        }
        if (GUILayout.Button("Stop")) { o.Stop(); }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;

        DrawMidiSeqKaraokeInfo(o.Midi);
        
        serializedObject.UpdateIfRequiredOrScript();

        DrawDefaultInspector();
        
        EditorGUI.BeginChangeCheck();
          
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}