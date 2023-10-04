/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public partial class AlphaRaycaster : GraphicRaycaster
{

    //public RawImage rawImg;

    private bool AlphaCheckDynamicText(GameObject obj, Text objText, AlphaRaycasterCheck objAlphaCheck, Vector2 pointerPos, OuterEdgeDetection edgeDetection, float threshold)
    {
        threshold = (1.0f - threshold);

        if (edgeDetection == OuterEdgeDetection.NativeRect)
        {
            return false;
        }

        var mouse = Input.mousePosition;
        //var mouse = new Vector2(pointerPos.x, Screen.height - pointerPos.y);

        var rt = objText.GetComponent<RectTransform>();

        var t = this.transform;
        var cam = Camera.main;

        var canvas = objText.canvas;

        var scaler = canvas.GetComponent<CanvasScaler>();

        var fontInfo = GetFontTexture(objText.font, scaler.scaleFactor, objText.font.material);

        //  rawImg.texture = fontTexture;

        var gen = objText.cachedTextGenerator;
        for (int charIndex = 0; charIndex < gen.characters.Count; charIndex++)
        {
            int vofs = charIndex * 4;
            var p0 = gen.verts[vofs + 0];
            var p1 = gen.verts[vofs + 1];
            var p2 = gen.verts[vofs + 2];
            var p3 = gen.verts[vofs + 3];
            Vector3 topLeft = Convert(p0.position, rt, canvas, false);
            Vector3 topRight = Convert(p1.position, rt, canvas, false);
            Vector3 bottomRight = Convert(p2.position, rt, canvas, false);
            Vector3 bottomLeft = Convert(p3.position, rt, canvas, false);

            var minX = Mathf.Min(Mathf.Min(bottomLeft.x, bottomRight.x), Mathf.Min(topLeft.x, topRight.x));
            var minY = Mathf.Min(Mathf.Min(bottomLeft.y, bottomRight.y), Mathf.Min(topLeft.y, topRight.y));
            var maxX = Mathf.Max(Mathf.Max(bottomLeft.x, bottomRight.x), Mathf.Max(topLeft.x, topRight.x));
            var maxY = Mathf.Max(Mathf.Max(bottomLeft.y, bottomRight.y), Mathf.Max(topLeft.y, topRight.y));

            if (mouse.x >= minX && mouse.x <= maxX && mouse.y >= minY && mouse.y <= maxY)
            {
                if (edgeDetection == OuterEdgeDetection.ClosestRect)
                {
                    return false;
                }

                /*
                 * p0 p1
                 * p3 p2
                 * */

                /*float u1 = Mathf.Min(Mathf.Min(p0.uv0.x, p1.uv0.x), Mathf.Min(p2.uv0.x, p3.uv0.x));
                float v1 = Mathf.Min(Mathf.Min(p0.uv0.y, p1.uv0.y), Mathf.Min(p2.uv0.y, p3.uv0.y));

                float u2 = Mathf.Max(Mathf.Max(p0.uv0.x, p1.uv0.x), Mathf.Max(p2.uv0.x, p3.uv0.x));
                float v2 = Mathf.Max(Mathf.Max(p0.uv0.y, p1.uv0.y), Mathf.Max(p2.uv0.y, p3.uv0.y));*/

                float u1 = p0.uv0.x;
                float v1 = p0.uv0.y;

                float u2 = p2.uv0.x;
                float v2 = p2.uv0.y;

                bool rot90 = (p0.uv0.y == p3.uv0.y && p0.uv0.x == p1.uv0.x && p0.uv0.x == Mathf.Max(u1, u2));

                if (u1 > u2)
                {
                    var temp = u1;
                    u1 = u2;
                    u2 = temp;
                }

                if (v1 > v2)
                {
                    var temp = v1;
                    v1 = v2;
                    v2 = temp;
                }

                //rawImg.uvRect = new Rect(u1,  v1, u2-u1, v2-v1);

                int x1 = (int)(u1 * fontInfo.width);
                int y1 = (int)(v1 * fontInfo.height);
                int x2 = (int)(u2 * fontInfo.width);
                int y2 = (int)(v2 * fontInfo.height);

                int w = x2 - x1 + 1;
                int h = y2 - y1 + 1;

                int tx, ty;

                float dx = (mouse.x - minX);
                float dy = (mouse.y - minY);

                dx = (dx / (float)(maxX - minX));
                dy = (dy / (float)(maxY - minY));

                if (rot90)
                {
                    dx = 1 - dx;
                    tx = (int)(w * dy);
                    ty = (int)(h * dx);

                    /*var temp = tx;
                    tx = ty;
                    ty = temp;*/
                    //ty = h - ty;
                }
                else
                {
                    tx = (int)(w * dx);
                    ty = (int)(h * dy);
                    ty = h - ty;
                }

                bool hit;

                /*if (edgeDetection == OuterEdgeDetection.Approximate)
                {
                    hit = BitmapApproximateHit(fontInfo, w, h, x1, y1, x2, y2, tx, ty, threshold);
                }
                else*/


                hit = BitmapDetectHit(fontInfo, w, h, x1, y1, x2, y2, tx, ty, threshold, edgeDetection == OuterEdgeDetection.Fill);
                /*if (edgeDetection == OuterEdgeDetection.Fill)
                {
                    hit = BitmapDetectHit(fontInfo, w, h, x1, y1, x2, y2, tx, ty, threshold, true);
                }
                else
                {
                    hit = fontInfo.pixels[(x1 + tx) + (y1 + ty) * fontInfo.width].a >= threshold;
                }*/


                if (hit)
                {
                    //Debug.Log("Hit: " + objText.text[charIndex] + "     rot:" + rot90);
                    //Debug.Log("Hit: " + cInfo.character);
                    //rawImage.enabled = true;                    
                    return false;
                }

                return true;
            }
        }

        return true;
    }
}
