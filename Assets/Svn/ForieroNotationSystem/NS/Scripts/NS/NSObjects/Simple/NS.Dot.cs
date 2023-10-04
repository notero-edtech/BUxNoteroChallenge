/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSDot : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public int count = 0;

            public void Reset()
            {
                count = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                count = o.count;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            string dots = "";
            for (int i = 0; i < options.count; i++)
            {
                dots += IndividualNotes.AugmentationDot.ToCharString();
            }

            text.SetText(dots);
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
            text.SetAlignment(TextAnchor.MiddleLeft);
        }
    }
}
