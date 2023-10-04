/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public partial class AlphaRaycaster : GraphicRaycaster
{

    // Helper struct for alpha checking text.
    private struct CharacterRect
    {
        public Rect Rect;
        public Text Text;
        public UIVertex UpperLeftVertex;
        public UIVertex UpperRightVertex;
        public UIVertex BottomRightVertex;
        public UIVertex BottomLeftVertex;

        public Rect ScaledRect
        {
            get
            {
                var fontScaleFactor = (float)Text.fontSize / Text.font.fontSize;
                return new Rect(Rect.position * fontScaleFactor, Rect.size * fontScaleFactor);
            }
        }

        public CharacterRect(Text text, UIVertex ulv, UIVertex urv, UIVertex brv, UIVertex blv)
        {
            UpperLeftVertex = ulv;
            UpperRightVertex = urv;
            BottomRightVertex = brv;
            BottomLeftVertex = blv;
            Text = text;
            Rect = new Rect(blv.position.x, blv.position.y,
                Mathf.Abs(ulv.position.x - urv.position.x),
                Mathf.Abs(ulv.position.y - blv.position.y));
        }

        public bool Contains(Vector2 position)
        {
            return ScaledRect.Contains(position);
        }

        public float GetTextureAlphaFromPosition(Vector2 position)
        {
            var normalizedPosition = Rect.PointToNormalized(ScaledRect, position);

            var texture = Text.mainTexture as Texture2D;
            var texCorX = Mathf.Lerp(BottomLeftVertex.uv0.x, BottomRightVertex.uv0.x, normalizedPosition.x) * texture.width;
            var texCorY = Mathf.Lerp(BottomLeftVertex.uv0.y, UpperLeftVertex.uv0.y, normalizedPosition.y) * texture.height;

            return texture.GetPixel((int)texCorX, (int)texCorY).a;
        }
    }

    // Return true if alpha check for text is positive (need to exclude the result).
    private bool AlphaCheckText(GameObject obj, Text objText, AlphaRaycasterCheck objAlphaCheck, Vector2 pointerPos)
    {
        var objTrs = obj.transform as RectTransform;
        Vector2 pointerLPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(objTrs, pointerPos, eventCamera, out pointerLPos))
            return true;

        var characterRects = new List<CharacterRect>();
        for (int i = 0; i < objText.cachedTextGenerator.verts.Count; i += 4)
        {
            characterRects.Add(new CharacterRect(objText,
                objText.cachedTextGenerator.verts[i],
                objText.cachedTextGenerator.verts[i + 1],
                objText.cachedTextGenerator.verts[i + 2],
                objText.cachedTextGenerator.verts[i + 3]
            ));
        }


        var alpha = -1f;
        foreach (var charRect in characterRects)
        {
            if (charRect.Contains(pointerLPos))
            {
                alpha = charRect.GetTextureAlphaFromPosition(pointerLPos);
                break;
            }
        }
        if (alpha == -1) return true;

        if (objAlphaCheck)
        {
            if (objAlphaCheck.includeMaterialAlpha) alpha *= objText.color.a;
            if (alpha < objAlphaCheck.alphaThreshold) return true;
        }
        else
        {
            if (IncludeMaterialAlpha) alpha *= objText.color.a;
            if (alpha < AlphaThreshold) return true;
        }

        return false;
    }
}
