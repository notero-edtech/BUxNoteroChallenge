/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using ForieroEngine.Music.Training.Classes;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public class Data
            {
                public class Measure
                {
                    public List<MidiStruct> notes = new List<MidiStruct>();
                }

                public bool showClefs = true;
                public bool showKeySignature = true;

                public int tempo = 60;
                public Enums.KeySignatureEnum keySignature = Enums.KeySignatureEnum.CMaj_AMin;
                public Enums.BeatsPerMeasureFlags beatsPerMeasure = Enums.BeatsPerMeasureFlags._4;
                public Enums.NotePerBeatFlags notesPerBeat = Enums.NotePerBeatFlags._4;

                public List<Measure> measures = new List<Measure>();
                public List<MidiStruct> midiDataAnswer = new List<MidiStruct>();

                public Enums.AnswerEnum answer;

                /*public int GetNumberOfMeasures()
                {
                    return 
                    double beats = 0;
                    foreach (var item in midiDataQuestion)
                    {
                        var duration = TL.Utilities.Rhythms.GetNoteRelativeDuration(item.duration);
                        beats += duration;
                    }

                    return (int)(beats / TL.Utilities.Rhythms.GetNotePerBeatValue(notesPerBeat));
                }*/
            }
        }
    }
}

