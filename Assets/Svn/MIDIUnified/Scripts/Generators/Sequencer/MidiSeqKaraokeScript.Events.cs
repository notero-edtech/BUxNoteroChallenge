using UnityEngine;
using System;
using ForieroEngine.MIDIUnified;

public partial class MidiSeqKaraokeScript : MonoBehaviour, IMidiSender, MidiSeqKaraoke.IMidiSeqKaraokeCallbacks
{
    public event ShortMessageEventHandler ShortMessageEvent;

    private void ShortMessageEventHandler(int aCommand, int aData1, int aData2, int deviceId)
        => ShortMessageEvent?.Invoke(aCommand, aData1, aData2, deviceId);
    public event Action OnInitialized;
    public event Action<MidiSeqKaraoke.WordText> OnWord;
    public event Action OnWordFinished;
    public event Action<MidiSeqKaraoke.WordText> OnWordOffset;
    public event Action OnWordOffsetFinished;
    public event Action<MidiSeqKaraoke.SentenceText> OnSentence;
    //public event Action OnSentenceFinished;
    public event Action<MidiSeqKaraoke.MidiText> OnVerse;
    //public event Action OnVerseFinished;
    public event Action OnFinished;
    public event Action OnPlay;
    public event Action OnResume;
    public event Action OnStop;
    public event Action OnPause;
    public event Action<int> OnRepeat;
    public event Action<int> OnPickUpBar;
    public event Action<int> OnPickupBarBegin;
    public event Action OnPickupBarEnd;
    public event Action OnMidiLoaded;
    public event Action<float> OnTempoChange;


    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnInitializedCallback() => OnInitialized?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnWordCallback(MidiSeqKaraoke.WordText w) => OnWord?.Invoke(w);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnWordFinishedCallback() => OnWordFinished?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnWordOffsetCallback(MidiSeqKaraoke.WordText w) => OnWordOffset?.Invoke(w);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnWordOffsetFinishedCallback() => OnWordOffsetFinished?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnSentenceCallback(MidiSeqKaraoke.SentenceText s) => OnSentence?.Invoke(s);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnVerseCallback(MidiSeqKaraoke.MidiText t) => OnVerse?.Invoke(t);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnFinishedCallback() => OnFinished?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnPlayCallback() => OnPlay?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnContinueCallback() => OnResume?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnStopCallback() => OnStop?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnPauseCallback() => OnPause?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnRepeatCallback(int r) => OnRepeat?.Invoke(r);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnPickUpBarCallback(int p) => OnPickUpBar?.Invoke(p);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnPickupBarBeginCallback(int p) => OnPickupBarBegin?.Invoke(p);
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnPickupBarEndCallback() => OnPickupBarEnd?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnMidiLoadedCallback() => OnMidiLoaded?.Invoke();
    void MidiSeqKaraoke.IMidiSeqKaraokeCallbacks.OnTempoChangeCallback(float t) => OnTempoChange?.Invoke(t);
}
