/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Core.Classes.Rhythms;
using System;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Rhythm
            {
                public static partial class Imitation
                {
                    public class Data : Rhythm.Data
                    {
                        public enum RhythmCaptureModeEnum
                        {
                            None,
                            Tap,
                            Midi
                        }

                        public List<RhythmItem> rhythm;

                        public bool isRecordingInput;

                        public List<RhythmEvent> questionEvents = new List<RhythmEvent>();
                        public List<RhythmEvent> rhythmInput = new List<RhythmEvent>();

                        public RhythmCaptureModeEnum rhythmCaptureMode = RhythmCaptureModeEnum.None;
                    }

                    public static Data data = new Data();
                }
            }
        }
    }
}
