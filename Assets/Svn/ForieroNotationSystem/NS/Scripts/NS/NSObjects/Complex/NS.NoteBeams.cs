/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSNoteBeams : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public FlagEnum flagEnum = FlagEnum.Undefined;
            public BeamEnum beamEnum = BeamEnum.Undefined;
            public StemEnum stemEnum = StemEnum.Undefined;

            public void Reset()
            {
                flagEnum = FlagEnum.Undefined;
                beamEnum = BeamEnum.Undefined;
                stemEnum = StemEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                flagEnum = o.flagEnum;
                beamEnum = o.beamEnum;
                stemEnum = o.stemEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public List<NSNoteBeam> beams = new List<NSNoteBeam>();

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            beams = new List<NSNoteBeam>();
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            NSNoteBeam beam = null;

            for (int i = 0; i < (int)options.flagEnum; i++)
            {
                beam = this.AddObject<NSNoteBeam>();
                beam.Commit();
                beams.Add(beam);
            }

            Update();
        }

        public void Update()
        {
            for (int i = 0; i < beams.Count; i++)
            {
                var beam = beams[i];
                beam.AlignToParent(true, true);
                switch (options.stemEnum)
                {
                    case StemEnum.Up:
                        beam.PixelShiftY(-ns.BeamVerticalDistance * i, true);
                        beam.verticalDirection = VerticalDirectionEnum.Down;
                        break;
                    case StemEnum.Down:
                        beam.PixelShiftY(ns.BeamVerticalDistance * i, true);
                        beam.verticalDirection = VerticalDirectionEnum.Up;
                        break;
                }
            }
        }
    }
}
