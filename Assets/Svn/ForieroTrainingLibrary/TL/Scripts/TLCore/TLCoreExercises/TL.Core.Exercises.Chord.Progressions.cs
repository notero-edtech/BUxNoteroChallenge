/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static partial class Progressions
                {
                    static partial class CoreProgressions
                    {
                        public static void Generate()
                        {

                            int size = 2;

                            data.root = Enums.ToneEnum.C;
                            data.scale = Enums.Scale.ScaleFlags.Major;

                            var scaleTones = Utilities.Scales.GetScaleTones(data.root, data.scale);

                            data.degrees = new Enums.Scale.ScaleDegreeEnum[size];
                            data.pitches = new int[size * 3];

                            int pitchIndex = 0;
                            for (int i = 0; i < data.degrees.Length; i++)
                            {
                                data.degrees[i] = Utilities.RandomEnum<Enums.Scale.ScaleDegreeEnum>();
                                var tones = Utilities.Scales.GetChordTonesFromDegree(data.degrees[i], scaleTones);
                                var pitches = Exercises.settings.pitchRange.GetPitchRangeFromTones(tones);
                                for (int j = 0; j < pitches.Length; j++)
                                {
                                    data.pitches[pitchIndex] = pitches[j];
                                    pitchIndex++;
                                }
                            }

                        }

                        public static void PlayProgression(float toneDuration, float toneGap, Action onFinish = null)
                        {
                            for (int i = 0; i < data.pitches.Length; i++)
                            {
                                int chordIndex = i / 3;
                                TL.Providers.Midi.NoteDispatch(data.pitches[i], Chord.settings.toneDuration, Providers.Midi.ToneGap(Chord.settings.toneGap, chordIndex), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, (i == data.pitches.Length - 1) ? onFinish : null);
                            }
                        }

                    }
                }
            }
        }
    }
}

