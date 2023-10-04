/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public partial class AlphaRaycaster : GraphicRaycaster
{
    public class FontCacheInfo
    {
        public float scale;
        public int width;
        public int height;
        public Color[] pixels;
    }

    //public RawImage rawImage;

    //private Material textSolverMaterial;

    private bool AlphaCheckTextMeshPro(GameObject obj, TextMeshProUGUI textMeshPro, AlphaRaycasterCheck objAlphaCheck, Vector2 pointerPos, OuterEdgeDetection edgeDetection, float threshold)
    {
        threshold = (0.5f - threshold);

        if (edgeDetection == OuterEdgeDetection.NativeRect)
        {
            return false;
        }

        /*if (!Input.GetMouseButtonDown(0))
        {
            return;
        }*/


        var fontInfo = GetFontTexture(textMeshPro.font, textMeshPro.fontMaterial);

        //rawImage.enabled = false;

        //var mouse = pointerPos;
        var mouse = Input.mousePosition;
        //var mouse = new Vector2(pointerPos.x, Screen.height - pointerPos.y);

        var rt = textMeshPro.GetComponent<RectTransform>();
        //var rect = RectTransformUtility.PixelAdjustRect(rt, textMeshPro.canvas);

        var t = this.transform;
        var cam = Camera.main;

        var canvas = textMeshPro.canvas;

        //var scaler = canvas.GetComponent<CanvasScaler>();

        foreach (var cInfo in textMeshPro.textInfo.characterInfo)
        {
            Vector3 bottomLeft = Convert(cInfo.bottomLeft, rt, canvas, true);
            Vector3 topLeft = Convert(cInfo.topLeft, rt, canvas, true);
            Vector3 topRight = Convert(cInfo.topRight, rt, canvas, true);
            Vector3 bottomRight = Convert(cInfo.bottomRight, rt, canvas, true);

            /*            bottomLeft = cam.WorldToScreenPoint(bottomLeft);
						topLeft = cam.WorldToScreenPoint(topLeft);
						bottomRight = cam.WorldToScreenPoint(bottomRight);
						topRight = cam.WorldToScreenPoint(topRight);*/

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

                float u1 = Mathf.Min(Mathf.Min(cInfo.vertex_BL.uv.x, cInfo.vertex_BR.uv.x), Mathf.Min(cInfo.vertex_TL.uv.x, cInfo.vertex_TR.uv.x));
                float v1 = Mathf.Min(Mathf.Min(cInfo.vertex_BL.uv.y, cInfo.vertex_BR.uv.y), Mathf.Min(cInfo.vertex_TL.uv.y, cInfo.vertex_TR.uv.y));

                float u2 = Mathf.Max(Mathf.Max(cInfo.vertex_BL.uv.x, cInfo.vertex_BR.uv.x), Mathf.Max(cInfo.vertex_TL.uv.x, cInfo.vertex_TR.uv.x));
                float v2 = Mathf.Max(Mathf.Max(cInfo.vertex_BL.uv.y, cInfo.vertex_BR.uv.y), Mathf.Max(cInfo.vertex_TL.uv.y, cInfo.vertex_TR.uv.y));

                var uv = new Rect();
                uv.xMin = u1;
                uv.xMax = u2;
                uv.yMin = v1;
                uv.yMax = v2;
                //rawImage.uvRect = uv;

                int x1 = (int)(u1 * fontInfo.width);
                int y1 = (int)(v1 * fontInfo.height);
                int x2 = (int)(u2 * fontInfo.width);
                int y2 = (int)(v2 * fontInfo.height);

                int w = x2 - x1 + 1;
                int h = y2 - y1 + 1;

                int tx = (int)(w * (mouse.x - minX) / (float)(maxX - minX));
                int ty = (int)(h * (mouse.y - minY) / (float)(maxY - minY));

                /*bool hit = false;

                var temp = new Texture2D(w, h);
                for (int j = y1; j <= y2; j++)
                {
                    for (int i = x1; i <= x2; i++)
                    {
                        int ofs = i + j * fontTexture.width;
                        var pixel = pixels[ofs];

                        int nx = i - x1;
                        int ny = j - y1;
                        temp.SetPixel(nx, ny, pixel);

                        if (nx == tx && ny == ty)
                        {
                            hit = pixel.a >= 1;
                            temp.SetPixel(nx, ny, Color.red);
                        }
                    }
                }
                temp.Apply();

                var bytes = temp.EncodeToPNG();
                File.WriteAllBytes("output2.png", bytes);*/

                bool hit;

                /*if (edgeDetection == OuterEdgeDetection.Approximate)
                {
                    hit = BitmapApproximateHit(fontInfo, w, h, x1, y1, x2, y2, tx, ty, threshold);
                }
                else*/

                /*tx = (int)(tx * canvas.scaleFactor);
                ty = (int)(ty * canvas.scaleFactor);*/
                hit = BitmapDetectHit(fontInfo, w, h, x1, y1, x2, y2, tx, ty, threshold, edgeDetection == OuterEdgeDetection.Fill);

                /*if (edgeDetection == OuterEdgeDetection.Fill)
                {
                    hit = BitmapDetectHit(fontInfo, w, h, x1, y1, x2, y2, tx, ty, threshold, true);
                }
                else
                {
                    hit = (fontInfo.pixels[(x1 + tx) + (y1 + ty) * fontInfo.width].a >= threshold);
                }*/



                if (hit)
                {
                    //Debug.Log("Hit: " + cInfo.character);
                    //rawImage.enabled = true;                    
                    return false;
                }

                return true;
            }
        }

        return true;
        //Debug.Log("Notihng found");
    }
}
