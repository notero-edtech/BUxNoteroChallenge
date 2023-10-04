/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ForieroEngine.Music.NotationSystem.Raycasting
{
    public static class AlphaRaycasterExtensions
    {
        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }

        public static Rect GetBestFitRect(this TextMeshProUGUI text)
        {
            var rt = text.GetComponent<RectTransform>();
            var canvas = text.canvas;

            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);


            foreach (var cInfo in text.textInfo.characterInfo)
            {
                Vector2 bottomLeft = AlphaRaycaster.Convert(cInfo.bottomLeft, rt, canvas, true);
                Vector2 topLeft = AlphaRaycaster.Convert(cInfo.topLeft, rt, canvas, true);
                Vector2 topRight = AlphaRaycaster.Convert(cInfo.topRight, rt, canvas, true);
                Vector2 bottomRight = AlphaRaycaster.Convert(cInfo.bottomRight, rt, canvas, true);

                min = Vector2.Min(min, bottomLeft);
                min = Vector2.Min(min, bottomRight);
                min = Vector2.Min(min, topLeft);
                min = Vector2.Min(min, topRight);

                max = Vector2.Max(max, bottomLeft);
                max = Vector2.Max(max, bottomRight);
                max = Vector2.Max(max, topLeft);
                max = Vector2.Max(max, topRight);
            }

            return new Rect(min.x / canvas.scaleFactor, min.y / canvas.scaleFactor, (max.x - min.x) / canvas.scaleFactor, (max.y - min.y) / canvas.scaleFactor);
        }

        public static Rect GetBestFitRect(this Text text)
        {
            var rt = text.GetComponent<RectTransform>();
            var canvas = text.canvas;

            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);




            var gen = text.cachedTextGenerator;
            for (int charIndex = 0; charIndex < gen.characters.Count; charIndex++)
            {
                int vofs = charIndex * 4;
                var p0 = gen.verts[vofs + 0];
                var p1 = gen.verts[vofs + 1];
                var p2 = gen.verts[vofs + 2];
                var p3 = gen.verts[vofs + 3];
                Vector3 topLeft = AlphaRaycaster.Convert(p0.position, rt, canvas, false);
                Vector3 topRight = AlphaRaycaster.Convert(p1.position, rt, canvas, false);
                Vector3 bottomRight = AlphaRaycaster.Convert(p2.position, rt, canvas, false);
                Vector3 bottomLeft = AlphaRaycaster.Convert(p3.position, rt, canvas, false);
                min = Vector2.Min(min, bottomLeft);
                min = Vector2.Min(min, bottomRight);
                min = Vector2.Min(min, topLeft);
                min = Vector2.Min(min, topRight);

                max = Vector2.Max(max, bottomLeft);
                max = Vector2.Max(max, bottomRight);
                max = Vector2.Max(max, topLeft);
                max = Vector2.Max(max, topRight);
                //break;
            }

            return new Rect(min.x / canvas.scaleFactor, min.y / canvas.scaleFactor, (max.x - min.x) / canvas.scaleFactor, (max.y - min.y) / canvas.scaleFactor);
        }

        public static void SetBestFitRect(this RectTransform rt, Rect rect)
        {
            //rt.anchoredPosition = new Vector2(rect.min.x + rect.size.x * rt.anchorMin.x, rect.min.y + rect.size.y * rt.anchorMin.y);
            rt.SetSize(new Vector2(rect.width, rect.height));
        }

        public static void SetBestFitRect(this Text text)
        {
            Rect rect = text.GetBestFitRect();

            (text.transform as RectTransform).SetBestFitRect(rect);
        }

        public static void SetBestFitRect(this TextMeshProUGUI text)
        {
            Rect rect = text.GetBestFitRect();

            (text.transform as RectTransform).SetBestFitRect(rect);
        }
    }
}

