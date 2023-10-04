/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSNoteFlag : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public FlagEnum flagEnum = FlagEnum.Undefined;
            public StemEnum stemEnum = StemEnum.Undefined;

            public void Reset()
            {
                flagEnum = FlagEnum.Undefined;
                stemEnum = StemEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                flagEnum = o.flagEnum;
                stemEnum = o.stemEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            base.Commit();

            if (options.flagEnum == FlagEnum.Undefined) return;

            text.SetText(options.flagEnum.ToSMuFL(options.stemEnum));
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            text.SetAlignment(TextAnchor.MiddleLeft);
        }

        public void Update()
        {
            if (text.GetText() != options.flagEnum.ToSMuFL(options.stemEnum))
            {
                text.SetText(options.flagEnum.ToSMuFL(options.stemEnum));
            }
        }
    }
}
