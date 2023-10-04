/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSObjectExtensions
    {
        public static NSStave AddStave(this NSPart part, NSStave.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = part.AddObject<NSStave>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSBarLine AddBarLine(this NSPart part, NSBarLine.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = part.AddObject<NSBarLine>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSBarLineVertical AddBarLineVertical(this NSPart part, NSBarLineVertical.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = part.AddObject<NSBarLineVertical>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSBarLineHorizontal AddBarLineHorizontal(this NSPart part, NSBarLineHorizontal.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = part.AddObject<NSBarLineHorizontal>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }
    }
}
