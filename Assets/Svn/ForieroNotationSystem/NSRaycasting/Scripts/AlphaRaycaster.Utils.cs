/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public partial class AlphaRaycaster : GraphicRaycaster
{
    private static bool hasHandler = false;
    private static Dictionary<Font, FontCacheInfo> _fontCache = new Dictionary<Font, FontCacheInfo>();
    private static Dictionary<TMP_FontAsset, FontCacheInfo> _tmp_fontCache = new Dictionary<TMP_FontAsset, FontCacheInfo>();

    private FontCacheInfo GetFontTexture(Font font, float scale, Material fontMaterial)
    {
        if (_fontCache.ContainsKey(font))
        {
            var result = _fontCache[font];
            if (result.scale == scale)
            {
                return result;
            }
        }

        var info = RebuildFontInfo(fontMaterial, scale);
        _fontCache[font] = info;

        if (!hasHandler)
        {
            hasHandler = true;
            Font.textureRebuilt += onTextureRebuilt;
        }

        return info;
    }

    private void onTextureRebuilt(Font font)
    {
        if (_fontCache.ContainsKey(font))
        {
            _fontCache.Remove(font);
        }
    }

    private FontCacheInfo GetFontTexture(TMP_FontAsset font, Material fontMaterial)
    {
        if (_tmp_fontCache.ContainsKey(font))
        {
            return _tmp_fontCache[font];
        }

        var info = RebuildFontInfo(fontMaterial, 1.0f);
        _tmp_fontCache[font] = info;
        return info;
    }

    private FontCacheInfo RebuildFontInfo(Material fontMaterial, float scale)
    {
        var fontTexture = fontMaterial.mainTexture;
        var info = new FontCacheInfo();
        info.width = fontTexture.width;
        info.height = fontTexture.height;
        info.scale = scale;

        var targetTex = new Texture2D(fontTexture.width, fontTexture.height);

        // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(fontTexture.width, fontTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(fontTexture, tmp/*, textSolverMaterial*/);

        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;

        // Copy the pixels from the RenderTexture to the new Texture
        targetTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        targetTex.Apply();

        // Reset the active RenderTexture
        RenderTexture.active = null;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

        info.pixels = targetTex.GetPixels();

        Destroy(targetTex);

        return info;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }

    public static Vector2 Convert(Vector2 p, RectTransform rt, Canvas canvas, bool scaleWithFactor)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        if (scaleWithFactor)
        {
            Vector3 center = Vector3.zero;

            for (int i = 0; i < 4; i++)
            {
                center += corners[i];
            }

            center *= 0.25f;

            float scale = canvas.scaleFactor;

            for (int i = 0; i < 4; i++)
            {
                corners[i] -= center;
                corners[i] *= scale;
                corners[i] += center;
                //corners[i] -= new Vector3(0.5f, 0.5f, 0);
                //corners[i] = new Vector3(corners[i].x * Screen.width, corners[i].y * Screen.height, 0);
            }

            p.x *= scale;
            p.y *= scale;
        }

        var min = Vector3.Min(corners[0], Vector3.Min(corners[1], Vector3.Min(corners[2], corners[3])));
        var max = Vector3.Max(corners[0], Vector3.Max(corners[1], Vector3.Max(corners[2], corners[3])));

        //min *= canvas.scaleFactor;
        //max *= canvas.scaleFactor;

        //var result = new Vector2(Mathf.Lerp(min.x, max.x, rt.pivot.x), Mathf.Lerp(min.y, max.y, rt.pivot.y));

        var result = new Vector2(Mathf.Lerp(min.x, max.x, rt.pivot.x) + p.x, Mathf.Lerp(min.y, max.y, rt.pivot.y) + p.y);
        //var result = new Vector2(min.x + p.x, min.y + p.y);

        //result += new Vector2(Screen.width * rt.anchorMin.x, Screen.height * rt.anchorMin.y);

        return result;
    }


    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public float Distance(Point other)
        {
            float dx = (this.X - other.X);
            dx *= dx;
            float dy = (this.Y - other.Y);
            dy *= dy;
            return Mathf.Sqrt(dx + dy);
        }

        public Vector2 ToVector()
        {
            return new Vector2(X, Y);
        }
    }

    private static bool BitmapApproximateHit(FontCacheInfo fontInfo, int w, int h, int x1, int y1, int x2, int y2, int tx, int ty, float threshold)
    {
        var points = new List<Point>();
        var pixels = fontInfo.pixels;
        for (int j = y1; j <= y2; j++)
        {
            for (int i = x1; i <= x2; i++)
            {
                int ofs = i + j * fontInfo.width;
                var pixel = pixels[ofs];
                if (pixel.a >= threshold)
                {

                    int up, down, left, right;

                    if (j > y1)
                    {
                        up = ofs - fontInfo.width;
                    }
                    else
                    {
                        up = ofs;
                    }

                    if (j < y2)
                    {
                        down = ofs + fontInfo.width;
                    }
                    else
                    {
                        down = ofs;
                    }

                    if (i > x1)
                    {
                        left = ofs - 1;
                    }
                    else
                    {
                        left = ofs;
                    }

                    if (i < x2)
                    {
                        right = ofs + 1;
                    }
                    else
                    {
                        right = ofs;
                    }

                    if (pixels[ofs].a.Equals(pixels[up].a) && pixels[ofs].a.Equals(pixels[left].a) && pixels[ofs].a.Equals(pixels[right].a) && pixels[ofs].a.Equals(pixels[down].a))
                    {
                        continue;
                    }

                    points.Add(new Point(i, j));
                }
            }
        }

        var hull = ComputeConvexHull(points);
        if (hull == null)
        {
            return false;
        }

        Vector2 midPoint = Vector2.zero;
        for (int i = 0; i < hull.Count; i++)
        {
            midPoint += hull[i].ToVector();
        }
        float s = 1.0f / hull.Count;
        midPoint *= s;


        tx += x1;
        ty += y1;
        var p = new Vector2(tx, ty);
        for (int i = 0; i < hull.Count; i++)
        {
            int k = i == hull.Count - 1 ? 0 : i + 1;

            if (PointInTriangle(p, hull[i].ToVector(), hull[k].ToVector(), midPoint))
            {
                return true;
            }

        }


        // TODO 
        return false;
    }

    private static bool BitmapDetectHit(FontCacheInfo fontInfo, int w, int h, int x1, int y1, int x2, int y2, int tx, int ty, float threshold, bool doFlood)
    {
        //doFlood = false;
        bool hit = false;
        if (w > 0 && h > 0)
        {
            var swapColor = Color.cyan;

            // to optimize /cache this later!!
            var pixels = new Color[w * h];
            for (int j = y1; j <= y2; j++)
            {
                for (int i = x1; i <= x2; i++)
                {
                    int ofs = i + j * fontInfo.width;
                    var pixel = fontInfo.pixels[ofs];

                    if (pixel.a >= threshold)
                    {
                        pixel = Color.white;
                    }
                    else
                    {
                        pixel = Color.clear;
                    }

                    int nx = i - x1;
                    int ny = j - y1;
                    pixels[nx + ny * w] = pixel;
                }
            }

            if (doFlood)
            {
                FloodFill(pixels, w, h, 0, 0, (w - 1), (h - 1), new Point(0, 0), swapColor, threshold);
                FloodFill(pixels, w, h, 0, 0, (w - 1), (h - 1), new Point((w - 1), 0), swapColor, threshold);
                FloodFill(pixels, w, h, 0, 0, (w - 1), (h - 1), new Point((w - 1), (h - 1)), swapColor, threshold);
                FloodFill(pixels, w, h, 0, 0, (w - 1), (h - 1), new Point(0, (h - 1)), swapColor, threshold);
                threshold = 1;
            }

            if (tx>=0 && ty>=0 && tx<w && ty<h)
            {
                hit = pixels[tx + ty * w].a >= threshold;
            }
            else
            {
                hit = false;
            }

#if DEVELOPMENT_BUILD
            var temp = new Texture2D(w, h);
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    int ofs = i + j * w;
                    var pixel = pixels[ofs];

                    if (doFlood)
                    {
                        if (pixel.Equals(swapColor))
                        {
                            pixel = Color.clear;
                        }
                        else
                        {
                            pixel = Color.white;
                        }
                    }

                    temp.SetPixel(i, j, pixel);

                    if (i == tx && j == ty)
                    {                        
                        temp.SetPixel(i, j, Color.red);
                    }

                }
            }
            temp.Apply();

            RaycastingTest.detectedTexture = temp;
