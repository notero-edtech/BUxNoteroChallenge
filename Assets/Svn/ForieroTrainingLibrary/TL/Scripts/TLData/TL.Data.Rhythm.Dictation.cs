/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Core.Classes.Rhythms;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Rhythm
            {
                public static partial class Dictation
                {
                    public class Data : Rhythm.Data
                    {
                        public List<RhythmItem> rhythm;
                        public List<RhythmItem> rhythmInput = new List<RhythmItem>();
                    }

                    public static Data data = new Data();
                }
            }
        }
    }
}
