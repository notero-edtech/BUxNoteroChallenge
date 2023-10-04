using System.Collections.Generic;
using ForieroEngine.Extensions;
using MoreLinq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FMODTimelineMidiSeq))]
public class FMODTimelineMidiSeqInspector : Editor
{
    private FMODTimelineMidiSeq o;
    private FMODTimelineMarkers markers;
    private List<string> mStringsList = new List<string>();
    private string[] mStringsArray = new string [0];

    private SerializedProperty items;

    private int i = 0;
    
    public void OnEnable()
    {
        o = target as FMODTimelineMidiSeq;
        if (o.timelineJson) markers = FMODTimelineMarkers.FromJSON(o.timelineJson.text);
        
        EditorApplication.update += this.Repaint;
        
        markers.Markers.ForEach(m =>
        {
            mStringsList.Add(m.ToString());
        });
        mStringsArray = mStringsList.ToArray();
    }

    void DrawMidiSeqKaraokeInfo(FMODTimelineMidiSeq.MidiTimelineItem m)
    {
        GUI.enabled = Application.isPlaying;
        if (m.midiSeq == null) return;
        GUILayout.BeginHorizontal();
        #if FMOD
        if (GUILayout.Button("Transition To")) o.EventInstance.setParameterByName(m.triggerParameter, 1);
        #endif
        GUILayout.Label($"{m.triggerParameter}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.enabled = true;
    }
 
    public override void OnInspectorGUI()
    {
        
        EditorGUILayout.BeginHorizontal();
        {
            #if FMOD
            GUILayout.Label($"State: {o.State} | Time: {o.TimeString}");
            #endif
            GUILayout.FlexibleSpace();
            GUI.enabled = Application.isPlaying;
            switch (o.State)
            {
                case MidiSeqStates.Playing:
                case MidiSeqStates.PickUpBar: if (GUILayout.Button("Pause")) o.Pause(); break;
                case MidiSeqStates.None:
                case MidiSeqStates.Finished: if (GUILayout.Button("Play")) o.Play(); break;
                case MidiSeqStates.Pausing: if (GUILayout.Button("Continue")) o.Continue(); break;
            }
            if (GUILayout.Button("Stop")) o.Stop();
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();
        #if FMOD
        GUILayout.Label($"Time Signature : {o.TimelineEvents?.TimesignatureNominator}/{o.TimelineEvents?.TimesignatureDenominator} | Tempo: {o.TimelineEvents?.Tempo} | Bar : {o.TimelineEvents?.Bar} | Beat : {o.TimelineEvents?.Beat}");
        GUILayout.Label($"Marker : {(string) o.TimelineEvents?.Marker} | Position : {o.TimelineEvents?.MarkerPositionString}");
        #endif
        
        EditorGUI.BeginChangeCheck();
        
        serializedObject.UpdateIfRequiredOrScript();
        
        DrawDefaultInspector();
        
        i = EditorGUILayout.Popup("Markers", i, mStringsArray);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add"))
        {
            var m = markers.Markers[i];
            if (m.MType == FMODTimelineMarkers.Marker.MTypeEnum.DestinationMarker)
            {
                var newM = new FMODTimelineMidiSeq.MidiTimelineItem();
                newM.marker = m.Name;
                newM.startTime = m.Position ?? 0;
                o.items = o.items.AddLast(newM);
                EditorUtility.SetDirty(o);
            }
        }
        if (GUILayout.Button("Add from Json"))
        {
            var newItems = new List<FMODTimelineMidiSeq.MidiTimelineItem>();
            
            if(markers == null) return;
            if(markers.Markers == null) return;
            
            markers.Markers.ForEach(m =>
            {
                if (m.MType == FMODTimelineMarkers.Marker.MTypeEnum.DestinationMarker)
                {
                    var newM = new FMODTimelineMidiSeq.MidiTimelineItem();
                    newM.marker = m.Name;
                    newM.startTime = m.Position ?? 0;
                    newItems.Add(newM);
                }
            });
            o.items = newItems.ToArray();
            EditorUtility.SetDirty(o);
        }
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); }

        //o?.items?.ForEach(m => { DrawMidiSeqKaraokeInfo(m); });
    }
}