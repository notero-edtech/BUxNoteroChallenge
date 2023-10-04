/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSBracket : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public float height = 0;
            public HorizontalDirectionEnum direction = HorizontalDirectionEnum.Right;

            public void Reset()
            {
                height = 0;
                direction = HorizontalDirectionEnum.Right;
            }

            public void CopyValuesFrom(Options o)
            {
                height = o.height;
                direction = o.direction;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            top = null;
            bottom = null;

            Shift(DirectionEnum.Down, true, 2);
        }

        NSObjectSMuFL top;
        NSObjectSMuFL bottom;

        public override void Commit()
        {
            DestroyChildren();

            base.Commit();

            vector.vectorEnum = VectorEnum.LineVertical;
            vector.lineVertical.options.thickness = ns.SystemBracketWidth;
            vector.lineVertical.options.sidesBlur = ns.StemWidth / 2;
            vector.lineVertical.options.endsBlur = ns.StemWidth / 2;
            vector.lineVertical.options.length = options.height;
            vector.Commit();

            ApplyHeight();
        }

        public void ApplyHeight()
        {
            vector.lineVertical.options.length = options.height;
            DestroyChildren();

            top = this.AddObject<NSObjectSMuFL>( PoolEnum.NS_PARENT);
            top.AlignToParent(true, false);
            top.Commit();
            switch (options.direction)
            {
                case HorizontalDirectionEnum.Right:
                    top.text.SetAlignment(TextAnchor.MiddleLeft);
                    top.text.SetText(SMuFL.Ranges.StaffBracketsAndDividers.BracketTop.ToCharString());
                    top.PixelShift(new Vector2(-ns.SystemBracketWidth + ns.StemWidth / 2f, options.height - 1f), false);
                    break;
                case HorizontalDirectionEnum.Left:
                    top.text.SetAlignment(TextAnchor.MiddleRight);
                    top.text.SetText(SMuFL.Ranges.StaffBracketsAndDividers.ReversedBracketTop.ToCharString());
                    top.PixelShift(new Vector2(ns.SystemBracketWidth - ns.StemWidth / 2f, options.height - 1f), false);
                    break;
            }

            bottom = this.AddObject<NSObjectSMuFL>( PoolEnum.NS_PARENT);
            bottom.AlignToParent(true, false);
            bottom.Commit();
            switch (options.direction)
            {
                case HorizontalDirectionEnum.Right:
                    bottom.text.SetAlignment(TextAnchor.MiddleLeft);
                    bottom.text.SetText(SMuFL.Ranges.StaffBracketsAndDividers.BracketBottom.ToCharString());
                    bottom.PixelShift(new Vector2(-ns.SystemBracketWidth + ns.StemWidth / 2f, 1f), false);
                    break;
                case HorizontalDirectionEnum.Left:
                    bottom.text.SetAlignment(TextAnchor.MiddleRight);
                    bottom.text.SetText(SMuFL.Ranges.StaffBracketsAndDividers.ReversedBracketBottom.ToCharString());
                    bottom.PixelShift(new Vector2(ns.SystemBracketWidth - ns.StemWidth / 2f, 1f), false);
                    break;

            }
        }
    }
}
