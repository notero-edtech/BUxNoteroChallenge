/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSBarLine : NSObjectSMuFL
    {
        public class Parsing
        {
            public double time = 0;
            public int number = 0;

            public void Reset()
            {
                time = 0;
                number = 0;
            }
        }

        public Parsing parsing = new Parsing();

        public class Options : INSObjectOptions<Options>
        {
            public BarLineEnum barLineEnum = BarLineEnum.Undefined;

            public void Reset()
            {
                barLineEnum = BarLineEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                barLineEnum = o.barLineEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            base.Commit();

            text.SetText(options.barLineEnum.ToSMuFL());
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
            parsing.Reset();

            Shift(DirectionEnum.Down, true, 2);
        }
    }
}
