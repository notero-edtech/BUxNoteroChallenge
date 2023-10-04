/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Extensions;
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSTimeSignature : NSObject, INSColorable
    {
        public class Options : INSObjectOptions<Options>
        {
            public TimeSignatureEnum timeSignatureEnum = TimeSignatureEnum.Undefined;
            public TimeSignatureStruct timeSignatureStruct = new TimeSignatureStruct(4, 4);
            public bool changing = false;

            public void Reset()
            {
                timeSignatureEnum = TimeSignatureEnum.Undefined;
                timeSignatureStruct = new TimeSignatureStruct(4, 4);
                changing = false;
            }

            public void CopyValuesFrom(Options o)
            {
                timeSignatureEnum = o.timeSignatureEnum;
                timeSignatureStruct = o.timeSignatureStruct;
                changing = o.changing;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public NSObjectSMuFL numeratorText = null;
        public NSObjectSMuFL denominatorText = null;

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            numeratorText = null;
            denominatorText = null;

            int _numerator_numerator = (int)(options.timeSignatureStruct.numerator / 10);
            int _numerator_number = options.timeSignatureStruct.numerator % 10;

            int _denominator_numerator = (int)(options.timeSignatureStruct.denominator / 10);
            int _denominator_number = options.timeSignatureStruct.denominator % 10;

            string t = "";

            switch (options.timeSignatureEnum)
            {
                case TimeSignatureEnum.Normal:
                case TimeSignatureEnum.Note:
                case TimeSignatureEnum.DottedNote:


                    if (numeratorText == null)
                    {
                        numeratorText = this.AddObject<NSObjectSMuFL>(pool, pivot,  "Numerator");
                        if (options.changing) numeratorText.Shift(DirectionEnum.Up, true, 3, ShiftStepEnum.Quarter);
                        else numeratorText.Shift(DirectionEnum.Up, true, 1);
                    }

                    t = (_numerator_numerator > 0 ? ((TimeSignatures)((int)TimeSignatures.TimeSig0 + _numerator_numerator)).ToCharString() : "") + ((TimeSignatures)((int)TimeSignatures.TimeSig0 + _numerator_number)).ToCharString();
                    numeratorText.text.SetText(t);
                    numeratorText.Commit();

                    if (denominatorText == null)
                    {
                        denominatorText = this.AddObject<NSObjectSMuFL>(pool, pivot,"Denominator");
                        if (options.changing) denominatorText.Shift(DirectionEnum.Down, true, 3, ShiftStepEnum.Quarter);
                        else denominatorText.Shift(DirectionEnum.Down, true, 1);
                    }

                    t = (_denominator_numerator > 0 ? ((TimeSignatures)((int)TimeSignatures.TimeSig0 + _denominator_numerator)).ToCharString() : "") + ((TimeSignatures)((int)TimeSignatures.TimeSig0 + _denominator_number)).ToCharString();
                    denominatorText.text.SetText(t);
                    denominatorText.Commit();

                    break;
                case TimeSignatureEnum.Cut:
                case TimeSignatureEnum.Common:
                    if (numeratorText == null)
                    {
                        numeratorText = this.AddObject<NSObjectSMuFL>(pool, pivot, "Numerator");
                    }

                    numeratorText.text.SetText(options.timeSignatureEnum.ToSMuFL());
                    numeratorText.Commit();
                    break;
                case TimeSignatureEnum.SingleNumber:
                    if (numeratorText == null)
                    {
                        numeratorText = this.AddObject<NSObjectSMuFL>(pool, pivot, "Numerator");
                    }

                    t = (_numerator_numerator > 0 ? ((TimeSignatures)((int)TimeSignatures.TimeSig0 + _numerator_numerator)).ToCharString() : "") + ((TimeSignatures)((int)TimeSignatures.TimeSig0 + _numerator_number)).ToCharString();

                    numeratorText.text.SetText(t);
                    numeratorText.Commit();
                    break;
            }

            if (options.changing)
            {
                if (numeratorText) numeratorText.SetScale(2f / 3f);
                if (denominatorText) denominatorText.SetScale(2f / 3f);
            }
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
        }

        #region INSColorable implementation

        public void SetColor(Color color) => this.color = color;
        public void SetAlpha(float alpha) => this.color = this.color.A(alpha);
        public Color GetColor() => this.color;
        
        #endregion
    }
}
