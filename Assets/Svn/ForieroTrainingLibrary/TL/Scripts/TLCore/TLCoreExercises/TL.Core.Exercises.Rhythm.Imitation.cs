/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.Training.Core.Classes.Rhythms;
using ForieroEngine.Music.Training.Classes;

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
                    static partial class CoreImmitation
                    {

                        public static void Generate()
                        {
                            var rhythm = new List<RhythmItem>();

                            /*
                             pick "longest" duration from included list
                             
                             */

                            var rnd = new Random();

                            for (int i=0; i<Exercises.Rhythm.settings.imitationSettings.numberOfMeasures; i++)
                            {
                                int tries = 0;
                                double beatCount = 0;

                                var flags = Exercises.Rhythm.settings.imitationSettings.noteFlags;
                                var list = Utilities.EnumFlagsToList<Enums.NoteAndRestFlags>(flags);
                                var largestIndex = 0;
                                var largestDuration = Utilities.Rhythms.GetNoteRelativeDuration(list[largestIndex]);

                                for (int j=1; j<list.Count; j++)
                                {
                                    var duration = Utilities.Rhythms.GetNoteRelativeDuration(list[j]);
                                    if (duration>largestDuration)
                                    {
                                        largestIndex = j;
                                        largestDuration = duration;
                                    }
                                }

                                var measureItems = new List<RhythmItem>();

                                var currentIndex = largestIndex;
                                while (beatCount<4)
                                {
                                    if (currentIndex < list.Count-1 &&  Utilities.RandomBool())
                                    {
                                        currentIndex++;
                                    }

                                    var duration = Utilities.Rhythms.GetNoteRelativeDuration(list[currentIndex]);

                                    if (beatCount + duration <= 4)
                                    {
                                        beatCount += duration;
                                        measureItems.Add(new RhythmItem(list[currentIndex], false));

                                        //UnityEngine.Debug.Log("added rhytm " + duration);
                                    }

                                    tries++;
                                    if (tries>100)
                                    {
                                        break;
                                    }
                                }

                                TL.Utilities.ShuffleList<RhythmItem>(measureItems, rnd);
                                foreach (var item in measureItems)
                                {
                                    rhythm.Add(item);
                                }
                                
                            }

                            #region GENERATE TIES
                            int n = 0;
                            bool insideTie = false;
                            while (n < rhythm.Count)
                            {
                                if (TL.Utilities.RandomBool(settings.tieNotesPercentage))
                                {
                                    insideTie = !insideTie;
                                    var temp = rhythm[n];
                                    
                                    temp.tie = insideTie ? MidiStruct.TighEnum.Begin : MidiStruct.TighEnum.End;

                                    rhythm[n] = temp;
                                }

                                n++;
                            }

                            if (insideTie)
                            {
                                n = rhythm.Count - 1;
                                var temp = rhythm[n];

                                temp.tie = MidiStruct.TighEnum.End;

                                rhythm[n] = temp;
                            }
                            #endregion

                            data.rhythm = rhythm;
                            data.rhythmInput.Clear();

                            var pitch = Exercises.settings.pitchRange.high;

                            var tupletNotesToInclude = Exercises.Rhythm.settings.imitationSettings.tupletNoteFlags;
                            var tupletRestsToInclude = Exercises.Rhythm.settings.imitationSettings.tupletRestFlags;

                            data.measures.Clear();
                            var measure = new Data.Measure();
                            data.measures.Add(measure);

                            foreach (var item in rhythm)
                            {
                                int tupletValue = 0;

                                if (TL.Utilities.RandomBool())
                                {
                                    var tupletList = new List<int>(4);
                                    //tupletList.Add(0);

                                    Enums.TupletNoteAndRestFlags temp;

                                    temp = (Enums.TupletNoteAndRestFlags)((Enums.DurationFlags)item.duration | Enums.DurationFlags._tuplet3);
                                    if (tupletNotesToInclude.HasFlag(temp))
                                    {
                                        int val = 3;

                                        temp = (Enums.TupletNoteAndRestFlags)((Enums.DurationFlags)item.duration | Enums.DurationFlags._tuplet3);
                                        if (tupletRestsToInclude.HasFlag(temp))
                                        {
                                            val = -val;
                                        }

                                        tupletList.Add(val);
                                    }

                                    temp = (Enums.TupletNoteAndRestFlags)((Enums.DurationFlags)item.duration | Enums.DurationFlags._tuplet5);
                                    if (tupletNotesToInclude.HasFlag(temp))
                                    {
                                        int val = 5;

                                        temp = (Enums.TupletNoteAndRestFlags)((Enums.DurationFlags)item.duration | Enums.DurationFlags._tuplet3);
                                        if (tupletRestsToInclude.HasFlag(temp))
                                        {
                                            val = -val;
                                        }

                                        tupletList.Add(val);
                                    }

                                    temp = (Enums.TupletNoteAndRestFlags)((Enums.DurationFlags)item.duration | Enums.DurationFlags._tuplet7);
                                    if (tupletNotesToInclude.HasFlag(temp))
                                    {
                                        int val = 7;

                                        temp = (Enums.TupletNoteAndRestFlags)((Enums.DurationFlags)item.duration | Enums.DurationFlags._tuplet3);
                                        if (tupletRestsToInclude.HasFlag(temp))
                                        {
                                            val = -val;
                                        }
                                        tupletList.Add(val);
                                    }


                                    if (tupletList.Count > 0)
                                    {
                                        tupletValue = TL.Utilities.RandomItemFromList(tupletList);
                                    }                                    
                                }

                                var allowRests = false;

                                if (tupletValue < 0)
                                {
                                    allowRests = true;
                                    tupletValue = -tupletValue;
                                }

                                if (tupletValue != 0 && item.isRest && !allowRests)
                                {
                                    tupletValue = 0;
                                }

                                switch (tupletValue)
                                {
                                    case 0:
                                        {
                                            var temp = new MidiStruct();
                                            temp.duration = item.duration;
                                            temp.isRest = item.isRest;
                                            temp.tighEnum = item.tie;
                                            temp.tupletEnum = MidiStruct.TupletEnum.Undefined;
                                            temp.midis = new int[] { pitch };
                                            measure.notes.Add(temp);
                                            break;
                                        }

                                    default:
                                        {
                                            var half = Utilities.Rhythms.GetHalfDurationOf(item.duration);

                                            for (int i = 0; i < tupletValue; i++)
                                            {
                                                var temp = new MidiStruct();
                                                temp.tighEnum =  i == 0 ? item.tie : MidiStruct.TighEnum.Undefined;
                                                temp.tupletValue = tupletValue;
                                                temp.tupletEnum = i == 0 ? MidiStruct.TupletEnum.Begin : (i == tupletValue-1 ? MidiStruct.TupletEnum.End: MidiStruct.TupletEnum.Continue);
                                                temp.duration = half;
                                                temp.isRest = item.isRest;
                                                temp.midis = new int[] { pitch };
                                                measure.notes.Add(temp);
                                            }
                                            break;
                                        }
                                }

                            }

                            foreach (var item in data.measures)
                            {
                                UnityEngine.Debug.Log(item);
                            }

                            data.rhythmCaptureMode = Data.RhythmCaptureModeEnum.None;
                            data.isRecordingInput = false;
                        }


                    }
                }
            }
        }
    }
}

