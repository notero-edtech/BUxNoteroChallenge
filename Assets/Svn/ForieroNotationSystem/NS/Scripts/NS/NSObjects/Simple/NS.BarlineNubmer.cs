/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{

    public class NSBarLineNumber : NSObjectText
    {
        public class Options : INSObjectOptions<Options>
        {
            public int number = 0;

            public void Reset()
            {
                number = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                number = o.number;
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

            text.SetText(options.number.ToString());
        }
    }
}
