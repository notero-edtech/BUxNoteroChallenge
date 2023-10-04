/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.Training.Classes.Providers
{
    public class MidiProvider
    {
        public virtual void NoteDispatch(int interval1Low, float toneDuration, float toneGap, int attack, int instrumentChannel, Action AStart = null, Action AStop = null)
        {
            throw new NotImplementedException("Method not implmeneted : NoteDispatch!");
        }

        public virtual void SchedulePercussion(TL.Enums.MIDI.PercussionEnum percussionEnum, int attack, float scheduleTime = 0)
        {
            throw new NotImplementedException("Method not implmeneted : NoteDispatch!");
        }

        public virtual void Reset()
        {
            throw new NotImplementedException("Method not implmeneted : Reset!");
        }
    }
}
