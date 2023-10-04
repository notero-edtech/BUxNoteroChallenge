/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSCircle : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public void Reset() { }
            public void CopyValuesFrom(Options o) { }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            text.SetText(RoundAndSquareNoteheads.NoteheadRoundBlack.ToCharString());
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
            text.SetAlignment(TextAnchor.MiddleCenter);
        }
    }
}
