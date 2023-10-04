/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSObjectExtensions
    {
        public static NSTie AddTie(this NSPart part, NSTie.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = part.AddObject<NSTie>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }
    }
}
