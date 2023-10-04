using System;
using ForieroEngine.Extensions;
using MoreLinq;
using UnityEngine;
#if FMOD
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using EventReference = FMODUnity.EventReference;
using FMOD.Studio;
#endif

public partial class FMODTimelineMidiSeq : MonoBehaviour, IMidiSeqControl,  MidiSeqKaraoke.IMidiSeqKaraokeCallbacks, MidiSeqKaraoke.IMidiSeqKaraokeEvents
{
#if FMOD
    public EventReference timelineEventId;
#endif
    public TextAsset timelineJson;
    [Range(-0.2f, 0.2f)] public double timeOffset = 0.05;
    
    public double Time => _timeMS / 1000.0;
    public string TimeString => TimeSpan.FromMilliseconds(_timeMS).ToString(@"mm\:ss\.fff");
    private int _timeMS = 0;

    [Serializable]
    public class MidiTimelineItem
    {
        public string marker;
        public double startTime;
        public string StartTimeString => TimeSpan.FromSeconds(startTime).ToString(@"mm\:ss\.fff");
        public string triggerParameter;
        public TextAsset midiFile;
        public FMODMidiSeq midiSeq;
        [Range(-0.2f, 0.2f)] public double timeOffset = 0;
    }

    [NonSerialized] public FMODTimelineMarkers markers;
    public MidiTimelineItem[] items;
    
    #if FMOD
    public EventInstance EventInstance => _ei;
    public FMODTimelineEvents TimelineEvents;
    private EventInstance _ei;
    #endif
    
    void Awake()
    {
        #if FMOD
        _ei = FMODUnity.RuntimeManager.CreateInstance(timelineEventId);
        TimelineEvents = new FMODTimelineEvents(_ei);
        #endif
        InitMidiSequencers();
    }

    void Update()
    {
        #if FMOD
        TimelineEvents.InvokeEvents();
        if (_ei.isValid()) _ei.getTimelinePosition(out _timeMS);
        #endif
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].midiSeq == null) continue;
            items[i].midiSeq.ManualUpdate(_timeMS / 1000.0 + (State == MidiSeqStates.Playing ? (timeOffset + items[i].timeOffset) : 0));
        }
    }

    private void OnDestroy()
    {
        #if FMOD
        TimelineEvents?.Dispose();
        TimelineEvents = null;
        if (_ei.isValid())
        {
            _ei.stop(STOP_MODE.ALLOWFADEOUT);
            _ei.release();
        }
        #endif
    }
  
    void InitMidiSequencers()
    {
        if(timelineJson) markers = FMODTimelineMarkers.FromJSON(timelineJson.text);
        items.ForEach((i) =>
        {
            if (!i.midiFile) return;
            if (!i.midiSeq) i.midiSeq = gameObject.AddComponent<FMODMidiSeq>();
            i.midiSeq.Initialize(i.midiFile.bytes, i.startTime, $"Marker : {i.marker} | Midi : {i.midiFile.name}");
        });
    }

    public MidiSeqStates State { get; private set; }

    public void Play()
    {
        State = MidiSeqStates.Playing;
        #if FMOD
        if(timeOffset > 0) this.FireAction((float)timeOffset, () => { if(_ei.isValid()) _ei.start(); });
        else if(_ei.isValid()) _ei.start();
        #endif
    }

    public void Continue()
    {
        State = MidiSeqStates.Playing;
        #if FMOD
        if(timeOffset > 0) this.FireAction((float)timeOffset, () => { if (_ei.isValid()) _ei.setPaused(false); });
        else if (_ei.isValid()) _ei.setPaused(false);
        #endif
    }

    public void Pause()
    {
        State = MidiSeqStates.Pausing;
        #if FMOD
        if(_ei.isValid()) _ei.setPaused(true);
        #endif
    }
   
    public void Stop()
    { 
        State = MidiSeqStates.None;
        #if FMOD
        if(_ei.isValid()) _ei.stop(STOP_MODE.ALLOWFADEOUT);
        #endif
    }
}