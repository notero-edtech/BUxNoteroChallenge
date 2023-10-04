/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public struct TimeSignatureStruct
    {
        public int numerator;
        public int denominator;

        public TimeSignatureStruct(int numerator, int denominator)
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }

        public void Log() => Debug.LogFormat("{0}/{1}", numerator, denominator);
        public double ToQuarterTempo(double tempo) => tempo * 4.0 / (double)denominator;
        
        public NSRest.Options GetRestOptions()
        {
            NSRest.Options options = new NSRest.Options();

            switch (denominator)
            {
                case 4:
                    {
                        switch (numerator)
                        {
                            case 4: options.restEnum = RestEnum.Whole; break;
                            case 3: options.restEnum = RestEnum.Half; options.dotsCount = 1; break;
                            case 2: options.restEnum = RestEnum.Half; break;
                            case 1: options.restEnum = RestEnum.Quarter; break;
                            default: Debug.LogError("Not implemented"); break;
                        }
                        break;
                    }

                case 8:
                    {
                        switch (numerator)
                        {
                            case 8: options.restEnum = RestEnum.Whole; break;
                            case 6: options.restEnum = RestEnum.Half; options.dotsCount = 1; break;
                            case 4: options.restEnum = RestEnum.Half; break;
                            case 3: options.restEnum = RestEnum.Quarter; options.dotsCount = 1; break;
                            case 2: options.restEnum = RestEnum.Quarter; break;
                            case 1: options.restEnum = RestEnum.Item8th; break;
                            default: Debug.LogError("Not implemented"); break;
                        }
                        break;
                    }
            }

            return options;
        }
    }
}
