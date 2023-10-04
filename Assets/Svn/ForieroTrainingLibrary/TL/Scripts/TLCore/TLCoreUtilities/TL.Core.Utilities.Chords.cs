/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        static partial class Utilities
        {
            public static partial class Chords
            {
                public static Enums.ToneEnum[] GetTonesFromChord(Enums.Chord.ChordTypeFlags chord, Enums.ToneEnum root, Enums.Chord.ChordInversionTypeFlags inversion = Enums.Chord.ChordInversionTypeFlags.Root)
                {
                    var degrees = Enum.GetValues(typeof(Enums.DegreeFlags)).Cast<Enums.DegreeFlags>().ToArray();
                    var result = new List<Enums.ToneEnum>();
                    result.Add(root);
                    
                    foreach (var degree in degrees)
                    {
                        var temp = (Enums.Chord.ChordTypeFlags)degree;
                        if (chord.HasFlag(temp))
                        {
                            var nextTone = Intervals.GetNextTone(root, (Enums.Interval.IntervalFlags) degree);
                            result.Add(nextTone);
                        }
                    }

                    if (result.Count == 4)
                    {
                        switch (inversion)
                        {
                            case Enums.Chord.ChordInversionTypeFlags.First:
                                {
                                    var third = result[1];
                                    var fifth = result[2];
                                    var seven = result[3];

                                    result[0] = third;
                                    result[1] = fifth;
                                    result[2] = seven;
                                    result[3] = root;
                                    break;
                                }

                            case Enums.Chord.ChordInversionTypeFlags.Second:
                                {
                                    var third = result[1];
                                    var fifth = result[2];
                                    var seven = result[3];

                                    result[0] = fifth;
                                    result[1] = seven;
                                    result[2] = root;
                                    result[3] = third;
                                    break;
                                }

                            case Enums.Chord.ChordInversionTypeFlags.Third:
                                {
                                    var third = result[1];
                                    var fifth = result[2];
                                    var seven = result[3];

                                    result[0] = seven;
                                    result[1] = root;
                                    result[2] = third;
                                    result[3] = fifth;
                                    break;
                                }
                        }

                    }
                    else
                    {
                        switch (inversion)
                        {
                            case Enums.Chord.ChordInversionTypeFlags.First:
                                {
                                    var third = result[1];
                                    var fifth = result[2];

                                    result[0] = third;
                                    result[1] = fifth;
                                    result[2] = root;
                                    break;
                                }

                            case Enums.Chord.ChordInversionTypeFlags.Second:
                                {
                                    var third = result[1];
                                    var fifth = result[2];

                                    result[0] = fifth;
                                    result[1] = root;
                                    result[2] = third;
                                    break;
                                }
                        }

                    }

                    return result.ToArray();

                    /*
                    Enums.ToneEnum[] result;

                    switch (chord)
                    {
                        case Enums.Chord.ChordTypeFlags.Major:
                            {
                                result = new Enums.ToneEnum[3];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.First:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[2] = root;
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.Second:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[1] = root;
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            break;
                                        }

                                    default: return null;

                                }
                                break;
                            }



                        case Enums.Chord.ChordTypeFlags.Major7:
                            {
                                result = new Enums.ToneEnum[4];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major7th);
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.First:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major7th);
                                            result[3] = root;
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.Second:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major7th);
                                            result[2] = root;
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.Third:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major7th);
                                            result[1] = root;
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            break;
                                        }

                                    default: return null;
                                }

                                break;
                            }


                        case Enums.Chord.ChordTypeFlags.Minor:
                            {
                                result = new Enums.ToneEnum[3];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.First:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor3rd);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[2] = root;
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.Second:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[1] = root;
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor3rd);
                                            break;
                                        }

                                    default: return null;
                                }
                                break;
                            }

                        case Enums.Chord.ChordTypeFlags.Minor7:
                            {
                                result = new Enums.ToneEnum[4];

                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor7th);
                                            break;
                                        }

                                    default: return null;
                                }

                                break;
                            }

                        case Enums.Chord.ChordTypeFlags.Diminished:
                            {
                                result = new Enums.ToneEnum[3];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Diminished5th);
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.First:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor3rd);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Diminished5th);
                                            result[2] = root;
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.Second:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Diminished5th);
                                            result[1] = root;
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor3rd);
                                            break;
                                        }

                                    default: return null;
                                }
                                break;
                            }

                        case Enums.Chord.ChordTypeFlags.Minor7b:
                            {
                                result = new Enums.ToneEnum[4];

                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor7th);
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.First:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor7th);
                                            result[3] = root;
                                            break;
                                        }

                                    case Enums.Chord.ChordInversionTypeFlags.Second:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor7th);
                                            result[2] = root;
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            break;
                                        }


                                    case Enums.Chord.ChordInversionTypeFlags.Third:
                                        {
                                            result[0] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor7th);
                                            result[1] = root;
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[3] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            break;
                                        }

                                    default: return null;
                                }

                                break;
                            }


                        case Enums.Chord.ChordTypeFlags.Augmented:
                            {
                                result = new Enums.ToneEnum[3];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major3rd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Minor6th); // TODO check if this is correct
                                            break;
                                        }
                                    default: return null;
                                }

                                break;
                            }

                        case Enums.Chord.ChordTypeFlags.Sus4:
                            {
                                result = new Enums.ToneEnum[3];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect4th);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            break;
                                        }
                                    default: return null;
                                }

                                break;
                            }

                        case Enums.Chord.ChordTypeFlags.Sus2:
                            {
                                result = new Enums.ToneEnum[3];
                                switch (inversion)
                                {
                                    case Enums.Chord.ChordInversionTypeFlags.Root:
                                        {
                                            result[0] = root;
                                            result[1] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Major2nd);
                                            result[2] = Intervals.GetNextTone(root, Enums.Interval.IntervalFlags.Perfect5th);
                                            break;
                                        }
                                    default: return null;
                                }

                                break;
                            }

                        default: return null;
                    }

                    return result;*/
                }
            }
        }
    }
}
