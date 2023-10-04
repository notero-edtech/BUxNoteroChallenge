/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSRest : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public RestEnum restEnum = RestEnum.Undefined;
            public int dotsCount = 0;

            public void Reset()
            {
                restEnum = RestEnum.Undefined;
                dotsCount = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                restEnum = o.restEnum;
                dotsCount = o.dotsCount;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        NSDot dots = null;

        public override void Reset()
        {
            base.Reset();
            options.Reset();
        }

        public override void Commit()
        {
            base.Commit();

            if (options.restEnum == RestEnum.Undefined) return;
            
            text.SetText(options.restEnum.ToSMuFL());

            if (options.dotsCount > 0)
            {
                dots = this.AddObject<NSDot>();
                dots.options.count = options.dotsCount;
                dots.Commit();
                Align(dots);
            }

            if (options.restEnum == RestEnum.Whole && stave.IsNotNull())
            {
                if (stave.options.staveEnum == StaveEnum.Five)
                {
                    this.PixelShiftY(ns.LineSize, true);
                }
            }
            
            this.SetVisible(NSDisplaySettings.Rests);
        }

        public virtual void Align(NSDot dots)
        {
            dots.AlignTo(this, true, true);

            switch (options.restEnum)
            {
                default:
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Half);
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Quarter);
                    break;
            }

            if (stave && dots.IsOnStaveLine()) dots.Shift(DirectionEnum.Up, true, 1, ShiftStepEnum.Half);
        }
    }
}

