/* Copyright © Marek Ledvina, Foriero s.r.o. */

using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {            
            Time time = new Time();

            class Time
            {
                public double time = 0;
                public double ticks = 0;
                public double totalTime = 0f;

                public readonly double deltaTimeResolution = 0.001;

                public double deltaTime = 0f;
                public double periodResolution = 0f;
                public double deltaTicks = 0f;

                public int deltaTimeIterator = 0;

                public double fractionalTicks = 0;
                public double lastTime = 0f;
                public double dspTime = 0f;
                public double lastDspTime = 0f;
                public double lastDeltaTime = 0f;
                public double lastDeltaTicks = 0f;

                public double deltaTimeNumerator = 0f;
                public double deltaTimeRest = 0f;

                public void ResetForPlaying()
                {
                    time = 0;
                    ticks = 0;
                    fractionalTicks = 0;
                    lastDeltaTime = 0f;
                    lastDeltaTicks = 0f;
                    deltaTimeNumerator = 0f;
                    deltaTimeRest = 0f;
                }

                public void ResetForLoading()
                {
                    ResetForPlaying();

                    totalTime = 0;
                    deltaTime = 0;
                    periodResolution = 0;
                    deltaTicks = 0;
                    deltaTimeIterator = 0;
                }
            }

            public double TimeToTicks(double aTime) => midi.PPQN * 1000f * aTime * ThreadedSequencer.MicrosecondsPerMillisecond / TempoTicks();
            public double TicksToTime(double ticks) => ticks / midi.PPQN / 1000f / ThreadedSequencer.MicrosecondsPerMillisecond * TempoTicks();
            public double TempoTicks()=> ThreadedSequencer.MicrosecondsPerMinute / midi.tempo;

            void UpdateTime()
            {
                time.deltaTime = time.deltaTimeResolution;

                time.periodResolution = midi.PPQN * 1000f * time.deltaTime * MicrosecondsPerMillisecond;
                //ticksPerClock = PPQN / PPQNMinValue;
                time.deltaTicks = (time.fractionalTicks + time.periodResolution) / TempoTicks();
                time.fractionalTicks += time.periodResolution - time.deltaTicks * TempoTicks();

                if (midi.repeatBarSelection)
                {
                    midi.barTmp = midi.bar;

                    if (midi.beatCount != (int)((time.ticks + time.deltaTicks) / midi.PPQN / (4f / midi.timeSignatureDenominator)) + 1)
                    {
                        if (midi.beatCount % (int)midi.timeSignatureNumerator + 1 == 1)
                        {
                            midi.barTmp++;
                        }
                    }

                    if (midi.barTmp > midi.endBar + 1)
                    {
                        // zavolej na main //
                        OnRepeat(midi.startBar);

                        cancelUpdate = true;
                        SetBar(midi.startBar, true, midi.pickUpBarOnRepeat);
                        return;
                    }
                }

                if (midi.beatCount != (int)(time.ticks / midi.PPQN / (4f / midi.timeSignatureDenominator)) + 1)
                {
                    midi.beat = midi.beatCount % (int)midi.timeSignatureNumerator + 1;

                    if (midi.beat == 1)
                    {
                        midi.bar++;
                    }

                    midi.beatCount = (int)(time.ticks / midi.PPQN / (4f / midi.timeSignatureDenominator)) + 1;

                    //if (metronome) TimeKeeper.BeatEvent(beat, timeSignatureNumerator, timeSignatureDenominator);                    
                }

                time.ticks += time.deltaTicks;
                time.time += time.deltaTime;
                time.lastTime = time.time;
                time.lastDeltaTime = time.deltaTime;
                time.lastDeltaTicks = time.deltaTicks;
            }
        }
    }
}
