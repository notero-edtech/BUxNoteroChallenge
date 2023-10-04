/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSObjectExtensions
    {
        public static T IsNotNull<T>(this T o) where T : NSObject => o ?? null;
        public static bool IsNull<T>(this T o) where T : NSObject => o == null;
        
        public static float GetPositionOnStave(this NSObject o, int aStep, int anOctave, NSClef.Options clef = null)
        {
            Assert.IsNotNull(o.ns.IsNotNull());

            if (clef == null)
            {
                if (o.stave && o.stave.clef)
                {
                    return (anOctave * 7 + aStep - o.stave.clef.options.ToBaseStaveIndex()) * o.ns.LineHalfSize;
                }
                else
                {
                    Debug.LogError("GetPositionOnStave() : Object is not on any stave!");
                    return 0;
                }
            }
            else
            {
                return (anOctave * 7 + aStep - clef.ToBaseStaveIndex()) * o.ns.LineHalfSize;
            }
        }

        public static float GetPositionOnStave(this NSObject o, StepEnum aStep, OctaveEnum anOctave, NSClef.Options clef = null)
        {
            return o.GetPositionOnStave((int)aStep, (int)anOctave, clef);
        }

        public static List<T> GetObjectsOfType<T>(this List<T> list, bool children, List<T> foundObjects = null) where T : NSObject
        {
            var l = new List<T>();
            foreach (var o in list)
            {
                l.AddRange(o.GetObjectsOfType<T>(children, foundObjects));
            }
            return l;
        }
    }
}