#endif
        }

        return hit;
    }

    private static void FloodFill(Color[] pixels, int width, int height, int x1, int y1, int x2, int y2, Point pt, Color replacementColor, float threshold)
    {
        var q = new Queue<Point>();
        q.Enqueue(pt);
        while (q.Count > 0)
        {
            var n = q.Dequeue();
            if (pixels[n.X + n.Y * width].a >= threshold)
            {
                continue;
            }

            Point w = n;
            var e = new Point(n.X + 1, n.Y);

            while ((w.X >= x1) && pixels[w.X + w.Y * width].a < threshold)
            {
                pixels[w.X + w.Y * width] = replacementColor;

                if ((w.Y > y1) && pixels[w.X + (w.Y - 1) * width].a < threshold)
                    q.Enqueue(new Point(w.X, w.Y - 1));

                if ((w.Y < y2) && pixels[w.X + (w.Y + 1) * width].a < threshold)
                    q.Enqueue(new Point(w.X, w.Y + 1));

                w.X--;
            }

            while ((e.X <= x2) && pixels[e.X + e.Y * width].a < threshold)
            {
                pixels[e.X + e.Y * width] = replacementColor;

                if ((e.Y > y1) && pixels[e.X + (e.Y - 1) * width].a < threshold)
                    q.Enqueue(new Point(e.X, e.Y - 1));

                if ((e.Y < y2) && pixels[e.X + (e.Y + 1) * width].a < threshold)
                    q.Enqueue(new Point(e.X, e.Y + 1));

                e.X++;
            }
        }
    }


    public static float TriangleArea2D(Point A, Point B, Point C)
    {
        return (A.X * B.Y + B.X * C.Y + C.X * A.Y - A.X * C.Y - B.X * A.Y - C.X * B.Y) * 0.5f;
    }

    public static List<Point> ComputeConvexHull(List<Point> points)
    {
        bool Found;

        // remove duplicate vertices
        int I = 0;
        while (I < points.Count)
        {
            Found = false;
            for (int J = 0; J < points.Count; J++)
            {
                if (I != J && points[J].Distance(points[I]) <= 0.1f)
                {
                    Found = true;
                    break;
                }
            }

            if (Found)
            {
                points.RemoveAt(I);
            }
            else
            {
                I++;
            }

        }

        if (points.Count < 3)
        {
            return null;
        }

        if (points.Count == 3)
        {
            return points;
        }

        //    pointOnHull = leftmost point in S
        int pointOnHull = 0;
        float Min = points[0].X;
        for (I = 1; I < points.Count; I++)
            if (points[I].X < Min)
            {

                Min = points[I].X;
                pointOnHull = I;
            }

        var result = new List<Point>();
        int firstPoint = pointOnHull;
        do
        {
            result.Add(points[pointOnHull]); //   P[i] = pointOnHull
            Found = false;
            for (I = 0; I < points.Count; I++)
                if (I != pointOnHull)
                {


                    bool AllOnLeft = true;
                    for (int J = 0; J < points.Count; J++)
                        if (J != I && J != pointOnHull)
                        {

                            if (TriangleArea2D(points[pointOnHull], points[I], points[J]) < 0)
                            {
                                AllOnLeft = false;
                                break;
                            }
                        }

                    if (AllOnLeft)
                    {
                        pointOnHull = I;
                        Found = true;
                        break;
                    }
                }

        } while (pointOnHull != firstPoint);
        return result;
    }

}
