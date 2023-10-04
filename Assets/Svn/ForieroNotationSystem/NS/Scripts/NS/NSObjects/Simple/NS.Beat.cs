/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Classes
{

    public class NSBeat : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public int beat = 0;

            public void Reset()
            {
                beat = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                beat = o.beat;
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
        }
    }
}
