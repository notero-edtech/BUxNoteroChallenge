/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSAccidental : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public AccidentalEnum accidentalEnum = AccidentalEnum.Undefined;
            public YesNoEnum parenthesisEnum = YesNoEnum.Undefined;

            public void Reset()
            {
                accidentalEnum = AccidentalEnum.Undefined;
                parenthesisEnum = YesNoEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                accidentalEnum = o.accidentalEnum;
                parenthesisEnum = o.parenthesisEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            var s = options.accidentalEnum.ToSMuFL();
            if (options.parenthesisEnum == YesNoEnum.Yes)
            {
                text.SetText(StandardAccidentals12Edo.AccidentalParensLeft.ToCharString() + s + StandardAccidentals12Edo.AccidentalParensRight.ToCharString());
            }
            else
            {
                text.SetText(s);
            }
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            text.SetAlignment(TextAnchor.MiddleRight);
        }
    }
}
