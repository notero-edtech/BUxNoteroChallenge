/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsLayout
    {
        public static void ArrangeStavesVerticalDistances(this NSPart part)
        {
            var staves = part.GetObjectsOfType<NSStave>(false).OrderBy(x => x.options.id).ToList();

            if (staves == null || staves.Count() == 0) return;

            float y = 0f;
            for (int i = 0; i < staves.Count(); i++)
            {
                NSStave stave = staves[i];
                stave.AlignToParent(true, true);

                y -= stave.topEdge;

                if (i > 0)
                {
                    y -= stave.options.lineDistance * stave.ns.LineSize;
                }

                stave.SetPositionY(y, true, true);

                y -= (-stave.bottomEdge);
            }

            var lb = staves.First().leftSystemBracket;
            var rb = staves.First().rightSystemBracket;

            if (lb != null)
            {
                lb.options.height = Mathf.Abs(staves.First().GetPositionY(true) - staves.Last().GetPositionY(true)) + staves.First().topEdge - staves.Last().bottomEdge;
                lb.SetPositionY(staves.Last().GetPositionY(true), true, true);
                lb.ApplyHeight();
            }

            if (rb != null)
            {
                rb.options.height = Mathf.Abs(staves.First().GetPositionY(true) - staves.Last().GetPositionY(true)) + staves.First().topEdge - staves.Last().bottomEdge;
                rb.SetPositionY(staves.Last().GetPositionY(true), true, true);
                rb.ApplyHeight();
            }
        }
    }
}
