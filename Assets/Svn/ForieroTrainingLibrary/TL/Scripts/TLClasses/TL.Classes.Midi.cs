/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.Training.Classes
{
    public struct MidiStruct
    {
        public enum TighEnum
        {
            Begin,
            End,
            Undefined = int.MaxValue
        }

        public enum TupletEnum
        {
            Begin,
            Continue,
            End,
            Undefined = int.MaxValue
        }

        public TL.Enums.NoteAndRestFlags duration;
        public int[] midis;
        public bool isRest;
        public TighEnum tighEnum;
        public TupletEnum tupletEnum;
        public int tupletValue;

        public static MidiStruct Rest(TL.Enums.NoteAndRestFlags duration)
        {
            var result = new MidiStruct();
            result.midis = new int[] { 0 };
            result.isRest = true;
            result.tighEnum = TighEnum.Undefined;
            result.duration = duration;
            return result;
        }

        public static MidiStruct Note(int pitch, TL.Enums.NoteAndRestFlags duration)
        {
            var result = new MidiStruct();
            result.midis = new int[] { pitch };
            result.isRest = false;
            result.tighEnum = TighEnum.Undefined;
            result.duration = duration;
            return result;
        }

        public static MidiStruct Chord(int[] pitches, TL.Enums.NoteAndRestFlags duration)
        {
            var result = new MidiStruct();
            result.midis = pitches;
            result.isRest = false;
            result.tighEnum = TighEnum.Undefined;
            result.duration = duration;
            return result;
        }

        public override string ToString()
        {
            return duration.ToString() + " "+tupletEnum.ToString()+ " tie="+tighEnum.ToString();
        }
    }
}


