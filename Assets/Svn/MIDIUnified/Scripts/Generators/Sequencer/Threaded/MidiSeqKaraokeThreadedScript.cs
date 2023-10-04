/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using System.Collections;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    public static MidiSeqKaraokeThreadedScript singleton;

    public string id = "";
    public string Id => id;
    
    public enum MidiState
    {
        None,
        Loading,
        PickUpBar,
        Playing,
        Pausing,
        Finished
    }

    public enum SynchronizationContext
    {
        Midi,
        Music,
        Vocal
    }

    public enum AudioInput
    {
        Unity,
        BASS24
    }

    public bool threaded = true;

    public bool playOnStart = false;
    public float delay = 0f;
    public AudioInput audioInput = AudioInput.Unity;
    public SynchronizationContext synchronizationContext = SynchronizationContext.Midi;
    
    public MidiState state{ get { return seq.state; } }

    public TextAsset midiFileTextAsset;
        
    public bool midiOut = true;
    public bool synth = true;
    
    public bool music = false;
    public AudioSource audioMusic;
    public AudioSourceBass24 audioMusicBass24;
    public AudioClip musicClip;
    public float musicVolume = 1f;

    public bool vocals = false;
    public AudioSource audioVocals;
    public AudioSourceBass24 audioVocalsBass24;
    public AudioClip vocalsClip;

    public float vocalsVolume = 1f;
       
    [Range(0.1f, 10f)]
    public float speed = 1f;
    [Range(-12, 12)]
    public int semitone = 0;
        
    public double ticks = 0;
    public double time = 0f;
    public double totalTime = 0f;
    public int beat = 0;
    public int beatCount = 0;
    public bool metronome = true;
    public int bar = 0;
    public int barCount = 0;
        
    public bool pickUpBar = true;
    public bool pickUpBarOnRepeat = true;
    public bool forceTrackAsChannel = true;

    public int timeSignatureNumerator = 4;
    public int timeSignatureDenominator = 4;

    public int PPQN = 24;
    public float tempo = 120f;

    public float tempoTicks { get { return ThreadedSequencer.MicrosecondsPerMinute / tempo; } }

    public int keyMajorMinor = 0;
    public int keySharpsFlats = 0;
                            
    public bool repeatBarSelection = false;
    public int startBar = 0;
    public int endBar = 0;

    #region private
    float lastSpeed = -1f;
    int lastSemitone = 0;
    bool lastThreaded = false;
    #endregion

    void Awake()
    {
        singleton = this;

        if (audioInput == AudioInput.Unity)
        {
            audioMusic = audioMusic ?? gameObject.AddComponent<AudioSource>();
            audioVocals = audioVocals ?? gameObject.AddComponent<AudioSource>();
        }
        else if (audioInput == AudioInput.BASS24)
        {
            audioMusicBass24 = audioMusicBass24 ?? gameObject.AddComponent<AudioSourceBass24>();
            audioVocalsBass24 = audioVocalsBass24 ?? gameObject.AddComponent<AudioSourceBass24>();
        }

        seq = new ThreadedSequencer();
        lastThreaded = seq.threaded = threaded;
    }

    IEnumerator Start()
    {
        yield return new WaitWhile(() => !MIDI.initialized);

        seq.threaded = false;

        Initialize(midiFileTextAsset, vocalsClip, musicClip, (b)=>
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

    void Update()
    {
        if (Math.Abs(lastSpeed - speed) > 0.0001f) { SetSpeed(speed); }
        if (lastSemitone != semitone) { SetSemitone(semitone); }
        if(lastThreaded != threaded) { lastThreaded = seq.threaded = threaded; }

        if (!seq.threaded) seq.Run();
    }

    void OnDestroy(){        
        seq?.Terminate();
        seq = null;
        if (midiOut) MidiOut.AllSoundOff();
    }
                   
    public void Play(bool aPickUpBar) => seq.Play(aPickUpBar);
    public void Continue() => seq.Continue();
    public void Stop() => seq.Stop();
    public void Pause() => seq.Pause();
    void Reset() => seq.Reset();
    
    public void SetVocals(bool v)
    {
        vocals = v;

        if (!vocalsClip)
        {
            return;
        }

        if (vocals)
        {
            if (GetMusicState() == StateAudioSource.Playing)
            {
                SetVocalsTime(GetMusicTime());
                PlayVocals();
            }
            else
            {
                if (state == MidiState.Playing)
                {
                    SetVocalsTime(time);
                    PlayVocals();
                }
            }
        }
        else
        {
            PauseVocals();
        }
    }

    public void SetMusic(bool m)
    {
        music = m;
        if (!musicClip)
        {
            return;
        }

        if (music)
        {
            if (GetVocalsState() == StateAudioSource.Playing)
            {
                SetMusicTime(GetVocalsTime());
                PlayMusic();
            }
            else
            {
                if (state == MidiState.Playing)
                {
                    SetMusicTime(time);
                    PlayMusic();
                }
            }
        }
        else
        {
            PauseMusic();
        }
    }
           
    public void SetSpeed(float speed)
    {
        SetMusicSpeed(speed);
        SetVocalsSpeed(speed);

        this.speed = this.lastSpeed = Mathf.Clamp(speed, 0f, 10f);
    }

    public void SetSemitone(int semitone)
    {
        SetMusicSemitone(semitone);
        SetVocalsSpeed(semitone);

        this.semitone = this.lastSemitone = semitone;

        if (midiOut) MidiOut.AllSoundOff(); 
    }

    public void Initialize(TextAsset aMidiFile, AudioClip aVocalClip, AudioClip aMusicClip, Action<bool> onFinished)
    {
        midiFileTextAsset = aMidiFile;
        if (aMidiFile) Initialize(aMidiFile.bytes, aVocalClip, aMusicClip, onFinished);
    }

    public void Initialize(byte[] bytes, AudioClip vocalsClip, AudioClip musicClip, Action<bool> onFinished)
    {
        seq.Initialize(bytes, onFinished);
    }
        
    SynchronizationContext syncContext = SynchronizationContext.Midi;

    void SetSyncContext()
    {
        switch (synchronizationContext)
        {
            case SynchronizationContext.Music:
                syncContext = CheckMusic() ? SynchronizationContext.Music : SynchronizationContext.Midi;
                break;
            case SynchronizationContext.Vocal:
                syncContext = CheckVocals() ? SynchronizationContext.Vocal : SynchronizationContext.Midi;
                break;
            case SynchronizationContext.Midi:
                syncContext = SynchronizationContext.Midi;
                break;
        }
    }
}
