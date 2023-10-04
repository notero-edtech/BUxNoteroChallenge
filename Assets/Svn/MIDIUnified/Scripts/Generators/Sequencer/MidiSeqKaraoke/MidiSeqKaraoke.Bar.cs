using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraoke : IMidiSender
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
        public float tempo;
        public int[] eventPos;
        public bool[] endOfTrack;
    }

    public List<Bar> bars = new List<Bar>();
    
    int GetBeatCount(double ticks) => (int)(ticks / PPQN / (4f / timeSignatureDenominator)) + 1;
    int GetBeat(int beatCount) => beatCount % (int)timeSignatureNumerator + 1;

    void UpdateBars()
    {
        if(tracks.Count == 0)
        {
            State = MidiSeqStates.Finished;
            Bar bar = new Bar()
            {
                time = 0,
                ticks = 0,
                tempo = this.tempo,
                timeSignatureNumerator = this.timeSignatureNumerator,
                timeSignatureDenominator = this.timeSignatureDenominator,
                majorMinor = this.keyMajorMinor,
                sharpsFlats = this.keySharpsFlats,
                timeDuration = this.time,
                ticksDuration = this.ticks
            };

            bars.Add(bar);
        }

        for (int i = 0; i < tracks.Count; i++)
        {
            while (tracks[i][eventPos[i]].AbsoluteTime <= ticks)
            {
                bool eot = false;
                if (endOfTrack[i]) break;

                midiEvent = tracks[i][eventPos[i]];

                switch (midiEvent.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        metaEvent = (midiEvent as MetaEvent);
                        switch (metaEvent.MetaEventType)
                        {
                            case MetaEventType.KeySignature:
                                keyMajorMinor = (metaEvent as KeySignatureEvent).MajorMinor;
                                if(debug) Debug.Log("MAJOR or MINOR: " + keyMajorMinor);
                                keySharpsFlats = (metaEvent as KeySignatureEvent).SharpsFlats;
                                if(debug) Debug.Log("SIGNATURE : " + keySharpsFlats);
                                break;
                            case MetaEventType.SequenceTrackName:
                                if(debug) Debug.Log("TrackName : " + (metaEvent as TextEvent).Text);
                                break;
                            case MetaEventType.SetTempo:
                                TempoEvent tempoEvent = (midiEvent as TempoEvent);
                                tempo = (float)tempoEvent.Tempo;
                                break;
                            case MetaEventType.SmpteOffset:

                                break;
                            case MetaEventType.TextEvent:

                                break;
                            case MetaEventType.TimeSignature:
                                TimeSignatureEvent signatureEvent = (midiEvent as TimeSignatureEvent);
                                timeSignatureNumerator = signatureEvent.Numerator;
                                _timeSignatureDenominator = signatureEvent.Denominator;
                                timeSignatureDenominator = (int)Mathf.Pow(2, _timeSignatureDenominator);
                                break;
                            case MetaEventType.EndTrack:
                                eot = true;
                                break;
                        }
                        break;
                }

                if (eot || eventPos[i] >= tracks[i].Count - 1)
                {
                    endOfTrack[i] = true;
                    bool eof = endOfTrack.All(b => b);
                    
                    if (eof)
                    {
                        State = MidiSeqStates.Finished;
                        ticks = ticks - lastDeltaTicks;
                        duration = time - lastDeltaTime;
                        if (bars.Count == 0)
                        {
                            Bar bar = new Bar()
                            {
                                time = 0,
                                ticks = 0,
                                tempo = this.tempo,
                                timeSignatureNumerator = this.timeSignatureNumerator,
                                timeSignatureDenominator = this.timeSignatureDenominator,
                                majorMinor = this.keyMajorMinor,
                                sharpsFlats = this.keySharpsFlats,
                                timeDuration = this.time,
                                ticksDuration = this.ticks
                            };

                            bars.Add(bar);
                        }
                        else
                        {
                            Bar lastBar = bars.Last();
                            lastBar.timeDuration = time - lastBar.time;
                            lastBar.ticksDuration = ticks - lastBar.ticks;
                        }
                        return;
                    }
                    break;
                }

                eventPos[i] = eventPos[i] == tracks[i].Count - 1 ? eventPos[i] : eventPos[i] + 1;
            }
        }

        if (beatCount != GetBeatCount(ticks))
        {
            beat = GetBeat(beatCount);
            beatCount = GetBeatCount(ticks);
            if (beat == 1)
            {
                Bar bar = new Bar()
                {
                    time = this.time,
                    ticks = this.ticks,
                    tempo = this.tempo,
                    timeSignatureNumerator = this.timeSignatureNumerator,
                    timeSignatureDenominator = this.timeSignatureDenominator,
                    majorMinor = this.keyMajorMinor,
                    sharpsFlats = this.keySharpsFlats
                };

                if (bars.Count > 0)
                {
                    Bar lastBar = bars.Last();
                    lastBar.timeDuration = time - lastDeltaTime - lastBar.time;
                    lastBar.ticksDuration = ticks - lastDeltaTicks - lastBar.ticks;
                }

                bar.eventPos = new int[eventPos.Length];
                bar.endOfTrack = new bool[endOfTrack.Length];

                for (int i = 0; i < bar.eventPos.Length; i++)
                {
                    bar.eventPos[i] = GetTrackEventPosFromAbsoluteTicks(i, ticks - 100);
                    bar.endOfTrack[i] = endOfTrack[i];
                }
                bars.Add(bar);
            }
        }

        deltaTime = deltaTimeResolution;

        periodResolution = PPQN * 1000f * deltaTime * MicrosecondsPerMillisecond;
        //ticksPerClock = PPQN / PPQNMinValue;
        deltaTicks = (fractionalTicks + periodResolution) / tempoTicks;
        fractionalTicks += periodResolution - deltaTicks * tempoTicks;
        ticks += deltaTicks;
        time += deltaTime;

        lastTime = time;
        lastDeltaTime = deltaTime;
        lastDeltaTicks = deltaTicks;
    }

    public void SetBar(int aBarNr, bool play, bool pickUpBar = true)
    {
        if (MidiFile != null)
        {
            Debug.Log(State.ToString());
            MidiSeqStates stateSetBar = State;

            MusicInterface.Pause();
            VocalsInterface.Pause();

            CancelPickUpBarCounting();
            State = MidiSeqStates.None;
            ResetSequencer();

            if (aBarNr <= 0)
            {
                time = lastTime = 0;
                ticks = 0;

                MusicInterface.SetTime(0);
                VocalsInterface.SetTime(0);

                if (stateSetBar == MidiSeqStates.Playing) Play(pickUpBar);
                else { if (play) Play(pickUpBar); else Pause();  }
            }
            else
            {
                bar = aBarNr;
                time = lastTime = bars[aBarNr].time;
                ticks = bars[aBarNr].ticks;

                MusicInterface.SetTime(time);
                VocalsInterface.SetTime(time);

                for (int i = 0; i < bars[aBarNr].eventPos.Length; i++)
                {
                    eventPos[i] = bars[aBarNr].eventPos[i];
                    endOfTrack[i] = bars[aBarNr].endOfTrack[i];
                }

                for (int i = 0; i < words.Count; i++) { if (words[i].absoluteStartTime >= ticks) { wordPos = wordOffsetPos = i - 1; break; } }
                for (int i = 0; i < sentences.Count; i++) { if (sentences[i].absoluteStartTime >= ticks) { sentencePos = i - 1; break; } }
                for (int i = 0; i < verses.Count; i++) { if (verses[i].absoluteStartTime >= ticks) { versePos = i - 1; break; } }
                if (stateSetBar == MidiSeqStates.Playing) { Play(pickUpBar); }
                else { if (play) Play(pickUpBar); else Pause(); }
            }
        }
    }
}
