/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static class Beats
        {
            private static readonly Queue<Beat> ScheduledBeats = new Queue<Beat>();

            public static void Cancel()
            {
                MIDIPercussion.CancelScheduledPercussion();
                ScheduledBeats.Clear();
            }

            public static void Play(Beat beat)
            {
                if (_playbackState != PlaybackState.Playing) return;

                //if (Metronome.mute) return;

                if (beat.number == 1)
                {
                    MidiOut.SchedulePercussion(
                        MIDIPercussionSettings.GetPercussionEnum(BeatType.Heavy),
                        MIDIPercussionSettings.GetPercussionAttack(BeatType.Heavy),
                        Time.Metronome.Offset / speed
                    );
                }
                else
                {
                    MidiOut.SchedulePercussion(
                        MIDIPercussionSettings.GetPercussionEnum(BeatType.Light),
                        MIDIPercussionSettings.GetPercussionAttack(BeatType.Light),
                        Time.Metronome.Offset / speed
                    );
                }

                float beatTime = 0;

                for (var i = 0; i < Metronome.Subdivisions; i++)
                {
                    beatTime += beat.totalTime / (Metronome.Subdivisions + 1) / speed + Time.Metronome.Offset / speed;
                    MidiOut.SchedulePercussion(
                        MIDIPercussionSettings.GetPercussionEnum(BeatType.Subdivision),
                        MIDIPercussionSettings.GetPercussionAttack(BeatType.Subdivision),
                        beatTime
                   );
                }
            }

            public static void Schedule()
            {
                if (_playbackState != PlaybackState.Playing) return;

                if (_beat == null) return;
                //if (Metronome.mute) return;

                var b = ScheduledBeats.PeekOrDefault();
                while (b != null)
                {
                    if (b.time <= Time.time)
                    {
                        ScheduledBeats.Dequeue();
                        b = ScheduledBeats.PeekOrDefault();
                    }
                    else { break; }
                }

                b = _beat.nextBeat;

                var maxScheduledUnits = Mathf.Clamp(MIDIPercussionSettings.instance.maxScheduledUnits / (Metronome.Subdivisions + 1), 2, int.MaxValue);

                while (b != null && ScheduledBeats.Count <= maxScheduledUnits)
                {
                    if (ScheduledBeats.Contains(b))
                    {
                        b = b.nextBeat;
                        continue;
                    }

                    var beatTime = Time.Metronome.StartTime + (Time.DSP.DSPTimeToTimeOffset + Time.Metronome.Offset + b.time) / speed;

                    if (b.number == 1)
                    {
                        ScheduledBeats.Enqueue(b);
                        MidiOut.SchedulePercussion(
                            MIDIPercussionSettings.GetPercussionEnum(BeatType.Heavy),
                            MIDIPercussionSettings.GetPercussionAttack(BeatType.Heavy),
                            beatTime,
                            true
                            );
                    }
                    else
                    {
                        ScheduledBeats.Enqueue(b);
                        MidiOut.SchedulePercussion(
                            MIDIPercussionSettings.GetPercussionEnum(BeatType.Light),
                            MIDIPercussionSettings.GetPercussionAttack(BeatType.Light),
                            beatTime,
                            true
                        );
                    }

                    for (var i = 0; i < Metronome.Subdivisions; i++)
                    {
                        beatTime += b.totalTime / (Metronome.Subdivisions + 1) / speed;
                        MidiOut.SchedulePercussion(
                            MIDIPercussionSettings.GetPercussionEnum(BeatType.Subdivision),
                            MIDIPercussionSettings.GetPercussionAttack(BeatType.Subdivision),
                            beatTime,
                            true
                       );
                    }

                    b = b.nextBeat;
                }
            }
        }
    }
}
