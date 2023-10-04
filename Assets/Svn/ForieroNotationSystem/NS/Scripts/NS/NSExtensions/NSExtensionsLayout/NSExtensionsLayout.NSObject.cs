/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsLayout
    {
        public static bool IsOnStaveLine(this NSObject o)
        {
            if (NS.debug) Assert.IsNotNull(o);
            if (NS.debug) Assert.IsNotNull(o.stave);
            if (NS.debug) Assert.IsNotNull(o.ns);

            var y = o.GetPositionY(false) - o.stave.GetPositionY(false);

            float rest = (float)(int)o.stave.options.staveEnum % 2f;
            if (Mathf.Approximately(rest, 0f))
            {
                rest = y % o.ns.LineSize;
                return !Mathf.Approximately(rest, 0f);
            }
            else
            {
                rest = y % o.ns.LineSize;
                return Mathf.Approximately(rest, 0f);
            }
        }

        public static float BeamSlant(this NSObject o1, NSObject o2)
        {
            if (NS.debug) Assert.IsNotNull(o1);
            if (NS.debug) Assert.IsNotNull(o1.stave);
            if (NS.debug) Assert.IsNotNull(o1.ns);
            return BeamSlant(o1.GetPositionY(false), o2.GetPositionY(false), o1.ns.LineSize);
        }

        public static int HalfLinesFromStaveMiddleLine(this NSObject o)
        {
            if (NS.debug) Assert.IsNotNull(o);
            if (NS.debug) Assert.IsNotNull(o.stave);
            if (NS.debug) Assert.IsNotNull(o.ns);
            return Mathf.RoundToInt((o.GetPosition(false).y - o.stave.GetPosition(false).y) / (o.ns.LineHalfSize));
        }

        public static bool IsOnStave(this NSObject o)
        {
            if (NS.debug) Assert.IsNotNull(o);
            if (NS.debug) Assert.IsNotNull(o.stave);
            return o.GetPositionY(false) >= o.stave.GetPosition(false).y + o.stave.bottomEdge && o.GetPositionY(false) <= o.stave.GetPosition(false).y + o.stave.topEdge;
        }

        public static bool IsAboveStave(this NSObject o)
        {
            if (NS.debug) Assert.IsNotNull(o);
            if (NS.debug) Assert.IsNotNull(o.stave);
            return o.GetPositionY(false) > o.stave.GetPosition(false).y + o.stave.topEdge;
        }

        public static bool IsBelowStave(this NSObject o)
        {
            if (NS.debug) Assert.IsNotNull(o);
            if (NS.debug) Assert.IsNotNull(o.stave);
            return o.GetPositionY(false) < o.stave.GetPosition(false).y + o.stave.bottomEdge;
        }

        public static bool IsLowerThan(this NSObject o1, NSObject o2)
        {
            return o1.GetPositionY(false) < o2.GetPositionY(false);
        }

        public static bool IsHigherThan(this NSObject o1, NSObject o2)
        {
            return o1.GetPositionY(false) > o2.GetPositionY(false);
        }

        public static bool IsApproximatelySameY(this NSObject o1, NSObject o2)
        {
            return Mathf.Approximately(o1.GetPositionY(false), o2.GetPositionY(false));
        }
    }
}
