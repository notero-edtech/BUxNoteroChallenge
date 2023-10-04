/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Classes;
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
                public static partial class Identification
                {
                    static partial class CoreIdentification
                    {

                        public static void Generate()
                        {
                            var data = Identification.data;
                            data.chord = Utilities.RandomEnumFromFlags<TL.Enums.Chord.ChordTypeFlags>(Chord.settings.identificationSettings.chordTypeFlags);
                            var root = Utilities.RandomEnum<Enums.ToneEnum>();
                            data.tones = Utilities.Chords.GetTonesFromChord(data.chord, root);

                            data.pitches = new int[data.tones.Length];
                            var pitchVal = Exercises.settings.pitchRange.low;

                            for (int i = 0; i < data.pitches.Length; i++)
                            {
                                var currentTone = data.tones[i];
                                while (Utilities.Midi.FromIndex(pitchVal) != currentTone)
                                {
                                    pitchVal++;
                                }
                                data.pitches[i] = pitchVal;
                            }

                            //var midi = MidiStruct.Chord(data.pitches, Enums.NoteAndRestFlags.Quarter);
                        }
                    }
                }
            }
        }
    }
}

