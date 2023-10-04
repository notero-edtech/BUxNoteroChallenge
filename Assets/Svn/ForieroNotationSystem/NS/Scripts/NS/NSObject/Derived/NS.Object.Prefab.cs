/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using PathologicalGames;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSObjectPrefab : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public float relativeFontSize = 1f;

            public void Reset()
            {
                relativeFontSize = 1f;
            }

            public void CopyValuesFrom(Options o)
            {
                relativeFontSize = o.relativeFontSize;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            base.Commit();
            this.rectTransform.localScale = Vector3.one *  options.relativeFontSize * NSSettingsStatic.smuflFontSize;
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
        }
    }
}
