/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.Training.Core.Classes.Rhythms
{
    public struct RhythmEvent
    {
        public double startTime;
        public double endTime;
        public bool isRest;
    }

    public struct RhythmItem
    {
        public TL.Enums.NoteAndRestFlags duration;
        public bool isRest;
        public MidiStruct.TighEnum tie;

        public RhythmItem(TL.Enums.NoteAndRestFlags duration, bool isRest, MidiStruct.TighEnum tie = MidiStruct.TighEnum.Undefined)
        {
            this.duration = duration;
            this.isRest = isRest;
            this.tie = tie;
        }
    }
}

