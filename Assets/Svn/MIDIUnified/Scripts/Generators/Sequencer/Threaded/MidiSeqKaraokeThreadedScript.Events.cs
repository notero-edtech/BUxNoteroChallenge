using System;
using ForieroEngine.MIDIUnified;
using UnityEngine;


public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    public event ShortMessageEventHandler ShortMessageEvent;

    #region init
    public event Action OnMidiLoaded;
    public event Action<bool> OnInitialized;
    #endregion

    #region control
    public event Action OnPlay;
    public event Action OnContinue;
    public event Action OnStop;
    public event Action OnPause;
    public event Action OnFinished;
    #endregion

    #region bars
    public event Action<int> OnRepeat;
    public event Action<int> OnPickUpBar;
    public event Action<int> OnPickupBarBegin;
    public event Action OnPickupBarEnd;
    #endregion

    #region lyrics
    public event Action<WordText> OnWord;
    public event Action OnWordFinished;
    public event Action<WordText> OnWordOffset;
    public event Action OnWordOffsetFinished;
    public event Action<SentenceText> OnSentence;
    //public event Action OnSentenceFinished;
    public event Action<MidiText> OnVerse;
    //public event Action OnVerseFinished;
    #endregion

    #region changes
    public event Action<float> OnTempoChange;
    #endregion
}
