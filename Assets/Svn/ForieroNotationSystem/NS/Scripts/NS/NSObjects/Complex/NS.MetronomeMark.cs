/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSMetronomeMark : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public NoteEnum noteEnum = NoteEnum.Quarter;
            public float beatsPerMinute = 120;
            public int dots = 0;

            public void Reset()
            {
                noteEnum = NoteEnum.Quarter;
                beatsPerMinute = 120;
                dots = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                noteEnum = o.noteEnum;
                beatsPerMinute = o.beatsPerMinute;
                dots = o.dots;
            }

            public bool Same(Options o)
            {
                return noteEnum == o.noteEnum && Mathf.Approximately(beatsPerMinute, o.beatsPerMinute) && dots == o.dots;
            }

            public void Log()
            {
                Debug.Log("NoteEnum : " + noteEnum.ToString() + " beatsPerMinute : " + beatsPerMinute.ToString() + " dots : " + dots.ToString());
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        NSObjectText tempoText;

        public float GetTempoPerQuarterNote()
        {
            float dotsCoeficient = options.dots.GetDotsCoefficient();

            var distance = Mathf.Abs(NoteEnum.Quarter - options.noteEnum);

            if (options.noteEnum > NoteEnum.Quarter)
            {
                return options.beatsPerMinute * (distance * dotsCoeficient) * 2f;
            }
            else if (options.noteEnum < NoteEnum.Quarter)
            {
                return options.beatsPerMinute * (distance * dotsCoeficient) / 2f;
            }
            else
            {
                return options.beatsPerMinute * dotsCoeficient;
            }
        }

        public override void Commit()
        {
            base.Commit();

            SetScale(0.8f, false);
            text.SetText(options.noteEnum.ToSMuFL(StemEnum.Up, options.dots));

            if (!tempoText)
            {
                tempoText = this.AddObject<NSObjectText>( pool);
                tempoText.Commit();

                tempoText.text.SetAlignment(TextAnchor.MiddleLeft);
                tempoText.Shift(DirectionEnum.Right, false, scaled: true);
            }

            tempoText.text.SetText("= " + options.beatsPerMinute.ToString());
        }

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();
        }
    }
}
