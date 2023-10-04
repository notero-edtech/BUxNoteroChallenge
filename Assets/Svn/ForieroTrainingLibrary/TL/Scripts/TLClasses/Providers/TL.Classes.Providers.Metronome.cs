/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Classes.Providers
{
    public class MetronomeProvider
    {
        public int beats
        {
            get => TL.Providers.Metronome.beats;
            set => TL.Providers.Metronome.beats = value;
        }

        public int bpm
        {
            get => TL.Providers.Metronome.bpm;
            set => TL.Providers.Metronome.bpm = value;
        }

        public int subdivisions
        {
            get => TL.Providers.Metronome.subdivisions;
            set => TL.Providers.Metronome.subdivisions = value;
        }

        public bool pickupBar
        {
            get => TL.Providers.Metronome.pickupBar;
            set => TL.Providers.Metronome.pickupBar = value;
        }

        public int beat
        {
            get => TL.Providers.Metronome.beat;
            set => TL.Providers.Metronome.beat = value;
        }

        public double totalTime
        {
            get => TL.Providers.Metronome.totalTime;
            set => TL.Providers.Metronome.totalTime = value;
        }

        public double measureTime
        {
            get => TL.Providers.Metronome.measureTime;
            set => TL.Providers.Metronome.measureTime = value;
        }

        public virtual void Reset() { }

        public virtual double Start() { throw new NotImplementedException("Method not implmented : Start!"); }
        public virtual void Stop() { throw new NotImplementedException("Method not implmeneted : Stop!"); }
        public virtual void Mute() { throw new NotImplementedException("Method not implmeneted : Mute!"); }
        public virtual void UnMute() { throw new NotImplementedException("Method not implmeneted : UnMute!"); }
        public virtual void ScheduleEvent(double time, Action OnScheduledEvent) { throw new NotImplementedException("Method not implmeneted : ScheduleEvent!"); }
        public virtual void CancelScheduledEvents() { throw new NotImplementedException("Method not implmeneted : CancelScheduledEvents!"); }
    }
}
