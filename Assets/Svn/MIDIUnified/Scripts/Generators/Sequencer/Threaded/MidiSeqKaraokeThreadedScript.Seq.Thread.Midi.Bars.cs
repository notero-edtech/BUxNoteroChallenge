using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    public class Bar
    {
        public double time;
        public double ticks;
        public double timeDuration;
        public double ticksDuration;
        public int timeSignatureNumerator;
        public int timeSignatureDenominator;
        public int majorMinor;
        public int sharpsFlats;
        public double tempo;
        public int[] eventPos;
        public bool[] endOfTrack;
    }

    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {            
            void UpdateBars()
            {
                if (midi.tracks.Count == 0)
                {
                    state = MidiState.Finished;
                    Bar bar = new Bar()
                    {
                        time = 0,
                        ticks = 0,
                        tempo = midi.tempo,
                        timeSignatureNumerator = midi.timeSignatureNumerator,
                        timeSignatureDenominator = midi.timeSignatureDenominator,
                        majorMinor = midi.keyMajorMinor,
                        sharpsFlats = midi.keySharpsFlats,
                        timeDuration = time.time,
                        ticksDuration = time.ticks
                    };

                    midi.bars.Add(bar);
                }

                for (int i = 0; i < midi.tracks.Count; i++)
                {
                    while (midi.tracks[i][midi.eventPos[i]].AbsoluteTime <= time.ticks)
                    {

                        if (midi.endOfTrack[i]) break;

                        events.midiEvent = midi.tracks[i][midi.eventPos[i]];

                        switch (events.midiEvent.CommandCode)
                        {
                            case MidiCommandCode.MetaEvent:
                                events.metaEvent = (events.midiEvent as MetaEvent);
                                switch (events.metaEvent.MetaEventType)
                                {
                                    case MetaEventType.KeySignature:
                                        midi.keyMajorMinor = (events.metaEvent as KeySignatureEvent).MajorMinor;
                                        //if(MIDISettings.isDebug) Debug.Log("MAJOR or MINOR: " + midi.keyMajorMinor);

                                        midi.keySharpsFlats = (events.metaEvent as KeySignatureEvent).SharpsFlats;
                                        //if (MIDISettings.isDebug) Debug.Log("SIGNATURE : " + midi.keySharpsFlats);

                                        break;
                                    case MetaEventType.SequenceTrackName:
                                        //if (MIDISettings.isDebug) Debug.Log("TrackName : " + (events.metaEvent as TextEvent).Text);
                                        break;
                                    case MetaEventType.SetTempo:
                                        TempoEvent tempoEvent = (events.midiEvent as TempoEvent);
                                        midi.tempo = (float)tempoEvent.Tempo;
                                        //if (MIDISettings.isDebug) Debug.Log("Tempo : " + midi.tempo);
                                        break;
                                    case MetaEventType.SmpteOffset:

                                        break;
                                    case MetaEventType.TextEvent:

                                        break;
                                    case MetaEventType.TimeSignature:
                                        TimeSignatureEvent signatureEvent = (events.midiEvent as TimeSignatureEvent);
                                        midi.timeSignatureNumerator = signatureEvent.Numerator;
                                        midi.timeSignatureDenominator = (int)Mathf.Pow(2, signatureEvent.Denominator);
                                        //if (MIDISettings.isDebug) Debug.Log("TimeSignature : " + midi.timeSignatureNumerator + "/" + midi.timeSignatureDenominator);
                                        break;
                                    case MetaEventType.EndTrack:

                                        break;
                                }
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
                                state = MidiState.Finished;
                                time.ticks = time.ticks - time.lastDeltaTicks;
                                time.totalTime = time.time - time.lastDeltaTime;
                                if (midi.bars.Count == 0)
                                {
                                    Bar bar = new Bar()
                                    {
                                        time = 0,
                                        ticks = 0,
                                        tempo = midi.tempo,
                                        timeSignatureNumerator = midi.timeSignatureNumerator,
                                        timeSignatureDenominator = midi.timeSignatureDenominator,
                                        majorMinor = midi.keyMajorMinor,
                                        sharpsFlats = midi.keySharpsFlats,
                                        timeDuration = time.time,
                                        ticksDuration = time.ticks
                                    };

                                    midi.bars.Add(bar);
                                }
                                else
                                {
                                    Bar lastBar = midi.bars.Last();
                                    lastBar.timeDuration = time.time - lastBar.time;
                                    lastBar.ticksDuration = time.ticks - lastBar.ticks;
                                }
                                return;
                            }
                            break;
                        }

                        midi.eventPos[i] = midi.eventPos[i] == midi.tracks[i].Count - 1 ? midi.eventPos[i] : midi.eventPos[i] + 1;
                    }
                }

                if (midi.beatCount != (int)(time.ticks / midi.PPQN / (4f / midi.timeSignatureDenominator)) + 1)
                {
                    midi.beat = midi.beatCount % (int)midi.timeSignatureNumerator + 1;
                    midi.beatCount = (int)(time.ticks / midi.PPQN / (4f / midi.timeSignatureDenominator)) + 1;
                    if (midi.beat == 1)
                    {
                        Bar bar = new Bar()
                        {
                            time = time.time,
                            ticks = time.ticks,
                            tempo = midi.tempo,
                            timeSignatureNumerator = midi.timeSignatureNumerator,
                            timeSignatureDenominator = midi.timeSignatureDenominator,
                            majorMinor = midi.keyMajorMinor,
                            sharpsFlats = midi.keySharpsFlats
                        };

                        if (midi.bars.Count > 0)
                        {
                            Bar lastBar = midi.bars.Last();
                            lastBar.timeDuration = time.time - time.lastDeltaTime - lastBar.time;
                            lastBar.ticksDuration = time.ticks - time.lastDeltaTicks - lastBar.ticks;
                        }

                        bar.eventPos = new int[midi.eventPos.Length];
                        bar.endOfTrack = new bool[midi.endOfTrack.Length];

                        for (int i = 0; i < bar.eventPos.Length; i++)
                        {
                            bar.eventPos[i] = midi.GetTrackEventPosFromAbsoluteTicks(i, time.ticks - 100);
                            bar.endOfTrack[i] = midi.endOfTrack[i];
                        }
                        midi.bars.Add(bar);
                    }
                }

                time.deltaTime = time.deltaTimeResolution;

                time.periodResolution = midi.PPQN * 1000f * time.deltaTime * MicrosecondsPerMillisecond;
                //ticksPerClock = PPQN / PPQNMinValue;
                time.deltaTicks = (time.fractionalTicks + time.periodResolution) / TempoTicks();
                time.fractionalTicks += time.periodResolution - time.deltaTicks * TempoTicks();
                time.ticks += time.deltaTicks;
                time.time += time.deltaTime;

                time.lastTime = time.time;
                time.lastDeltaTime = time.deltaTime;
                time.lastDeltaTicks = time.deltaTicks;
            }

            public void SetBar(int aBarNr, bool play, bool pickUpBar = true)
            {
                if (midi.midiFile == null) return;

                if (MIDISettings.IsDebug) Debug.Log("Setting Bar : " + aBarNr);
                MidiState stateSetBar = state;

                PauseMusic();
                PauseVocals();

                state = MidiState.None;
                Reset();

                if (aBarNr <= 0)
                {
                    time.time = time.lastTime = 0;
                    time.ticks = 0;

                    SetMusicTime(0);
                    SetVocalsTime(0);

                    if (stateSetBar == MidiState.Playing)
                    {
                        Play(pickUpBar);
                    }
                    else
                    {
                        if (play)
                        {
                            Play(pickUpBar);
                        }
                        else
                        {
                            Pause();
                        }
                    }
                }
                else
                {
                    midi.bar = aBarNr;
                    time.time = time.lastTime = midi.bars[aBarNr].time;
                    time.ticks = midi.bars[aBarNr].ticks;

                    SetMusicTime(time.time);
                    SetVocalsTime(time.time);

                    for (int i = 0; i < midi.bars[aBarNr].eventPos.Length; i++)
                    {
                        midi.eventPos[i] = midi.bars[aBarNr].eventPos[i];
                        midi.endOfTrack[i] = midi.bars[aBarNr].endOfTrack[i];
                    }

                    for (int i = 0; i < lyrics.words.Count; i++)
                    {
                        if (lyrics.words[i].absoluteStartTime >= time.ticks)
                        {
                            lyrics.wordPos = lyrics.wordOffsetPos = i - 1;
                            break;
                        }
                    }

                    for (int i = 0; i < lyrics.sentences.Count; i++)
                    {
                        if (lyrics.sentences[i].absoluteStartTime >= time.ticks)
                        {
                            lyrics.sentencePos = i - 1;
                            break;
                        }
                    }

                    for (int i = 0; i < lyrics.verses.Count; i++)
                    {
                        if (lyrics.verses[i].absoluteStartTime >= time.ticks)
                        {
                            lyrics.versePos = i - 1;
                            break;
                        }
                    }

                    if (stateSetBar == MidiState.Playing)
                    {
                        Play(pickUpBar);
                    }
                    else
                    {
                        if (play)
                        {
                            Play(pickUpBar);
                        }
                        else
                        {
                            Pause();
                        }
                    }
                }
            }
        }
    }
}
