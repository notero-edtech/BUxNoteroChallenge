using System;
using AudioSynthesis.Synthesis;
using ForieroEngine.MIDIUnified;

public partial class MidiSeqKaraoke : IMidiSender
{
    public interface IMidiSeqKaraokeEvents
    {
        event Action OnInitialized;
        event Action<WordText> OnWord;
        event Action OnWordFinished;
        event Action<WordText> OnWordOffset;
        event Action OnWordOffsetFinished;

        event Action<SentenceText> OnSentence;

        //event Action OnSentenceFinished();
        event Action<MidiText> OnVerse;

        //event Action OnVerseFinished;
        event Action OnFinished;
        event Action OnPlay;
        event Action OnResume;
        event Action OnStop;
        event Action OnPause;
        event Action<int> OnRepeat;
        event Action<int> OnPickUpBar;
        event Action<int> OnPickupBarBegin;
        event Action OnPickupBarEnd;
        event Action OnMidiLoaded;
        event Action<float> OnTempoChange;
    }

    public interface IMidiSeqKaraokeCallbacks
    {
        void OnInitializedCallback();
        void OnWordCallback(WordText w);
        void OnWordFinishedCallback();
        void OnWordOffsetCallback(WordText w);
        void OnWordOffsetFinishedCallback();

        void OnSentenceCallback(SentenceText s);

        //void OnSentenceFinished();
        void OnVerseCallback(MidiText t);

        //event Action OnVerseFinished;
        void OnFinishedCallback();
        void OnPlayCallback();
        void OnContinueCallback();
        void OnStopCallback();
        void OnPauseCallback();
        void OnRepeatCallback(int r);
        void OnPickUpBarCallback(int p);
        void OnPickupBarBeginCallback(int p);
        void OnPickupBarEndCallback();
        void OnMidiLoadedCallback();
        void OnTempoChangeCallback(float t);
    }
}
