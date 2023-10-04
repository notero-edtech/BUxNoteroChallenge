/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;
#if FMOD
using FMODUnity;
#endif

public partial class MidiSeqKaraokeScript : MonoBehaviour, IMidiSender, IMidiSeqControl, MidiSeqKaraoke.IMidiSeqKaraokeCallbacks, MidiSeqKaraoke.IMidiSeqKaraokeEvents
{
    public static MidiSeqKaraokeScript singleton;

    public string id = "";
    public string Id => id;
    
    private MidiSeqKaraoke _midi = new MidiSeqKaraoke();
    public MidiSeqKaraoke Midi { get => _midi; private set => _midi = value; }

    public enum UpdateEnum
    {
        Update,
        LateUpdate,
        FixedUpdate,
        Thread
    }
    
    [Header("MIDI")]
    [ReadOnlyWhenPlaying] public TextAsset midiFileTextAsset;
    
    [Header("MUSIC")]
    public bool music = false;
    private bool _music = false;
    [RestrictInterface (typeof(IAudioSource))]
    [ReadOnlyWhenPlaying] public Object musicInterface;
#if FMOD || WWISE
    [HideInInspector]
#endif
    [ReadOnlyWhenPlaying] public AudioClip musicClip;
#if FMOD
    public EventReference musicClipId;
#else
    [ReadOnlyWhenPlaying] public string musicClipId;
#endif
    [Range(0f, 1f)] public float musicVolume = 1f;
    float _musicVolume = 1f;
    
    [Header("VOCALS")]
    public bool vocals = false;
    private bool _vocals = false;
    [RestrictInterface (typeof(IAudioSource))]
    [ReadOnlyWhenPlaying] public Object vocalsInterface;
#if FMOD || WWISE
    [HideInInspector]
#endif
    [ReadOnlyWhenPlaying] public AudioClip vocalsClip;
#if FMOD
    public EventReference vocalsClipId;
#else
    [ReadOnlyWhenPlaying] public string vocalsClipId;
#endif
    [Range(0f, 1f)] public float vocalsVolume = 1f;
    float _vocalsVolume = 1f;
    
    [Header("UDPATE")]
    public MidiSeqKaraoke.SynchronizationContext synchronizationContext = MidiSeqKaraoke.SynchronizationContext.Midi;
    private MidiSeqKaraoke.SynchronizationContext _synchronizationContext = MidiSeqKaraoke.SynchronizationContext.Midi;
    public UpdateEnum update = UpdateEnum.Update;
    private UpdateEnum _update = UpdateEnum.Update;
    public MidiSeqStates State => Midi.State;
    
    [Header("MIDI OUTPUT")]
    public bool midiOut = true;
    private bool _midiOut = true;
    public bool midiThrough = false;
    private bool _midiThrough = false;
    [Header("CONTROL")]
    public bool playOnStart = false;
    public float delay = 0f;
    public float playingDelay  = 0;
    private float _playingDelay = 0;
    public bool pickUpBar = true;
    bool _pickUpBar = true;
    public bool pickUpBarOnRepeat = true;
    bool _pickUpBarOnRepeat = true;
    [Range(0.1f, 10f)] public float speed = 1f;
    float _speed = 1f;
    [Range(-12, 12)] public int semitone = 0;
    int _semitone = 0;
    [Header("TIME")]
    public bool metronome = false;
    bool _metronome = false;
    public double ticks => Midi.ticks;
    public double time => Midi.time;
    public string timeString => Midi.timeString;
    public double duration => Midi.duration;
    public string durationString => Midi.durationString;
    public float eventsOffset = 0;
    private float _eventsOffset = 0;
    
    public double markerTimeThreshold = 0.2;
    private double _markerTimeThreshold = 0.2;
    //[Header("BEATS")]
    public int beat => Midi.beat;
    public int beatCount => Midi.beatCount;
   
    [Header("BARS")]
    public bool repeatBarSelection = false;
    bool _repeatBarSelection = false;
    public int startBar = 0;
    int _startBar = 0;
    public int endBar = 0;
    int _endBar = 0;
    public int bar => Midi.bar;
    public int barCount => Midi.barCount;
    [Header("OTHERS")]
    public bool forceTrackAsChannel = true;
    bool _forceTrackAsChannel = true;

    public int timeSignatureNumerator => Midi.timeSignatureNumerator;
    public int timeSignatureDenominator => Midi.timeSignatureDenominator;

    public int PPQN = 24;
    int _PPQN = 24;
    public int PPQNMinValue => Midi.PPQNMinValue;
    public float tempo => Midi.tempo;
    public float tempoTicks => Midi.tempoTicks;
    public int keyMajorMinor => Midi.keyMajorMinor;
    public int keySharpsFlats => Midi.keySharpsFlats;
    public List<MidiSeqKaraoke.Bar> bars => Midi.bars;
    bool initialized => Midi.initialized;
    public List<IList<MidiEvent>> tracks => Midi.tracks;
    public int[] eventPos => Midi.eventPos;
    public bool[] endOfTrack => Midi.endOfTrack;
    public bool[] muteTrack => Midi.muteTrack;

    private IAudioSource _musicInterface;
    private IAudioSource _vocalsInterface;
    
