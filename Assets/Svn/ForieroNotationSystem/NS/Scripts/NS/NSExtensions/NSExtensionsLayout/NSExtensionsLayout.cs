/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsLayout
    {
        public static float BeamSlant(float y1, float y2, float lineSize)
        {
            float slant = 0;

            var intervalDistance = Mathf.RoundToInt(y1.Distance(y2) / lineSize / 2f) + 1;

            if (intervalDistance <= 1)
            {
                slant = 0;
            }
            else if (intervalDistance == 2)
            {
                slant = lineSize / 4f;
            }
            else if (intervalDistance == 3)
            {
                slant = lineSize / 2f;
            }
            else if (intervalDistance >= 4 && intervalDistance < 8)
            {
                slant = lineSize;
            }
            else
            {
                slant = lineSize * 2f;
            }

            return slant * (y2 > y1 ? 1f : -1f);
        }
    }
}
