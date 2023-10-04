/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public class NSRhythmSystemTest : MonoBehaviour
    {
        public NSBehaviour nsBehaviour;

        public void OnTest1()
        {
            (nsBehaviour.ns as NSRhythmSystem).LoadTLData(Test1());
        }

        TL.Exercises.Data Test1()
        {
            var result = new TL.Exercises.Data();

            for (int i=0; i<5; i++)
            {
                var measure = new TL.Exercises.Data.Measure();

                switch (i)
                {
                    case 0:
                        {
                            measure.notes.Add(Training.Classes.MidiStruct.Note(34, TL.Enums.NoteAndRestFlags.Whole));
                            break;
                        }

                    case 1:
                        {
                            measure.notes.Add(Training.Classes.MidiStruct.Note(30, TL.Enums.NoteAndRestFlags.Half));
                            measure.notes.Add(Training.Classes.MidiStruct.Note(30, TL.Enums.NoteAndRestFlags.Quarter));

                            var temp = Training.Classes.MidiStruct.Note(32, TL.Enums.NoteAndRestFlags.Quarter);
                            temp.tighEnum = Training.Classes.MidiStruct.TighEnum.Begin;
                            measure.notes.Add(temp);

                            break;
                        }

                    case 2:
                        {
                            var temp = Training.Classes.MidiStruct.Note(32, TL.Enums.NoteAndRestFlags.Item8th);
                            temp.tighEnum = Training.Classes.MidiStruct.TighEnum.End;
                            measure.notes.Add(temp);

                            measure.notes.Add(Training.Classes.MidiStruct.Note(30, TL.Enums.NoteAndRestFlags.Half));

                            measure.notes.Add(Training.Classes.MidiStruct.Rest(TL.Enums.NoteAndRestFlags.Item8th));
                            measure.notes.Add(Training.Classes.MidiStruct.Note(30, TL.Enums.NoteAndRestFlags.Quarter));
                            break;
                        }

                    case 4:
                        {
                            measure.notes.Add(Training.Classes.MidiStruct.Note(30, TL.Enums.NoteAndRestFlags.Half));

                            var temp = Training.Classes.MidiStruct.Note(32, TL.Enums.NoteAndRestFlags.Item8th);
                            temp.tupletEnum = Training.Classes.MidiStruct.TupletEnum.Begin;
                            temp.tupletValue = 3;
                            measure.notes.Add(temp);

                            temp = Training.Classes.MidiStruct.Note(32, TL.Enums.NoteAndRestFlags.Item8th);
                            temp.tupletEnum = Training.Classes.MidiStruct.TupletEnum.Continue;
                            temp.tupletValue = 3;
                            measure.notes.Add(temp);

                            temp = Training.Classes.MidiStruct.Note(32, TL.Enums.NoteAndRestFlags.Item8th);
                            temp.tupletEnum = Training.Classes.MidiStruct.TupletEnum.End;
                            temp.tupletValue = 3;
                            measure.notes.Add(temp);

                            measure.notes.Add(Training.Classes.MidiStruct.Chord(new int[] { 30, 32, 34 }, TL.Enums.NoteAndRestFlags.Quarter));
                            break;
                        }

                    default:
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                measure.notes.Add(Training.Classes.MidiStruct.Note(30, TL.Enums.NoteAndRestFlags.Quarter));
                            }
                            break;
                        }
                }

                result.measures.Add(measure);
            }

            return result;
            /*TL.Exercises.Rhythm.Imitation.Generate();
            return TL.Exercises.Rhythm.Imitation.data;*/
        }

    }
}
