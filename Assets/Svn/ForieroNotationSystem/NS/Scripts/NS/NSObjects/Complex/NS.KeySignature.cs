/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;


namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSKeySignature : NSObject, INSColorable
    {
        public class Options : INSObjectOptions<Options>
        {
            public KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;
            public KeyModeEnum keyModeEnum = KeyModeEnum.Undefined;
            public bool changing = false;

            public void Reset()
            {
                keySignatureEnum = KeySignatureEnum.Undefined;
                keyModeEnum = KeyModeEnum.Undefined;
                changing = false;
            }

            public void CopyValuesFrom(Options o)
            {
                keySignatureEnum = o.keySignatureEnum;
                keyModeEnum = o.keyModeEnum;
                changing = o.changing;
            }
        }

        public readonly Options options = new();
        public static readonly Options _options = new();

        public List<NSAccidental> accidentals = new();

        private NSAccidental _accidental;
        private float _scale = 1;

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            SetPositionY(0, true, true);

            _scale = options.changing ? 2f / 3f : 1f;

            AddKeySignature();

            if (stave != null && stave.clef != null)
            {
                Update(stave.clef.options.clefEnum);
            }
        }

        public void Update(ClefEnum clefEnum)
        {
            SetPositionY(0, true, true);
            PixelShift(
                 new Vector2(
                     0,
                     (int)options.keySignatureEnum >= 0 ? clefEnum.ToKeySignatureSharpsIndex() * ns.LineHalfSize : clefEnum.ToKeySignatureFlatsIndex() * ns.LineHalfSize
                 ), true
             );
        }

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();
        }

        void AddKeySignature()
        {
            switch (options.keySignatureEnum)
            {
                case KeySignatureEnum.CFlatMaj:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    AddAcc(1, 3, AccidentalEnum.Flat);
                    AddAcc(2, -1, AccidentalEnum.Flat);
                    AddAcc(3, 2, AccidentalEnum.Flat);
                    AddAcc(4, -2, AccidentalEnum.Flat);
                    AddAcc(5, 1, AccidentalEnum.Flat);
                    AddAcc(6, -3, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.GFlatMaj_EFlatMin:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    AddAcc(1, 3, AccidentalEnum.Flat);
                    AddAcc(2, -1, AccidentalEnum.Flat);
                    AddAcc(3, 2, AccidentalEnum.Flat);
                    AddAcc(4, -2, AccidentalEnum.Flat);
                    AddAcc(5, 1, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.DFlatMaj_BFlatMin:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    AddAcc(1, 3, AccidentalEnum.Flat);
                    AddAcc(2, -1, AccidentalEnum.Flat);
                    AddAcc(3, 2, AccidentalEnum.Flat);
                    AddAcc(4, -2, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.AFlatMaj_FMin:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    AddAcc(1, 3, AccidentalEnum.Flat);
                    AddAcc(2, -1, AccidentalEnum.Flat);
                    AddAcc(3, 2, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.EFlatMaj_CMin:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    AddAcc(1, 3, AccidentalEnum.Flat);
                    AddAcc(2, -1, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.BFlatMaj_GMin:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    AddAcc(1, 3, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.FMaj_DMin:
                    AddAcc(0, 0, AccidentalEnum.Flat);
                    break;
                case KeySignatureEnum.CMaj_AMin:
                    break;
                case KeySignatureEnum.GMaj_EMin:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    break;
                case KeySignatureEnum.DMaj_BMin:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    AddAcc(1, -3, AccidentalEnum.Sharp);
                    break;
                case KeySignatureEnum.AMaj_FSharpMin:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    AddAcc(1, -3, AccidentalEnum.Sharp);
                    AddAcc(2, 1, AccidentalEnum.Sharp);
                    break;
                case KeySignatureEnum.EMaj_CSharpMin:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    AddAcc(1, -3, AccidentalEnum.Sharp);
                    AddAcc(2, 1, AccidentalEnum.Sharp);
                    AddAcc(3, -2, AccidentalEnum.Sharp);
                    break;
                case KeySignatureEnum.BMaj_GSharpMin:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    AddAcc(1, -3, AccidentalEnum.Sharp);
                    AddAcc(2, 1, AccidentalEnum.Sharp);
                    AddAcc(3, -2, AccidentalEnum.Sharp);
                    AddAcc(4, -5, AccidentalEnum.Sharp);
                    break;
                case KeySignatureEnum.FSharpMaj_DSharpMin:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    AddAcc(1, -3, AccidentalEnum.Sharp);
                    AddAcc(2, 1, AccidentalEnum.Sharp);
                    AddAcc(3, -2, AccidentalEnum.Sharp);
                    AddAcc(4, -5, AccidentalEnum.Sharp);
                    AddAcc(5, -1, AccidentalEnum.Sharp);
                    break;
                case KeySignatureEnum.CSharpMaj:
                    AddAcc(0, 0, AccidentalEnum.Sharp);
                    AddAcc(1, -3, AccidentalEnum.Sharp);
                    AddAcc(2, 1, AccidentalEnum.Sharp);
                    AddAcc(3, -2, AccidentalEnum.Sharp);
                    AddAcc(4, -5, AccidentalEnum.Sharp);
                    AddAcc(5, -1, AccidentalEnum.Sharp);
                    AddAcc(6, -4, AccidentalEnum.Sharp);
                    break;
            }
        }

        void AddAcc(int xShift, int yShift, AccidentalEnum accidentalEnum)
        {
            NSAccidental accidental = AddAccidental(accidentalEnum);
            accidental.Commit();
            accidental.SetScale(_scale, false);
            accidental.PixelShift(new Vector2(xShift * ns.LineSize * _scale, yShift * ns.LineHalfSize - 2f * ns.LineSize), true);
            accidentals.Add(accidental);
        }

        public NSAccidental AddAccidental(AccidentalEnum anAccidental)
        {
            NSAccidental result = this.AddObject<NSAccidental>(pool, pivot);
            result.Commit();
            result.options.accidentalEnum = anAccidental;
            return result;
        }

        #region INSColorable implementation

        public void SetColor(Color color) => this.color = color;
        public void SetAlpha(float alpha) => this.color = this.color.A(alpha);
        public Color GetColor() => this.color;
        
        #endregion
    }
}
