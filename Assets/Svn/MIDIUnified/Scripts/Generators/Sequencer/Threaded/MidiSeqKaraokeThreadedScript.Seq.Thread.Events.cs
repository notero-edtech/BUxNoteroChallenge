using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {
            Events events = new Events();

            class Events
            {
                public MidiCommandCode command;
                public int data1 = 0;
                public MidiEvent midiEvent;
                public MetaEvent metaEvent;
            }

            bool CallEvents()
            {
                if (lyrics.wordPos < lyrics.words.Count)
                {
                    if (lyrics.wordPos > 0)
                    {
                        if (!lyrics.words[lyrics.wordPos].finishFired)
                        {
                            if (lyrics.words[lyrics.wordPos].absoluteStartTime <= time.ticks)
                            {
                                OnWordFinished();
                                lyrics.words[lyrics.wordPos].finishFired = true;
                            }
                        }
                    }

                    if (lyrics.words[lyrics.wordPos < 0 ? 0 : lyrics.wordPos].absoluteStartTime <= time.ticks)
                    {
                        OnWord(lyrics.words[lyrics.wordPos < 0 ? 0 : lyrics.wordPos]);
                        lyrics.wordPos++;
                    }
                }

                if (lyrics.wordOffsetPos < lyrics.words.Count)
                {
                    if (lyrics.wordOffsetPos > 0)
                    {
                        if (!lyrics.words[lyrics.wordOffsetPos].finishFired)
                        {
                            if (lyrics.words[lyrics.wordOffsetPos].absoluteStartTime + TimeToTicks(lyrics.wordTimeOffset) + TimeToTicks(lyrics.wordTimeFinishedOffset) <= time.ticks)
                            {
                                OnWordOffsetFinished();
                                lyrics.words[lyrics.wordOffsetPos].finishOffsetFired = true;
                            }
                        }
                    }

                    if (lyrics.words[lyrics.wordOffsetPos < 0 ? 0 : lyrics.wordOffsetPos].absoluteStartTime + TimeToTicks(lyrics.wordTimeOffset) <= time.ticks)
                    {
                        OnWordOffset(lyrics.words[lyrics.wordOffsetPos < 0 ? 0 : lyrics.wordOffsetPos]);
                        lyrics.wordOffsetPos++;
                    }
                }

                if (lyrics.sentencePos < lyrics.sentences.Count)
                {
                    if (lyrics.sentences[lyrics.sentencePos < 0 ? 0 : lyrics.sentencePos].absoluteStartTime + TimeToTicks(lyrics.senteceTimeOffset) <= time.ticks)
                    {
                        OnSentence(lyrics.sentences[lyrics.sentencePos < 0 ? 0 : lyrics.sentencePos]);
                        lyrics.sentencePos++;
                    }
                }

                if (lyrics.versePos < lyrics.verses.Count)
                {
                    if (lyrics.verses[lyrics.versePos < 0 ? 0 : lyrics.versePos].absoluteStartTime + TimeToTicks(lyrics.versetTimeOffset) <= time.ticks)
                    {

                        OnVerse(lyrics.verses[lyrics.versePos < 0 ? 0 : lyrics.versePos]);
                        lyrics.versePos++;
                    }
                }

                for (int i = 0; i < midi.tracks.Count; i++)
                {
                    while (midi.tracks[i][midi.eventPos[i]].AbsoluteTime <= time.ticks)
                    {

                        if (midi.endOfTrack[i]) break;

                        events.midiEvent = midi.tracks[i][midi.eventPos[i]];
                        events.command = events.midiEvent.CommandCode;
                        events.data1 = events.midiEvent.Data1;

                        if (events.command == MidiCommandCode.NoteOff)
                        {
                            events.data1 += semitone;
                        }
                        else if (events.command == MidiCommandCode.NoteOn)
                        {
                            events.data1 += semitone;
                            if (events.midiEvent.Data2 == 0)
                            {
                                events.command = MidiCommandCode.NoteOff;
                            }
                        }

                        if (midiOut)
                        {
                            if (!midi.muteTrack[i])
                            {
                                MidiOut.SendShortMessage((forceTrackAsChannel ? i : (events.midiEvent.Channel - 1)) + (int)events.command, events.data1, events.midiEvent.Data2, -1);
                            }
                        }

                        //ShortMessageEvent?.Invoke((forceTrackAsChannel ? i : (events.midiEvent.Channel - 1)) + (int)events.command, events.data1, events.midiEvent.Data2, -1));
                        
                        switch (events.midiEvent.CommandCode)
                        {
                            case MidiCommandCode.AutoSensing:

                                break;
                            case MidiCommandCode.ChannelAfterTouch:

                                break;
                            case MidiCommandCode.ContinueSequence:

                                break;
                            case MidiCommandCode.ControlChange:
                                //controlEvent = (midiEvent as ControlChangeEvent);

                                break;
                            case MidiCommandCode.Eox:

                                break;
                            case MidiCommandCode.KeyAfterTouch:

                                break;
                            case MidiCommandCode.MetaEvent:
                                events.metaEvent = (events.midiEvent as MetaEvent);
                                switch (events.metaEvent.MetaEventType)
                                {
                                    case MetaEventType.Copyright:
                                        Debug.Log("Copyright : " + (events.metaEvent as TextEvent).Text);
                                        break;
                                    case MetaEventType.CuePoint:

                                        break;
                                    case MetaEventType.DeviceName:

                                        break;
                                    case MetaEventType.EndTrack:

                                        break;
                                    case MetaEventType.KeySignature:
                                        midi.keyMajorMinor = (events.metaEvent as KeySignatureEvent).MajorMinor;
                                        Debug.Log("MAJOR or MINOR: " + midi.keyMajorMinor);
                                        midi.keySharpsFlats = (events.metaEvent as KeySignatureEvent).SharpsFlats;
                                        Debug.Log("SIGNATURE : " + midi.keySharpsFlats);
                                        break;
                                    case MetaEventType.Lyric:

                                        break;
                                    case MetaEventType.Marker:

                                        break;
                                    case MetaEventType.MidiChannel:

                                        break;
                                    case MetaEventType.MidiPort:

                                        break;
                                    case MetaEventType.ProgramName:
                                        Debug.Log("Program Name : " + (events.metaEvent as TextEvent).Text);
                                        break;
                                    case MetaEventType.SequencerSpecific:
                                        //SequencerSpecificEvent sequencerEvent = midiEvent as SequencerSpecificEvent;

                                        break;
                                    case MetaEventType.SequenceTrackName:
                                        Debug.Log("TrackName : " + (events.metaEvent as TextEvent).Text);
                                        break;
                                    case MetaEventType.SetTempo:
                                        TempoEvent tempoEvent = (events.midiEvent as TempoEvent);
                                        midi.tempo = (float)tempoEvent.Tempo;

                                        OnTempoChange(midi.tempo);
                                        break;
                                    case MetaEventType.SmpteOffset:

                                        break;
                                    case MetaEventType.TextEvent:

                                        break;
                                    case MetaEventType.TimeSignature:
                                        TimeSignatureEvent signatureEvent = (events.midiEvent as TimeSignatureEvent);
                                        midi.timeSignatureNumerator = signatureEvent.Numerator;
                                        midi.timeSignatureDenominator = (int)Mathf.Pow(2, signatureEvent.Denominator);
                                        break;
                                    case MetaEventType.TrackInstrumentName:
                                        Debug.Log("Instrument Name : " + (events.metaEvent as TextEvent).Text);
                                        break;
                                    case MetaEventType.TrackSequenceNumber:

                                        break;
                                    default:

                                        break;
                                }
                                break;

                            case MidiCommandCode.NoteOn:

                                break;
                            case MidiCommandCode.NoteOff:

                                break;
                            case MidiCommandCode.PatchChange:

                                break;
                            case MidiCommandCode.PitchWheelChange:

                                break;
                            case MidiCommandCode.StartSequence:

                                break;
                            case MidiCommandCode.StopSequence:

                                break;
                            case MidiCommandCode.Sysex:

                                break;
                            case MidiCommandCode.TimingClock:

                                break;
                        }

                        if (midi.eventPos[i] >= midi.tracks[i].Count - 1)
                        {
                            midi.endOfTrack[i] = true;
                            bool endOfFile = true;
                            for (int k = 0; k < midi.tracks.Count; k++)
                            {
                                if (!midi.endOfTrack[k])
                                {
                                    endOfFile = false;
                                    break;
                                }
                            }

                            if (endOfFile)
                            {
                                time.ticks = time.ticks - time.lastDeltaTicks;
                                time.time = time.time - time.lastDeltaTime;
                                if (midi.repeatBarSelection)
                                {
                                    cancelUpdate = true;
                                    SetBar(midi.startBar, true);
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

                        midi.eventPos[i] = midi.eventPos[i] == midi.tracks[i].Count - 1 ? midi.eventPos[i] : midi.eventPos[i] + 1;
                    }
                }
                return true;
            }
        }
    }
}
