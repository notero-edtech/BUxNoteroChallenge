/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSFermata : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public FermataEnum fermataEnum = FermataEnum.Undefined;
            public OrientationEnum orientationEnum = OrientationEnum.Undefined;

            public void Reset()
            {
                fermataEnum = FermataEnum.Undefined;
                orientationEnum = OrientationEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                fermataEnum = o.fermataEnum;
                orientationEnum = o.orientationEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            text.SetText(options.fermataEnum.ToSMuFL());
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();

        }
    }
}
