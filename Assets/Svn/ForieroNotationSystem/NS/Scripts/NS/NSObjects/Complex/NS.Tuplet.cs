/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;


namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSTuplet : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public int number = 3;

            public void Reset()
            {
                number = 3;
            }

            public void CopyValuesFrom(Options o)
            {
                number = o.number;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        NSTupletBracket bracket;

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            bracket = null;
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            bracket = AddObject<NSTupletBracket>( pool);
            bracket.options.tupletNumber = options.number;
            bracket.Commit();
        }
    }
}