    private void Awake()
    {
        singleton = this;
        _musicInterface = musicInterface == null ? gameObject.GetComponent<IAudioSource>() : (musicInterface as IAudioSource);
        _vocalsInterface = vocalsInterface == null ? gameObject.GetComponent<IAudioSource>() : (vocalsInterface as IAudioSource);

        Midi = new MidiSeqKaraoke(_musicInterface, _vocalsInterface);
        Midi.ShortMessageEvent += ShortMessageEventHandler;
        this.Register();
        (Midi as MidiSeqKaraoke.IMidiSeqKaraokeEvents).RegisterCallbacks(this);
        
        _synchronizationContext = Midi.synchronizationContext = synchronizationContext;
        _playingDelay = Midi.playingDelay = playingDelay;
        _music = Midi.music = music;
        _musicVolume = Midi.musicVolume = musicVolume;
        _vocals = Midi.vocals = vocals;
        _vocalsVolume = Midi.vocalsVolume = vocalsVolume;
        _speed = speed; Midi.SetSpeed(speed);
        _semitone = semitone; Midi.SetSemitone(semitone);
        _midiOut = Midi.midiOut = midiOut;
        _midiThrough = Midi.midiThrough = midiThrough;
        _eventsOffset = Midi.eventsOffset = _eventsOffset;
        _markerTimeThreshold = Midi.markerTimeThreshold = markerTimeThreshold;
        _metronome = Midi.metronome = metronome;
        _repeatBarSelection = Midi.repeatBarSelection = repeatBarSelection;
        _startBar = Midi.startBar = startBar;
        _endBar = Midi.endBar = endBar;
        _pickUpBar = Midi.pickUpBar = pickUpBar;
        _pickUpBarOnRepeat =  Midi.pickUpBarOnRepeat = pickUpBarOnRepeat;
        _forceTrackAsChannel = Midi.forceTrackAsChannel = forceTrackAsChannel;
        _lyricTrack = Midi.lyricTrack = lyricTrack;
        _wordTimeOffset = Midi.wordTimeOffset = wordTimeOffset;
        _wordTimeFinishedOffset = Midi.wordTimeFinishedOffset = wordTimeFinishedOffset;
        _forceSentences = Midi.forceSentences = forceSentences;
        _forceSentenceNewLine = Midi.forceSentenceNewLine = forceSentenceNewLine;
        _forceCommaNewLine = Midi.forceCommaNewLine = forceCommaNewLine;
        _sentenceTimeOffset = Midi.senteceTimeOffset = sentenceTimeOffset;
        _versetTimeOffset = Midi.versetTimeOffset = versetTimeOffset;
        _PPQN = Midi.PPQN = PPQN;
    }

    void Init()
    {
#if FMOD
        if(midiFileTextAsset) Initialize(midiFileTextAsset.bytes, vocalsClipId, musicClipId);
#elif WWISE 
        if(midiFileTextAsset) Initialize(midiFileTextAsset.bytes, vocalsClipId, musicClipId);
#else
        if(midiFileTextAsset) Initialize(midiFileTextAsset.bytes, vocalsClip, musicClip);
#endif
    }
    
    IEnumerator Start()
    {
        Init();
        
        yield return new WaitWhile(() => !MIDI.initialized);
        
        if (initialized)
        {
            this.FireAction(0.1f, () =>
            {
                OnMidiLoaded?.Invoke();

                if (playOnStart)
                {
                    this.FireAction(delay, () =>
                    {
                        Play(pickUpBar);
                    });
                }
            });
        }
    }

    void OnDestroy()
    {
        (Midi as MidiSeqKaraoke.IMidiSeqKaraokeEvents)?.UnregisterCallbacks(this);
        Midi.ShortMessageEvent -= ShortMessageEventHandler;
        Midi.Dispose();
        this.Unregister();
    }

    public void Play() => Play(false);
    public void Play(bool aPickUpBar) => Midi.Play(aPickUpBar);
    public void Continue() => Midi.Continue();
    public void Pause() => Midi.Pause();
    public void SetVocals(bool vocals) => this.vocals = Midi.SetVocals(vocals);
    public void SetMusic(bool music) => this.music = Midi.SetMusic(music);
    public void Stop() => Midi.Stop();
    public void SetSpeed(float speed) => this.speed = Midi.SetSpeed(speed);
    public void SetSemitone(int semitone) => this.semitone = Midi.SetSemitone(semitone);
    
    public void Initialize(byte[] midiBytes, AudioClip vClip, AudioClip mClip) => Midi.Initialize(midiBytes, vClip, mClip);
    public void Initialize(byte[] midiBytes, string vClipId, string mClipId) => Midi.Initialize(midiBytes, vClipId, mClipId);
    #if FMOD
    public void Initialize(byte[] midiBytes, EventReference vClipEventReference, EventReference mEventReference) => Midi.Initialize(midiBytes, vClipEventReference, mEventReference);
    #endif
    public void Initialize(byte[] midiBytes) => Midi.Initialize(midiBytes);

    public double TicksToTime(double t) => Midi.TicksToTime(t);
    public double TimeToTicks(double t) => Midi.TicksToTime(t);
}
