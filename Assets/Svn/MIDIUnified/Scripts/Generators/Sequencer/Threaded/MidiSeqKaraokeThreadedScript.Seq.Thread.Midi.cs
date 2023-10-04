/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        //	BPM – it’s a beats per minute 60 000 000/MPQN
        //	PPQN – Pulses per quater note, resolution found in MIDI file
        //	MPQN – microseconds per quaternote or quaternote duration, range is 0-8355711 microseconds (according to MIDI file specification)
        //	QLS – seconds per quaternote (MPQN/1000000)
        //	TDPS – seconds per tick (QLS/PPQN)
        //  TPS - ticks per second (1000000/QLS/PPQN) = (1000000/1/MPQN/1000000/PPQN)

        public const int MicrosecondsPerMinute = 60000000;
        public const int MicrosecondsPerSecond = 1000000;
        public const int MicrosecondsPerMillisecond = 1000;
        public const int PPQNMinValue = 24;

        partial class SequencerThread
        {
            public double speed { get { return midi.speed; } set { midi.speed = value; } }
            public int semitone { get { return midi.semitone; } set { midi.semitone = value; } }
            public bool forceTrackAsChannel { get { return midi.forceTrackAsChannel; } set { midi.forceTrackAsChannel = value; } }

            Midi midi = new Midi();
                        
            class Midi
            {
                public MidiFile midiFile;

                public volatile bool forceTrackAsChannel = true;

                public List<Bar> bars = new List<Bar>();

                public int keyMajorMinor = 0;
                public int keySharpsFlats = 0;

                public int timeSignatureNumerator = 4;
                public int timeSignatureDenominator = 4;

                public int PPQN = 24;
                
                public double tempo = 120;
                public double speed = 1;
                public volatile int semitone = 0;

                public int beat = 0;
                public int beatCount = 0;
                public int bar = 0;
                public int barTmp = 0;
                public int barCount = 0;

                public bool pickupBar = true;
                public bool pickUpBarOnRepeat = true;
                public bool repeatBarSelection = true;
                public int startBar = 0;
                public int endBar = 0;
                
                public List<IList<MidiEvent>> tracks = new List<IList<MidiEvent>>();
                public int[] eventPos = new int[0];
                public bool[] endOfTrack = new bool[0];
                public bool[] muteTrack = new bool[0];

                public int GetTrackEventPosFromAbsoluteTicks(int aTrackIndex, double aAbsoluteTicks)
                {
                    for (int i = 0; i < tracks[aTrackIndex].Count; i++)
                    {
                        if (tracks[aTrackIndex][i].AbsoluteTime > aAbsoluteTicks)
                        {
                            return i - 1;
                        }
                    }
                    return tracks[aTrackIndex].Count - 1;
                }

                public void ResetForPlaying()
                {
                    beat = 0;
                    beatCount = bars.Count;
                    bar = 0;

                    if (midiFile != null)
                    {
                        eventPos = new int[midiFile.Tracks];
                        endOfTrack = new bool[midiFile.Tracks];
                        muteTrack = new bool[midiFile.Tracks];
                    }
                }

                public void ResetForLoading()
                {
                    midiFile = null;

                    ResetForPlaying();

                    barCount = 0;
                    barTmp = 0;
                    tracks = new List<IList<MidiEvent>>();
                    eventPos = new int[0];
                    endOfTrack = new bool[0];
                    muteTrack = new bool[0];
                    bars = new List<Bar>();
                }
            }
        }
    }
}
