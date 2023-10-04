/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSArticulation : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public ArticulationEnum articulationEnum = ArticulationEnum.Undefined;
            public OrientationEnum orientationEnum = OrientationEnum.Undefined;

            public void Reset()
            {
                articulationEnum = ArticulationEnum.Undefined;
                orientationEnum = OrientationEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                articulationEnum = o.articulationEnum;
                orientationEnum = o.orientationEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            text.SetText(options.articulationEnum.ToSMuFL());
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();

        }
    }
}
