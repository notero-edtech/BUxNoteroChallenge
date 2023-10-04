/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{

    public class NSClef : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public ClefEnum clefEnum = ClefEnum.Undefined;
            public int staveLine = 0;
            public int octaveChange = 0;
            public bool changing = false;

            public void Reset()
            {
                clefEnum = ClefEnum.Undefined;
                staveLine = 0;
                octaveChange = 0;
                changing = false;
            }

            public void CopyValuesFrom(Options o)
            {
                clefEnum = o.clefEnum;
                staveLine = o.staveLine;
                octaveChange = o.octaveChange;
                changing = o.changing;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            base.Reset();
            options.Reset();
        }

        public override void Commit()
        {
            base.Commit();

            text.SetText(options.clefEnum.ToSMuFL(options.octaveChange));
            if (options.changing || options.clefEnum == ClefEnum.TAB)
            {
                this.SetScale(2f / 3f, true);
            }
            else
            {
                this.SetScale(1, true);
            }

            SetPosition();
        }

        void SetPosition()
        {
            if (parent is NSStave)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, parent.rectTransform.anchoredPosition.y);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
            }

            switch (options.clefEnum)
            {
                case ClefEnum.Undefined:
                    break;
                case ClefEnum.Treble:
                    Shift(DirectionEnum.Down, true, 1);
                    if (options.staveLine != 0)
                    {
                        Shift(DirectionEnum.Down, true, 1);
                        Shift(DirectionEnum.Up, true, options.staveLine - 1);
                    }
                    break;
                case ClefEnum.Bass:
                    Shift(DirectionEnum.Up, true, 1);
                    if (options.staveLine != 0)
                    {
                        Shift(DirectionEnum.Down, true, 3);
                        Shift(DirectionEnum.Up, true, options.staveLine - 1);
                    }
                    break;
                case ClefEnum.Alto:
                    Shift(DirectionEnum.Up, true, 0);
                    break;
                case ClefEnum.Baritone:
                    Shift(DirectionEnum.Up, true, 2);
                    break;
                case ClefEnum.Soprano:
                    Shift(DirectionEnum.Down, true, 2);
                    break;
                case ClefEnum.MezzoSoprano:
                    Shift(DirectionEnum.Down, true, 1);
                    break;
                case ClefEnum.Tenor:
                    Shift(DirectionEnum.Up, true, 1);
                    break;
                case ClefEnum.Percussion:
                    break;
                case ClefEnum.PercussionUnpitched:
                    break;
                case ClefEnum.TAB:
                    break;
                case ClefEnum.C:
                    Shift(DirectionEnum.Down, true, 2);
                    Shift(DirectionEnum.Up, true, options.staveLine - 1);
                    break;
            }

            if (stave.options.staveEnum == StaveEnum.One)
            {

            }
        }
    }
}
