/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;


namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSChord : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public NoteEnum noteEnum = NoteEnum.Undefined;
            public BeamEnum beamEnum = BeamEnum.Undefined;
            public StemEnum stemEnum = StemEnum.Undefined;

            public void Reset()
            {
                noteEnum = NoteEnum.Undefined;
                beamEnum = BeamEnum.Undefined;
                stemEnum = StemEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                noteEnum = o.noteEnum;
                beamEnum = o.beamEnum;
                stemEnum = o.stemEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public List<int> steps = new List<int>();

#pragma warning disable 414
        List<NSObjectSMuFL> heads = new List<NSObjectSMuFL>();
#pragma warning restore 414

        NSNoteStem stem = null;

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            heads = new List<NSObjectSMuFL>();

            stem = this.AddObject<NSNoteStem>();

            var x = 0f; var y = 0f; var lastStep = 0;
            for (var i = 0; i < steps.Count; i++)
            {
                if (i == 0) { lastStep = steps[0]; }
                var step = steps[i];
                var head = this.AddObject<NSObjectSMuFL>();
                head.text.SetText(options.noteEnum.ToSMuFL());

                switch (options.stemEnum)
                {
                    case StemEnum.Up: y = ns.LineHalfSize * step; break;
                    case StemEnum.Down: y = ns.LineHalfSize * step; break;
                }

                head.PixelShift(new Vector2(x, y), true);
                head.Commit();

                lastStep = step;
            }

            stem.Commit();
        }

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            heads = new List<NSObjectSMuFL>();
            steps = new List<int>();
            stem = null;
        }
    }
}
