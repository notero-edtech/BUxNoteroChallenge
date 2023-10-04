using System;

namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        public static partial class MC
        {
            public static Action OnStart;
            public static Action OnStop;
            public static Action OnContinue;
            public static Action OnTimeSignature;
            public static Action OnTick;
            public static Action OnBeat;
            public static Action OnBar;

            public static int ticks { get; private set; } = 0;
            public static int bar { get; private set; } = 1;
            public static int beat { get; private set; } = 1;
            public static int timeNumerator { get; private set; } = 4;
            public static int timeDenominator { get; private set; } = 4;

            static long timestamps = DateTime.Now.ToFileTime();
            static double length = 20.833;

            static int total24thTicks = 24;
            static int total16thTicks = 0;

            static double seconds
            {
                get { return BeatsToSeconds(beat); }
                set { ticks = SecondsToBeats(value) * 6; }
            }

            static double bpm
            {
                get { return MsToBpm(length); }
                set { length = BpmToMs(value); }
            }

            public static void Reset()
            {
                ticks = 0;
                bar = 0;
                beat = 0;
                timeNumerator = 4;
                timeDenominator = 4;
                total16thTicks = 0;
                timestamps = DateTime.Now.ToFileTime();
            }

            static double BeatsToSeconds(int beats)
            {
                return (double)beats * length * 0.006; // 6 / 1000.0;
            }

            static int SecondsToBeats(double seconds)
            {
                return (int)((seconds * 1000) / (6 * length));
            }

            static double BpmToMs(double bpm)
            {
                if (bpm == 0) { return 0; }
                return  1000.0 / ((bpm * total24thTicks) / 60.0);
            }

            static double MsToBpm(double ms)
            {
                if (ms == 0) { return 0; }
                return  (1000.0 / ms / total24thTicks) * 60.0;
            }

            public static new string ToString()
            {
                return
                    "Ticks : " + ticks.ToString() + "\n" +
                    "Bar : " + bar.ToString() + "\n" +
                    "TimeSignature : " + timeNumerator.ToString() + "/" + timeDenominator.ToString() + "\n" +
                    "Beat : " + beat.ToString() + "\n" +
                    //"Seconds : " + seconds.ToString() + "\n" +
                    //"BPM : " + bpm.ToString() +
                    "";
            }
        }
    }
}