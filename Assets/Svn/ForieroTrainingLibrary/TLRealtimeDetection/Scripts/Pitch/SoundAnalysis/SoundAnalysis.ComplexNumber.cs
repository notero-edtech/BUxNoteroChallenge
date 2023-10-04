/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;

namespace ForieroEngine.Music.SoundAnalysis
{
    /// <summary>
    /// Complex number.
    /// </summary>
    struct ComplexNumber
    {
        public float Re;
        public float Im;

        public ComplexNumber(float re)
        {
            this.Re = re;
            this.Im = 0;
        }

        public ComplexNumber(float re, float im)
        {
            this.Re = re;
            this.Im = im;
        }

        public static ComplexNumber operator *(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Re * n2.Re - n1.Im * n2.Im,
                n1.Im * n2.Re + n1.Re * n2.Im);
        }

        public static ComplexNumber operator +(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Re + n2.Re, n1.Im + n2.Im);
        }

        public static ComplexNumber operator -(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Re - n2.Re, n1.Im - n2.Im);
        }

        public static ComplexNumber operator -(ComplexNumber n)
        {
            return new ComplexNumber(-n.Re, -n.Im);
        }

        public static implicit operator ComplexNumber(float n)
        {
            return new ComplexNumber(n, 0);
        }

        public ComplexNumber PoweredE()
        {
            float e = Mathf.Exp(Re);
            return new ComplexNumber(e * Mathf.Cos(Im), e * Mathf.Sin(Im));
        }

        public float Power2()
        {
            return Re * Re - Im * Im;
        }

        public float AbsPower2()
        {
            return Re * Re + Im * Im;
        }

        public override string ToString()
        {
            return String.Format("{0}+i*{1}", Re, Im);
        }
    }
}
