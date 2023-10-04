/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSNoteStem : NSObjectVector
    {
        public float size
        {
            get { return Mathf.Abs(vector.lineVertical.options.length); }
            set
            {
                switch (options.stemEnum)
                {
                    case StemEnum.Up:
                        vector.lineVertical.options.length = value;
                        break;
                    case StemEnum.Down:
                        vector.lineVertical.options.length = -value;
                        break;
                }

                Align(noteBeams);
                Align(noteFlag);
            }
        }

        public class Options : INSObjectOptions<Options>
        {
            public float size = 0f;
            public NoteEnum noteEnum = NoteEnum.Undefined;
            public StemEnum stemEnum = StemEnum.Undefined;
            public bool autoStemEnum = true;
            public BeamEnum beamEnum = BeamEnum.Undefined;

            public void Reset()
            {
                size = 0f;
                noteEnum = NoteEnum.Undefined;
                stemEnum = StemEnum.Undefined;
                autoStemEnum = true;
                beamEnum = BeamEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                size = o.size;
                noteEnum = o.noteEnum;
                stemEnum = o.stemEnum;
                autoStemEnum = o.autoStemEnum;
                beamEnum = o.beamEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public NSNoteBeams noteBeams;
        public NSNoteFlag noteFlag;

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            DestroyChildren();

            noteBeams = null;
            noteFlag = null;

            vector.vectorEnum = VectorEnum.LineVertical;
            vector.lineVertical.options.thickness = ns.StemWidth;
            vector.lineVertical.options.sidesBlur = ns.StemBlur;
            vector.lineVertical.options.endsBlur = ns.StemBlur;

            size = ns.LineSize * 3.5f;
        }

        public override void Commit()
        {
            base.Commit();

            if (options.beamEnum != BeamEnum.Undefined)
            {
                noteBeams = AddNoteBeams(options.noteEnum.ToFlagEnum(), options.beamEnum);
            }
            else
            {
                noteFlag = AddNoteFlag(options.noteEnum.ToFlagEnum(), options.stemEnum);
            }
            
            this.SetVisible(NSDisplaySettings.Stems);
        }

        public void Update()
        {
            if (noteFlag)
            {
                noteFlag.options.stemEnum = options.stemEnum;
                noteFlag.Update();
                Align(noteFlag);
            }

            if (noteBeams)
            {
                noteBeams.options.stemEnum = options.stemEnum;
                noteBeams.Update();
                Align(noteBeams);
            }
        }

        public virtual void Align(NSNoteBeams noteBeams)
        {
            if (!noteBeams) return;

            noteBeams.AlignTo(this, true, true);

            switch (options.stemEnum)
            {
                case StemEnum.Up:
                    noteBeams.PixelShiftY(size + ns.StemBlur, true);
                    break;
                case StemEnum.Down:
                    noteBeams.PixelShiftY(-size - ns.StemBlur + ns.LineHalfSize, true);
                    break;
            }
        }

        public virtual void Align(NSNoteFlag noteFlag)
        {
            if (!noteFlag) return;

            noteFlag.AlignTo(this, true, true);

            switch (options.stemEnum)
            {
                case StemEnum.Up:
                    noteFlag.PixelShift(new Vector2(-ns.StemWidth / 2f - ns.StemWidth / 2f / 2f, size), true, true);
                    break;
                case StemEnum.Down:
                    noteFlag.PixelShift(new Vector2(-ns.StemWidth / 2f - ns.StemWidth / 2f / 2f, -size), true, true);
                    break;
            }
        }

        public NSNoteFlag AddNoteFlag(FlagEnum flagEnum, StemEnum stemEnum)
        {
            if (flagEnum == FlagEnum.Undefined) return null;

            noteFlag = this.AddObject<NSNoteFlag>();
            noteFlag.options.stemEnum = stemEnum;
            noteFlag.options.flagEnum = flagEnum;
            noteFlag.Commit();
            Align(noteFlag);
            return noteFlag;
        }

        public NSNoteBeams AddNoteBeams(FlagEnum flagEnum, BeamEnum beamEnum)
        {
            if (flagEnum == FlagEnum.Undefined) return null;

            noteBeams = this.AddObject<NSNoteBeams>();
            noteBeams.options.flagEnum = flagEnum;
            noteBeams.options.beamEnum = beamEnum;
            noteBeams.Commit();
            Align(noteBeams);
            return noteBeams;
        }

        public void PixelSnap(bool includeChildren = true)
        {
            Vector2 diff = rectTransform.anchoredPosition;

            switch (options.stemEnum)
            {
                case StemEnum.Up:
                    rectTransform.AlignAnchoredPositionToPixels(PixelAlignEnum.Floor);
                    diff = rectTransform.anchoredPosition - diff;
                    break;
                case StemEnum.Down:
                    rectTransform.AlignAnchoredPositionToPixels(PixelAlignEnum.Ceil);
                    diff = rectTransform.anchoredPosition - diff;
                    break;
            }

            if (includeChildren)
            {
                foreach (NSObject o in objects)
                {
                    o.PixelShift(diff, true);
                }

                foreach (NSObject o in images)
                {
                    o.PixelShift(diff, true);
                }

                foreach (NSObject o in rawImages)
                {
                    o.PixelShift(diff, true);
                }

                foreach (NSObject o in texts)
                {
                    o.PixelShift(diff, true);
                }

                foreach (NSObject o in vectors)
                {
                    o.PixelShift(diff, true);
                }

                foreach (NSObject o in smufls)
                {
                    o.PixelShift(diff, true);
                }
            }
        }
    }
}
