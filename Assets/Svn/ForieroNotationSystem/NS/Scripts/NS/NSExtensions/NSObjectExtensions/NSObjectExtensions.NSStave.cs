/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSObjectExtensions
    {
        public static NSBracket AddBracket(this NSStave stave, NSBracket.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = stave.AddObject<NSBracket>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSTimeSignature AddTimeSignature(this NSStave stave, NSTimeSignature.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = stave.AddObject<NSTimeSignature>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSClef AddClef(this NSStave stave, NSClef.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = stave.AddObject<NSClef>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSKeySignature AddKeySignature(this NSStave stave, NSKeySignature.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = stave.AddObject<NSKeySignature>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSNote AddNote(this NSStave stave, NSNote.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = stave.AddObject<NSNote>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }

        public static NSRest AddRest(this NSStave stave, NSRest.Options options, PivotEnum pivot = PivotEnum.MiddleCenter, PoolEnum pool = PoolEnum.NS_PARENT)
        {
            var o = stave.AddObject<NSRest>(pool, pivot);
            o.options.CopyValuesFrom(options);
            o.Commit();
            return o;
        }
    }
}
