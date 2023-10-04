/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class TLUnityMetronome : MonoBehaviour
{
    private class BeatItem
    {
        public BeatType beatType = BeatType.Unknown;
        public double scheduleTime = 0f;
        public bool invoked = false;

        public int bpm = 0;
        public int subdivision = 0;

        public List<BeatItem> subdivisions = new List<BeatItem>();

        public void Init(BeatType beatType, int bpm, int subdivision)
        {
            this.beatType = beatType;
            this.bpm = bpm;
            this.subdivision = subdivision;
        }

        public void PlayScheduled(double time)
        {
            invoked = false;
            scheduleTime = time;
            if (AudioSettings.dspTime <= time)
                MidiOut.SchedulePercussion(
                    MIDIPercussionSettings.GetPercussionEnum(beatType),
                    MIDIPercussionSettings.GetPercussionAttack(beatType),
                    time,
                    true);

            for (var s = 0; s < subdivision; s++)
            {
                time = scheduleTime + (s + 1) * 60.0 / bpm / (subdivision + 1.0);
                subdivisions[s].scheduleTime = time;
                subdivisions[s].invoked = false;
                if (AudioSettings.dspTime < time)
                {
                    MidiOut.SchedulePercussion(
                        MIDIPercussionSettings.GetPercussionEnum(BeatType.Subdivision),
                        MIDIPercussionSettings.GetPercussionAttack(BeatType.Subdivision),
                        time,
                        true
                    );
                }
            }
        }
    }
}
