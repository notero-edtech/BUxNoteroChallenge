/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.Music.NotationSystem.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSWaveGraph : NSObjectRawImage
    {
        public class Options : INSObjectOptions<Options>
        {
            public float length = 100f;
            public float height = 20f;
            public int maxTextureSize = 2048;
            public float[] samples = new float[0];
            public int channels = 2;
            public int offset = 0;
            public int samplesCount = 0;
            public float rangeMin = 0.5f;
            public float rangeMax = 0.5f;
            public DirectionEnum directionEnum = DirectionEnum.Right;

            [Range(0f, 1f)]
            public float shadowAlpha = 0.2f;
            public Color waveformColorLow = new Color(0.1f, 0.5f, 0.2f, 1);
            public Color waveformColorHi = new Color(0.7f, 0.3f, 0.2f, 0.75f);
            public Color backgroundColor = Color.clear;

            public void Reset()
            {
                length = 100f;
                height = 20f;
                maxTextureSize = 2048;
                samples = new float[0];
                channels = 2;
                offset = 0;
                samplesCount = 0;
                rangeMin = 0.5f;
                rangeMax = 0.5f;
                directionEnum = DirectionEnum.Right;
                shadowAlpha = 0.2f;
                waveformColorLow = new Color(0.1f, 0.5f, 0.2f, 1);
                waveformColorHi = new Color(0.7f, 0.3f, 0.2f, 0.75f);
                backgroundColor = Color.clear;
            }

            public void CopyValuesFrom(Options o)
            {
                length = o.length;
                height = o.height;
                maxTextureSize = o.maxTextureSize;
                samples = o.samples;
                channels = o.channels;
                offset = o.offset;
                samplesCount = o.samplesCount;
                rangeMin = o.rangeMin;
                rangeMax = o.rangeMax;
                directionEnum = o.directionEnum;
                shadowAlpha = o.shadowAlpha;
                waveformColorHi = o.waveformColorHi;
                waveformColorLow = o.waveformColorLow;
                backgroundColor = o.backgroundColor;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        Color[] blank;
        Texture2D texture;
        int sampleCount = 0;

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            texture = null;
        }

        public override void Commit()
        {
            base.Commit();

            if (NS.debug)
            {
                //Debug.Log("GRAPH WIDTH : " + GetGraphWidth().ToString() + " GRAPH HEIGHT : " + GetGraphHeight().ToString());
                //Debug.Log("TEXTURE WIDTH : " + GetTextureWidth().ToString() + " TEXTURE HEIGHT : " + GetTextureHeight().ToString());
            }

            rectTransform.SetWidth(GetGraphWidth());
            rectTransform.SetHeight(GetGraphHeight());

            CreateTexture();
        }

        float GetGraphWidth()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return options.length;
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return options.height;
            }
            else
            {
                return options.length;
            }
        }

        float GetGraphHeight()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return options.height;
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return options.length;
            }
            else
            {
                return options.height;
            }
        }

        int GetTextureWidth()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return Mathf.CeilToInt(Mathf.Clamp(options.length, 1, options.maxTextureSize));
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return Mathf.CeilToInt(Mathf.Clamp(options.height, 1, options.maxTextureSize));
            }
            else
            {
                return Mathf.CeilToInt(Mathf.Clamp(options.length, 1, options.maxTextureSize));
            }
        }

        int GetTextureHeight()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return Mathf.CeilToInt(Mathf.Clamp(options.height, 1, options.maxTextureSize));
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return Mathf.CeilToInt(Mathf.Clamp(options.length, 1, options.maxTextureSize));
            }
            else
            {
                return Mathf.CeilToInt(Mathf.Clamp(options.height, 1, 2048));
            }
        }

        int GetTextureLength()
        {
            return Mathf.CeilToInt(Mathf.Clamp(options.length, 1, options.maxTextureSize));
        }

        int GetSampleCount()
        {
            return options.samplesCount == 0 ? options.samples.Length : options.samplesCount;
        }

        void CreateTexture()
        {
            this.image.color = Color.white;

            int size = GetSampleCount() / options.channels;

            texture = new Texture2D(GetTextureWidth(), GetTextureHeight());
            image.texture = texture;

            // create a 'blank screen' image
            blank = new Color[GetTextureWidth() * GetTextureHeight()];

            for (var i = 0; i < blank.Length; i++)
            {
                blank[i] = options.backgroundColor;
            }

            UpdateWaveTexture(size);
        }

        void DrawLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Mathf.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;

            float halfHeight = GetTextureHeight() * 0.5f;

            var shadowColor = new Color(0, 0, 0, options.shadowAlpha);

            for (int x = x0; x <= x1; x++)
            {
                var tx = (steep ? y : x);
                var ty = (steep ? x : y);

                var yy = Mathf.Abs(halfHeight - ty) / halfHeight;
                var color = Color.Lerp(options.waveformColorLow, options.waveformColorHi, yy);

                if (options.shadowAlpha > 0)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        for (int i = 0; i <= 2; i++)
                        {
                            var temp = texture.GetPixel(tx + i, ty + j);
                            if (temp != options.backgroundColor)
                            {
                                continue;
                            }
                            texture.SetPixel(tx + i, ty + j, shadowColor);
                        }
                    }
                }

                texture.SetPixel(tx, ty, color);

                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        class SampleAccum
        {
            public float min;
            public float max;

            public SampleAccum()
            {
                min = 0.5f;
                max = min;
            }
        }

        void UpdateWaveTexture(int size)
        {
            // clear the texture
            texture.SetPixels(blank, 0);

            var graph = new Dictionary<int, SampleAccum>();
            for (var i = 0; i < GetTextureLength(); i++)
            {
                graph[i] = new SampleAccum();
            }

            // accumulate samples
            for (var i = 0; i < size; i++)
            {
                for (int j = 0; j < options.channels; j++)
                {
                    var index = options.offset + (i * options.channels) + j;
                    float val = 0;
                    if (index >= 0 && index < options.samples.Length)
                    {
                        val = options.samples[index];
                    }
                    else
                    {
                        break;
                    }

                    // convert from -1..1 to 0..1 range
                    val += 1;
                    val /= 2;

                    int x = (int)(GetTextureLength() * (i / (float)size));

                    var acc = graph[x];
                    acc.min = Mathf.Min(acc.min, val);
                    acc.max = Mathf.Max(acc.max, val);
                }
            }

            // draw the waveform
            foreach (var entry in graph)
            {
                var curEntry = entry.Value;

                float minY = curEntry.min;
                float maxY = curEntry.max;

                // convert to actual limited range
                minY = Mathf.InverseLerp(options.rangeMin, options.rangeMax, minY);
                maxY = Mathf.InverseLerp(options.rangeMin, options.rangeMax, maxY);

                int x1 = entry.Key;
                int y1 = (int)(GetTextureHeight() * maxY);

                int x2 = entry.Key;
                int y2 = (int)(GetTextureHeight() * minY);

                switch (options.directionEnum)
                {
                    case DirectionEnum.Up:
                        {
                            DrawLine(y1, x1, y2, x2);
                            break;
                        }

                    case DirectionEnum.Down:
                        {
                            DrawLine(y1, GetTextureLength() - x1, y2, GetTextureLength() - x2);
                            break;
                        }

                    case DirectionEnum.Left:
                        {
                            DrawLine(GetTextureLength() - x1, y1, GetTextureLength() - x2, y2);
                            break;
                        }

                    case DirectionEnum.Right:
                    case DirectionEnum.Undefined:
                        {
                            DrawLine(x1, y1, x2, y2);
                            break;
                        }
                }
            }

            texture.Apply();
        }
    }
}
