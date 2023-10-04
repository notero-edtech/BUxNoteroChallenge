/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections.Generic;

namespace ForieroEngine.Music.SMuFL
{
    public static partial class Metadata
    {
        static Dictionary<int, GlyphBoundingBox> cachedGlyphBoundingBoxes = new Dictionary<int, GlyphBoundingBox>();

        public static GlyphBoundingBox GetGlyphBoundingBox(int unicode)
        {
            if (cachedGlyphBoundingBoxes.ContainsKey(unicode))
            {
                return cachedGlyphBoundingBoxes[unicode];
            }
            else if (Bravura.glyphBoundingBoxes.ContainsKey(unicode))
            {
                var b = Bravura.glyphBoundingBoxes[unicode];
                cachedGlyphBoundingBoxes.Add(unicode, b);
                return b;
            }
            else
            {
                Debug.LogError("GlyphBoundingBox not found : " + unicode.ToString());
                return new GlyphBoundingBox();
            }
        }

        public struct GlyphBoundingBox
        {
            public int unicode;

            public float topEm;
            public float leftEm;
            public float rightEm;
            public float bottomEm;

            public float widthEm { get { return Mathf.Abs(leftEm - rightEm) / 4f; } }
            public float heightEm { get { return Mathf.Abs(bottomEm - topEm) / 4f; } }
        }

    }
}
