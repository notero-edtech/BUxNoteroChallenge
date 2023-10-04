/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static partial class Inversions
                {
                    public static partial class CoreInversions
                    {

                        public static void Generate()
                        {
                            data.chord = Utilities.RandomEnumFromFlags<Enums.Chord.ChordTypeFlags>(Chord.settings.inversionsSettings.chordTypeFlags);

                            var root = Utilities.RandomEnum<Enums.ToneEnum>();

                            data.inversion = Utilities.RandomEnumFromFlags<Enums.Chord.ChordInversionTypeFlags>(Chord.settings.inversionsSettings.chordInversionTypeFlags);

                            data.tones = Utilities.Chords.GetTonesFromChord(data.chord, root, data.inversion);

                            data.pitches = new int[data.tones.Length];
                            var toneVal = Exercises.settings.pitchRange.low;

                            for (int i = 0; i < data.pitches.Length; i++)
                            {
                                var currentTone = data.tones[i];
                                while (Utilities.Midi.FromIndex(toneVal) != currentTone)
                                {
                                    toneVal++;
                                }
                                data.pitches[i] = toneVal;
                            }
                        }
                    }
                }
            }
        }
    }

}

