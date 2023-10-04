using System;
using System.Runtime.InteropServices;

using UnityEngine;

using Debug = UnityEngine.Debug;
public partial class FMODTimelineMidiSeq : MonoBehaviour
{
    public event Action OnInitialized;
    public event Action<MidiSeqKaraoke.WordText> OnWord;
    public event Action OnWordFinished;
    public event Action<MidiSeqKaraoke.WordText> OnWordOffset;
    public event Action OnWordOffsetFinished;
    public event Action<MidiSeqKaraoke.SentenceText> OnSentence;
    public event Action<MidiSeqKaraoke.MidiText> OnVerse;
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
    public void OnInitializedCallback() => OnInitialized?.Invoke();
    public void OnWordCallback(MidiSeqKaraoke.WordText w) => OnWord?.Invoke(w);
    public void OnWordFinishedCallback() => OnWordFinished?.Invoke();
    public void OnWordOffsetCallback(MidiSeqKaraoke.WordText w) => OnWordOffset?.Invoke(w);
    public void OnWordOffsetFinishedCallback() => OnWordOffsetFinished?.Invoke();
    public void OnSentenceCallback(MidiSeqKaraoke.SentenceText s) => OnSentence?.Invoke(s);
    public void OnVerseCallback(MidiSeqKaraoke.MidiText t) => OnVerse?.Invoke(t);
    public void OnFinishedCallback() => OnFinished?.Invoke();
    public void OnPlayCallback() => OnPlay?.Invoke();
    public void OnContinueCallback() => OnResume?.Invoke();
    public void OnStopCallback() => OnStop?.Invoke();
    public void OnPauseCallback() => OnPause?.Invoke();
    public void OnRepeatCallback(int r) => OnRepeat?.Invoke(r);
    public void OnPickUpBarCallback(int p) => OnPickUpBar?.Invoke(p);
    public void OnPickupBarBeginCallback(int p) => OnPickupBarBegin?.Invoke(p);
    public void OnPickupBarEndCallback() => OnPickupBarEnd?.Invoke();
    public void OnMidiLoadedCallback() => OnMidiLoaded?.Invoke();
    public void OnTempoChangeCallback(float t) => OnTempoChange?.Invoke(t);
}