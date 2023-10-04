/* Copyright © Marek Ledvina, Foriero s.r.o. */

using System;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.EnumUtilities;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraoke : IMidiSender, IDisposable, MidiSeqKaraoke.IMidiSeqKaraokeEvents, IMidiSeqControl
{
    public string Id { get; }
    public string Name { get; set; }

    public bool debug = false;

    public MidiSeqKaraoke(string name = null)
    {
        this.Name = name;
    }
    public MidiSeqKaraoke(string id, string name = null)
    {
        this.Id = id;
        this.Name = name;
        this.Register();
    }
    public MidiSeqKaraoke(IAudioSource m, IAudioSource v)
    {
        pickupBarTimer.Elapsed += (sender, args) => { PickUpBar();};
        this.MusicInterface.AudioInterface = m;
        this.VocalsInterface.AudioInterface = v;
    }

    ~MidiSeqKaraoke()
    {
        pickupBarTimer.Stop();
        pickupBarTimer.Close();
        this.Unregister();
    }

    public void Dispose()
    {
        pickupBarTimer.Stop();
        pickupBarTimer.Close();
        this.Unregister();
        this.ShortMessageEvent = null;
    }

    public enum SynchronizationContext { Midi, Music, Vocal, Manual }
    public SynchronizationContext synchronizationContext = SynchronizationContext.Midi;
    public float playingDelay = 0;
    private float _playingDelay = 0;
    
    private MidiSeqStates _state = MidiSeqStates.None;
    
    public MidiSeqStates State
    {
        get => _state;
        private set
        {
            if (value != _state)
            {
                MidiSeqStates tmpState = _state;
                _state = value;
                switch (value)
                {
                    case MidiSeqStates.PickUpBar: OnPickUpBar?.Invoke(bar); break;
                    case MidiSeqStates.Finished: OnFinished?.Invoke(); break;
                    case MidiSeqStates.Playing: if (tmpState == MidiSeqStates.Pausing) OnResume?.Invoke(); else OnPlay?.Invoke(); break;
                    case MidiSeqStates.Pausing: OnPause?.Invoke(); break;
                }
            }
        }
    }

    public bool midiOut { get; set; } = true;
    public bool midiThrough { get; set; } = false;

    public bool music { get => MusicInterface.Enabled; set => MusicInterface.Enabled = value; }
    public AudioClip musicClip { get; private set; }
    public string musicClipId { get; set; }
    public float musicVolume { get; set; } = 1f;

    public bool vocals { get => VocalsInterface.Enabled; set => VocalsInterface.Enabled = value; }
    public AudioClip vocalsClip { get; private set; }
    public string vocalsClipId { get; set; }
    public float vocalsVolume { get; set; } = 1f;
  
    // 0.1 | 10 //
    public float speed { get; set; } = 1f;
    // -12 | +12 //
    public int semitone { get; private set; } = 0;
    public double ticks { get; private set; } = 0;
    public double time { get; set; } = 0;
    public string timeString => TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.fff");
    public double timelineStartTime { get; set; } = 0;
    public string timelineStartTimeString => TimeSpan.FromSeconds(timelineStartTime).ToString(@"mm\:ss\.fff");
    public double duration { get; private set; } = 0f;
    public string durationString => TimeSpan.FromSeconds(duration).ToString(@"mm\:ss\.fff");
    public float eventsOffset = 0;

    public double GetCumulativeTonesDuration()
    {
        double r = 0;
        foreach(var t in tracks)
        {
            for(int i = 0; i<t.Count;i++)
            {
                var e = t[i];
                if (e.CommandCode == MidiCommandCode.NoteOn)
                {
                    var next = 1;
                    while (i + next < t.Count && t[i + next].CommandCode != MidiCommandCode.NoteOff) next++;
                    r += t[i + next].AbsoluteTime - t[i].AbsoluteTime;
                }
            }
        }

        return TicksToTime(r);
    } 
    public double markerTimeThreshold { get; set; } = 0.2;
    public int beat { get; private set; } = 0;
    public int beatCount { get; private set; } = 0;
    public bool metronome { get; set; } = true;
    public int bar { get; private set; } = 0;
    public int barCount { get; private set; } = 0;
    public bool pickUpBar { get; set; } = true;
    public bool pickUpBarOnRepeat { get; set; } = true;
    public bool forceTrackAsChannel { get; set; } = true;
    public int timeSignatureNumerator { get; private set; } = 4;
    public int timeSignatureDenominator { get; private set; }= 4;
    public int PPQN { get; set; } = 24;
    public readonly int PPQNMinValue = 24;
    public float tempo { get; private set; } = 120f;
    public float tempoTicks => MicrosecondsPerMinute / tempo;
    public int keyMajorMinor { get; private set; } = 0;
    public int keySharpsFlats { get; private set; } = 0;

    //	float speedTmp = 1f;
    //	float tempoTmp = 120f;

    //	BPM – it’s a beats per minute 60 000 000/MPQN
    //	PPQN – Pulses per quater note, resolution found in MIDI file
    //	MPQN – microseconds per quaternote or quaternote duration, range is 0-8355711 microseconds (according to MIDI file specification)
    //	QLS – seconds per quaternote (MPQN/1000000)
    //	TDPS – seconds per tick (QLS/PPQN)
    //  TPS - ticks per second (1000000/QLS/PPQN) = (1000000/1/MPQN/1000000/PPQN)

    public List<IList<MidiEvent>> tracks { get; private set; } = new List<IList<MidiEvent>>();
    public int[] eventPos { get; private set; } = new int[0];
    public bool[] endOfTrack { get; private set; } = new bool[0];
    
    public bool[] muteTrack { get; private set; } = new bool[0];

    public bool repeatBarSelection { get; set; } = false;
    public int startBar { get; set; } = 0;
    public int endBar { get; set; } = 0;
    public bool initialized { get; private set; }
    
    #region PRIVATE
    private MidiFile MidiFile { get; set; }
    private byte[] MidiBytes { get; set; }
    private int _timeSignatureDenominator = 2;
    private int barTmp = 0;
    
    private readonly float deltaTimeResolution = 0.001f;
    private double deltaTimeNumerator = 0f;
    private double deltaTimeRest = 0f;
    private int pickUpBarCounter = 0;
    private bool onPickupBarBeginFired = false;
    
    private double fractionalTicks = 0;
    private double lastTime = 0f;
    private double dspTime = 0f;
    private double lastDspTime = 0f;
    private double lastDeltaTime = 0f;
    private double lastDeltaTicks = 0f;
    
    private MidiCommandCode command;
    private MidiEvent midiEvent;
    private MetaEvent metaEvent;
    //ControlChangeEvent controlEvent;
    private bool cancelUpdate = false;

    private double deltaTime = 0f;
    private double periodResolution = 0f;
    //float ticksPerClock = 0f;
    private double deltaTicks = 0f;

    public bool midiFinished { get; private set; }
    public bool musicFinished { get; private set; }
    public bool vocalsFinished { get; private set; }

    private int deltaTimeIterator = 0;
    
    #endregion
}
