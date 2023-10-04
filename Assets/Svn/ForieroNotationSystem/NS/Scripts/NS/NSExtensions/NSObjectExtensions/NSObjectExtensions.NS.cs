/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSObjectExtensions
    {
        public static NSPart AddPart(this NS ns, NSPart.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = ns.AddObject<NSPart>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }
    }
}
