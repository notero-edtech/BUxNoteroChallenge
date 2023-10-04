/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Classes;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Melody
            {
                public static partial class Imitation
                {
                    static partial class CoreImitation
                    {

                        public static void Generate()
                        {
                            var root = TL.Enums.ToneEnum.C;
                            var chord = TL.Utilities.Chords.GetTonesFromChord(Enums.Chord.ChordTypeFlags.Major, root);

                            var measure = new Data.Measure();
                            data.measures.Add(measure);
                            foreach (var tone in chord)
                            {
                                var pitch = TL.Utilities.Pitches.FromTone(tone);
                                measure.notes.Add(MidiStruct.Note(pitch, Enums.NoteAndRestFlags.Quarter));
                            }

                            var rootPitch = TL.Utilities.Pitches.FromTone(root);
                            measure.notes.Add(MidiStruct.Note(rootPitch, Enums.NoteAndRestFlags.Quarter));

                            data.lastTime = TL.Utilities.GetEndTimeOfMidiStruct(data.measures, data.measures.Count - 1, TL.Exercises.Melody.settings.imitationSettings.BPM);
                        }

                    }
                }
            }
        }
    }
}
