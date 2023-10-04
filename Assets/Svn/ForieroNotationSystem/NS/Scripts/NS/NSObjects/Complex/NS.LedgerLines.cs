/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;


namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSLedgerLines : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public int countAbove = 0;
            public int countBelow = 0;

            public void Reset()
            {
                countAbove = 0;
                countBelow = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                countAbove = o.countAbove;
                countBelow = o.countBelow;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public List<NSLedgerLine> ledgerLinesAbove = new List<NSLedgerLine>();
        public List<NSLedgerLine> ledgerLinesBelow = new List<NSLedgerLine>();

        public override void Reset()
        {
            DestroyChildren();

            ledgerLinesAbove = new List<NSLedgerLine>();
            ledgerLinesBelow = new List<NSLedgerLine>();

            base.Reset();
            options.Reset();
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            ledgerLinesAbove = new List<NSLedgerLine>();
            ledgerLinesBelow = new List<NSLedgerLine>();

            for (int i = 0; i < options.countAbove; i++)
            {
                NSLedgerLine ledgerLine = this.AddObject<NSLedgerLine>(pool, pivot);
                // moving to the edge of the line staff system //
                ledgerLine.Shift(DirectionEnum.Up, true, (int)stave.options.staveEnum - 1, ShiftStepEnum.Half);
                // moving indexed line //
                ledgerLine.Shift(DirectionEnum.Up, true, i + 1);
                ledgerLine.Commit();
            }

            for (int i = 0; i < options.countBelow; i++)
            {
                NSLedgerLine ledgerLine = this.AddObject<NSLedgerLine>(pool, pivot);
                // moving to the edge of the line staff system //
                ledgerLine.Shift(DirectionEnum.Down, true, (int)stave.options.staveEnum - 1, ShiftStepEnum.Half);
                // moving indexed line //
                ledgerLine.Shift(DirectionEnum.Down, true, i + 1);
                ledgerLine.Commit();
            }

            this.SendVisuallyBack(null, true);
        }
    }
}
