using System;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraoke : IMidiSender
{
    public event ShortMessageEventHandler ShortMessageEvent;

    public event Action OnInitialized;
    public event Action<WordText> OnWord;
    public event Action OnWordFinished;
    public event Action<WordText> OnWordOffset;
    public event Action OnWordOffsetFinished;
    public event Action<SentenceText> OnSentence;
    //public event Action OnSentenceFinished;
    public event Action<MidiText> OnVerse;
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

    int data1 = 0;

    bool CallEvents()
    {
        if (wordPos < words.Count)
        {
            if (wordPos > 0)
            {
                if (!words[wordPos].finishFired)
                {
                    if (words[wordPos].absoluteStartTime <= ticks)
                    {
                        OnWordFinished?.Invoke();
                        words[wordPos].finishFired = true;
                    }
                }
            }

            if (words[wordPos < 0 ? 0 : wordPos].absoluteStartTime <= ticks)
            {
                OnWord?.Invoke(words[wordPos < 0 ? 0 : wordPos]);
                wordPos++;
            }
        }

        if (wordOffsetPos < words.Count)
        {
            if (wordOffsetPos > 0)
            {
                if (!words[wordOffsetPos].finishFired)
                {
                    if (words[wordOffsetPos].absoluteStartTime + TimeToTicks(wordTimeOffset) + TimeToTicks(wordTimeFinishedOffset) <= ticks)
                    {
                        OnWordOffsetFinished?.Invoke();
                        words[wordOffsetPos].finishOffsetFired = true;
                    }
                }
            }

            if (words[wordOffsetPos < 0 ? 0 : wordOffsetPos].absoluteStartTime + TimeToTicks(wordTimeOffset) <= ticks)
            {
                OnWordOffset?.Invoke(words[wordOffsetPos < 0 ? 0 : wordOffsetPos]);
                wordOffsetPos++;
            }
        }

        if (sentencePos < sentences.Count)
        {
            if (sentences[sentencePos < 0 ? 0 : sentencePos].absoluteStartTime + TimeToTicks(senteceTimeOffset) <= ticks)
            {
                OnSentence?.Invoke(sentences[sentencePos < 0 ? 0 : sentencePos]);
                sentencePos++;
            }
        }

        if (versePos < verses.Count)
        {
            if (verses[versePos < 0 ? 0 : versePos].absoluteStartTime + TimeToTicks(versetTimeOffset) <= ticks)
            {
                OnVerse?.Invoke(verses[versePos < 0 ? 0 : versePos]);
                versePos++;
            }
        }
                
        for (int i = 0; i < tracks.Count; i++)
        {
            if (!(eventPos[i] >= 0 && eventPos[i] < tracks[i].Count)) continue;
            while (tracks[i][eventPos[i]].AbsoluteTime + TimeToTicks(eventsOffset) <= ticks)
            {
                bool eot = false;
                if (endOfTrack[i]) break;

                midiEvent = tracks[i][eventPos[i]];

                command = midiEvent.CommandCode;
                data1 = midiEvent.Data1;

                if (command == MidiCommandCode.NoteOff) { data1 += semitone; }
                else if (command == MidiCommandCode.NoteOn)
                {
                    data1 += semitone;
                    if (midiEvent.Data2 == 0) { command = MidiCommandCode.NoteOff; }
                }

                if (midiOut && !muteTrack[i]) 
                    MidiOut.SendShortMessage(
                        (forceTrackAsChannel ? i : (midiEvent.Channel - 1)) + (int)command,
                        data1,
                        midiEvent.Data2,
                        -1);

                ShortMessageEvent?.Invoke(
                    (forceTrackAsChannel ? i : (midiEvent.Channel - 1)) + (int)command,
                    data1,
                    midiEvent.Data2,
                    -1);

                switch (midiEvent.CommandCode)
                {
                    case MidiCommandCode.AutoSensing: break;
                    case MidiCommandCode.ChannelAfterTouch: break;
                    case MidiCommandCode.ContinueSequence: break;
                    case MidiCommandCode.ControlChange: var controlChangeEvent = (midiEvent as ControlChangeEvent); break;
                    case MidiCommandCode.Eox: break;
                    case MidiCommandCode.KeyAfterTouch: break;
                    case MidiCommandCode.MetaEvent:
                        metaEvent = (midiEvent as MetaEvent);
                        switch (metaEvent.MetaEventType)
                        {
                            case MetaEventType.Copyright: if(debug) Debug.Log("Copyright : " + (metaEvent as TextEvent).Text); break;
                            case MetaEventType.CuePoint: break;
                            case MetaEventType.DeviceName: break;
                            case MetaEventType.EndTrack: eot = true; break;
                            case MetaEventType.KeySignature:
                                keyMajorMinor = (metaEvent as KeySignatureEvent).MajorMinor;
                                if(debug) Debug.Log("MAJOR or MINOR: " + keyMajorMinor);
                                keySharpsFlats = (metaEvent as KeySignatureEvent).SharpsFlats;
                                if(debug) Debug.Log("SIGNATURE : " + keySharpsFlats);
                                break;
                            case MetaEventType.Lyric: break;
                            case MetaEventType.Marker: break;
                            case MetaEventType.MidiChannel: break;
                            case MetaEventType.MidiPort: break;
                            case MetaEventType.ProgramName: if(debug) Debug.Log("Program Name : " + (metaEvent as TextEvent).Text); break;
                            case MetaEventType.SequencerSpecific: var sequencerSpecificEvent = midiEvent as SequencerSpecificEvent; break;
                            case MetaEventType.SequenceTrackName: if(debug) Debug.Log("TrackName : " + (metaEvent as TextEvent).Text); break;
                            case MetaEventType.SetTempo:
                                var tempoEvent = (midiEvent as TempoEvent);
                                tempo = (float)tempoEvent.Tempo;
                                OnTempoChange?.Invoke(tempo);
                                break;
                            case MetaEventType.SmpteOffset: break;
                            case MetaEventType.TextEvent: break;
                            case MetaEventType.TimeSignature:
                                TimeSignatureEvent signatureEvent = (midiEvent as TimeSignatureEvent);
                                timeSignatureNumerator = signatureEvent.Numerator;
                                _timeSignatureDenominator = signatureEvent.Denominator;
                                timeSignatureDenominator = (int)Mathf.Pow(2, _timeSignatureDenominator);
                                break;
                            case MetaEventType.TrackInstrumentName: if(debug) Debug.Log("Instrument Name : " + (metaEvent as TextEvent).Text); break;
                            case MetaEventType.TrackSequenceNumber: break;
                            default: break;
                        }
                        break;

                    case MidiCommandCode.NoteOn: break;
                    case MidiCommandCode.NoteOff: break;
                    case MidiCommandCode.PatchChange: break;
                    case MidiCommandCode.PitchWheelChange: break;
                    case MidiCommandCode.StartSequence: break;
                    case MidiCommandCode.StopSequence: break;
                    case MidiCommandCode.Sysex: break;
                    case MidiCommandCode.TimingClock: break;
                }

                if (eot || eventPos[i] >= tracks[i].Count - 1)
                {
                    endOfTrack[i] = true;
                    bool eof = endOfTrack.All(b => b);
                    
                    if (eof)
                    {
                        ticks = ticks - lastDeltaTicks;
                        time = time - lastDeltaTime;
                        if (repeatBarSelection)
                        {
                            cancelUpdate = true;
                            SetBar(startBar, true);
                            return false;
                        }
                        else
                        {
                            cancelUpdate = true;
                            midiFinished = true;
                            return false;
                        }
                    }
                    break;
                }

                eventPos[i] = eventPos[i] == tracks[i].Count - 1 ? eventPos[i] : eventPos[i] + 1;
            }
        }
        return true;
    }
}
