/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class NSPart : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public string id = "1";
            public int index = 0;
            public int groupIndex = 0;
            public bool bracket = false;
            public float lineDistance = 11;

            public void Reset()
            {
                id = "1";
                index = 0;
                groupIndex = 0;
                bracket = false;
                lineDistance = 11;
            }

            public void CopyValuesFrom(Options o)
            {
                id = o.id;
                index = o.index;
                groupIndex = o.groupIndex;
                bracket = o.bracket;
                lineDistance = o.lineDistance;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public class Parsing
        {
            public List<NSStave> staves = new List<NSStave>();
            public NSMetronomeMark.Options metronomeMark = new NSMetronomeMark.Options() { beatsPerMinute = 120, dots = 0, noteEnum = NoteEnum.Quarter };
        }

        public Parsing parsing = new Parsing();

        public override void Reset()
        {
            base.Reset();
            options.Reset();
        }

        public override void Commit()
        {
            base.Commit();
        }
    }
}

