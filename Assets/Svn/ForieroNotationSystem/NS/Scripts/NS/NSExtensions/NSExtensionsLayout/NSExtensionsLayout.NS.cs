/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsLayout
    {
        public static void ArrangePartsVerticalDistances(this NS ns, bool centerFirstPart = true)
        {
            var parts = ns.GetObjectsOfType<NSPart>(false).OrderBy(x => x.options.id).ToList();

            if (parts == null || parts.Count() == 0) return;

            var y = 0f;
            for (var i = 0; i < parts.Count(); i++)
            {
                var part = parts[i];

                var staves = part.GetObjectsOfType<NSStave>(false).OrderBy(x => x.options.id).ToList();

                if (staves == null || staves.Count == 0) return;

                part.AlignToParent(true, true);
                if (i == 0 && centerFirstPart)
                {
                    y += Mathf.Abs((staves.First().GetPositionY(true) - staves.Last().GetPositionY(true)) / 2f) + staves.First().topEdge;
                }
                else if (i > 0)
                {
                    y -= part.options.lineDistance * part.ns.LineSize;
                }

                part.SetPositionY(y, true, true);

                y -= (-staves.Last().GetPositionY(true) - staves.Last().topEdge);
            }
        }
    }
}
