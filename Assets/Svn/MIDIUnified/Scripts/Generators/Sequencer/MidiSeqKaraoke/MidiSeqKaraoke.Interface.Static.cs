using System;

public static class MidiSeqKaraokeInterface
{
    public static void RegisterCallbacks(this MidiSeqKaraoke.IMidiSeqKaraokeEvents e,
        MidiSeqKaraoke.IMidiSeqKaraokeCallbacks c)
    {
        if (e == null || c == null) return;
        e.OnInitialized += c.OnInitializedCallback;
        e.OnWord += c.OnWordCallback;
        e.OnWordFinished += c.OnWordFinishedCallback;
        e.OnWordOffset += c.OnWordOffsetCallback;
        e.OnWordOffsetFinished += c.OnWordOffsetFinishedCallback;
        e.OnSentence += c.OnSentenceCallback;
        e.OnVerse += c.OnVerseCallback;
        e.OnFinished += c.OnFinishedCallback;
        e.OnPlay += c.OnPlayCallback;
        e.OnResume += c.OnContinueCallback;
        e.OnStop += c.OnStopCallback;
        e.OnPause += c.OnPauseCallback;
        e.OnRepeat += c.OnRepeatCallback;
        e.OnPickUpBar += c.OnPickUpBarCallback;
        e.OnPickupBarBegin += c.OnPickupBarBeginCallback;
        e.OnPickupBarEnd += c.OnPickupBarEndCallback;
        e.OnMidiLoaded += c.OnMidiLoadedCallback;
        e.OnTempoChange += c.OnTempoChangeCallback;
    }

    public static void UnregisterCallbacks(this MidiSeqKaraoke.IMidiSeqKaraokeEvents e,
        MidiSeqKaraoke.IMidiSeqKaraokeCallbacks c)
    {
        if (e == null || c == null) return;
        e.OnInitialized -= c.OnInitializedCallback;
        e.OnWord -= c.OnWordCallback;
        e.OnWordFinished -= c.OnWordFinishedCallback;
        e.OnWordOffset -= c.OnWordOffsetCallback;
        e.OnWordOffsetFinished -= c.OnWordOffsetFinishedCallback;
        e.OnSentence -= c.OnSentenceCallback;
        e.OnVerse -= c.OnVerseCallback;
        e.OnFinished -= c.OnFinishedCallback;
        e.OnPlay -= c.OnPlayCallback;
        e.OnResume -= c.OnContinueCallback;
        e.OnStop -= c.OnStopCallback;
        e.OnPause -= c.OnPauseCallback;
        e.OnRepeat -= c.OnRepeatCallback;
        e.OnPickUpBar -= c.OnPickUpBarCallback;
        e.OnPickupBarBegin -= c.OnPickupBarBeginCallback;
        e.OnPickupBarEnd -= c.OnPickupBarEndCallback;
        e.OnMidiLoaded -= c.OnMidiLoadedCallback;
        e.OnTempoChange -= c.OnTempoChangeCallback;
    }
}