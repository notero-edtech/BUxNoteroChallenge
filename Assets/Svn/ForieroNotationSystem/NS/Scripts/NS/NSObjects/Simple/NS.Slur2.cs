/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSSlur2 : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public void Reset()
            {

            }

            public void CopyValuesFrom(Options o)
            {

            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {

        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
            vector.vectorEnum = VectorEnum.Slur1;
        }
    }
}
