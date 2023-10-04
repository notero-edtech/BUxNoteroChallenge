/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Core.Extensions
{
    public static partial class TLCoreChordExtensions
    {
        public static string ToName(this TL.Enums.Chord.ChordTypeFlags chordEnum)
        {
            switch (chordEnum)
            {
                case TL.Enums.Chord.ChordTypeFlags.Major: return "Major";
                case TL.Enums.Chord.ChordTypeFlags.Minor: return "Minor";
                case TL.Enums.Chord.ChordTypeFlags.Diminished: return "Dimimished";
                case TL.Enums.Chord.ChordTypeFlags.Major7: return "Major 7";
                case TL.Enums.Chord.ChordTypeFlags.Minor7b: return "7 Root";
                case TL.Enums.Chord.ChordTypeFlags.Minor7: return "Minor 7";
                case TL.Enums.Chord.ChordTypeFlags.Augmented: return "Augmented";
                case TL.Enums.Chord.ChordTypeFlags.Sus4: return "Sus4";
                case TL.Enums.Chord.ChordTypeFlags.Sus2: return "Sus2";
                default: return "Invalid";
            }
        }
    }
}
