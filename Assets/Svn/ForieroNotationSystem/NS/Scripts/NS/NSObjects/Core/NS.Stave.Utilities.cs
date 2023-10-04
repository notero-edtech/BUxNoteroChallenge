/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class NSStave : NSObject
    {
        public float Arrange(HorizontalDirectionEnum horizontalDirectionEnum = HorizontalDirectionEnum.Left)
        {
            var direction = horizontalDirectionEnum == HorizontalDirectionEnum.Right ? -1f : 1f;

            var x = (NSSettingsStatic.notationOffset + ns.LineSize) * direction;

            x += (rectWidth / 2f) * direction * (-1);

            if (leftBarLine != null) { x += (2f * ns.LineSize) * direction; }

            x += (ns.LineSize + ns.LineHalfSize) * direction;

            if (metronomeMark)
            {
                metronomeMark.SetPositionX(x, true, true);
                metronomeMark.SetPositionY(this.topEdge + ns.LineSize * 3, true, true);
            }

            if (clef != null && clef.options.clefEnum != ClefEnum.Undefined)
            {
                clef.SetPositionX(x, true, true);
                x += (2f * ns.LineSize + ns.LineHalfSize) * direction;
            }

            if (
                keySignature != null
                && keySignature.options.keySignatureEnum != KeySignatureEnum.Undefined
                && keySignature.options.keySignatureEnum != KeySignatureEnum.CMaj_AMin &&
                keySignature.accidentals.Count > 0
               )
            {
                keySignature.SetPositionX(x, true, true);
                x += Mathf.Abs((int)keySignature.options.keySignatureEnum) * ns.LineSize * direction;
                x += ns.LineHalfSize * direction;
            }
            else { x += (ns.LineHalfSize) * direction; }

            if (timeSignature != null) { timeSignature.SetPositionX(x, true, true); }

            return x;
        }
        
        public void ApplyLedgerLines(NSObject o)
        {
            if (o == null) return;

            var y = o.rectTransform.anchoredPosition.y;
            var top = rectTransform.anchoredPosition.y + ((int)stave.options.staveEnum - 1) * ns.LineHalfSize;
            var bottom = rectTransform.anchoredPosition.y - ((int)stave.options.staveEnum - 1) * ns.LineHalfSize;

            NSLedgerLines lines = null;

            if (ledgerLines.ContainsKey(o.GetPositionX(false)))
            {
                lines = ledgerLines[o.GetPositionX(false)];
            }
            else
            {
                lines = this.AddObject<NSLedgerLines>();
                ledgerLines.Add(o.GetPositionX(false), lines);
                lines.AlignXTo(o, true, false);
                lines.AlignYTo(this, true, false);
            }

            if (y > top + ns.LineHalfSize)
            {
                var countAbove = Mathf.Abs(Mathf.FloorToInt((y - top) / ns.LineSize));
                if (countAbove > lines.options.countAbove)
                {
                    lines.options.countAbove = countAbove;
                    lines.Commit();
                }
            }
            else if (y < bottom - ns.LineHalfSize)
            {
                var countBelow = Mathf.Abs(Mathf.FloorToInt((bottom - y) / ns.LineSize));

                if (countBelow > lines.options.countBelow)
                {
                    lines.options.countBelow = countBelow;
                    lines.Commit();
                }
            }
        }
    }
}
