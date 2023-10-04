/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.Music.NotationSystem.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSWaveTrack : NSObjectImage
    {
        public class Options : INSObjectOptions<Options>
        {
            //public float length = 100f;
            public float totalTime = 0;
            public float height = 20f;
            public int maxSegmentTextureSize = 2048;
            public float[] samples = new float[0];
            public int channels = 2;
            public DirectionEnum directionEnum = DirectionEnum.Right;

            public float onset = 0f;
            public float outset = 0f;

            [Range(0f, 1f)]
            public float shadowAlpha = 0.2f;
            public Color waveformColorLow = new Color(0.1f, 0.5f, 0.2f, 1);
            public Color waveformColorHi = new Color(0.7f, 0.3f, 0.2f, 0.75f);
            public bool background = false;
            public Color backgroundColor = Color.black;

            public void Reset()
            {
                totalTime = 1f;
                height = 20f;
                samples = new float[0];
                channels = 2;
                directionEnum = DirectionEnum.Right;
                onset = 0f;
                outset = 0f;
                shadowAlpha = 0.2f;
                waveformColorLow = new Color(0.1f, 0.5f, 0.2f, 1);
                waveformColorHi = new Color(0.7f, 0.3f, 0.2f, 0.75f);
                background = false;
                backgroundColor = Color.black;
            }

            public void CopyValuesFrom(Options o)
            {
                totalTime = o.totalTime;
                height = o.height;
                samples = o.samples;
                channels = o.channels;
                directionEnum = o.directionEnum;
                onset = o.onset;
                outset = o.outset;
                shadowAlpha = o.shadowAlpha;
                waveformColorHi = o.waveformColorHi;
                waveformColorLow = o.waveformColorLow;
                background = o.background;
                backgroundColor = o.backgroundColor;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public float length { get; private set; }

        List<NSWaveGraph> graphs = new List<NSWaveGraph>();

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            graphs = new List<NSWaveGraph>();
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            float rangeMin = 0.5f;
            float rangeMax = 0.5f;

            // we need to go over the full samples here, at least until a better solution is found
            foreach (var sample in options.samples)
            {
                var val = sample;
                // convert from -1..1 to 0..1 range
                val += 1;
                val /= 2;

                rangeMin = Mathf.Min(rangeMin, val);
                rangeMax = Mathf.Max(rangeMax, val);
            }

            var totalTime = options.totalTime - (options.onset + options.outset);
            int totalSamples = (int)(options.samples.Length * (totalTime / options.totalTime));
            int onsetSamples = (int)(options.samples.Length * (options.onset / options.totalTime));
            int outsetSamples = (int)(options.samples.Length * (options.outset / options.totalTime));

            this.length = (float)(totalTime * NSPlayback.NSRollingPlayback.pixelsPerSecond);

            Debug.Log("Score Length : " + ns.pixelLenght);
            Debug.Log("Track Length : " + this.length.ToString());

            var widthDivision = length / options.maxSegmentTextureSize;
            int graphsCount = Mathf.FloorToInt(widthDivision);
            float restGraph = widthDivision - graphsCount;

            var sampleDivision = (graphsCount <= 0) ? totalSamples : (totalSamples / widthDivision);
            int samplesPerGraph = Mathf.FloorToInt(sampleDivision);

            var restLength = (int)(options.maxSegmentTextureSize * restGraph);
            var restSamples = (int)(totalSamples - samplesPerGraph * graphsCount);


            graphs = new List<NSWaveGraph>();

            rectTransform.SetWidth(GetTrackWidth());
            rectTransform.SetHeight(GetTrackHeight());

            image.enabled = options.background;
            image.color = options.backgroundColor;

            for (int i = 0; i <= graphsCount; i++)
            {
                var graph = this.AddObject<NSWaveGraph>(pool, GetGraphPivot());
                graph.options.length =
                         i == graphsCount
                         ? restLength
                         : options.maxSegmentTextureSize;

                graph.options.height = options.height;
                graph.options.maxTextureSize = options.maxSegmentTextureSize;
                graph.options.samples = options.samples;
                graph.options.channels = options.channels;
                graph.options.rangeMin = rangeMin;
                graph.options.rangeMax = rangeMax;
                graph.options.directionEnum = options.directionEnum;
                graph.options.offset = onsetSamples + (i * samplesPerGraph);
                graph.options.samplesCount =
                         i == graphsCount
                         ? restSamples
                         : samplesPerGraph;

                graph.options.shadowAlpha = options.shadowAlpha;
                graph.options.waveformColorHi = options.waveformColorHi;
                graph.options.waveformColorLow = options.waveformColorLow;
                graph.options.backgroundColor = Color.clear;
                graph.Commit();

                graph.SendVisuallyFront(this);

                switch (options.directionEnum)
                {
                    case DirectionEnum.Right:
                        {
                            graph.PixelShiftX(options.maxSegmentTextureSize * i, false);
                            break;
                        }

                    case DirectionEnum.Left:
                        {
                            graph.PixelShiftX(options.maxSegmentTextureSize * -i, false);
                            break;
                        }

                    case DirectionEnum.Down:
                        {
                            graph.PixelShiftY(options.maxSegmentTextureSize * i, false);
                            break;
                        }

                    case DirectionEnum.Up:
                        {
                            graph.PixelShiftY(options.maxSegmentTextureSize * -i, false);
                            break;
                        }
                }

                graphs.Add(graph);
            }
        }

        PivotEnum GetGraphPivot()
        {
            var result = PivotEnum.MiddleLeft;
            switch (options.directionEnum)
            {
                case DirectionEnum.Right:
                    result = PivotEnum.MiddleLeft;
                    break;
                case DirectionEnum.Left:
                    result = PivotEnum.MiddleRight;
                    break;
                case DirectionEnum.Up:
                    result = PivotEnum.BottomCenter;
                    break;
                case DirectionEnum.Down:
                    result = PivotEnum.TopCenter;
                    break;
            }
            return result;
        }

        int GetTrackWidth()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return (int)this.length;
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return (int)options.height;
            }
            else
            {
                return (int)this.length;
            }
        }

        int GetTrackHeight()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return (int)options.height;
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return (int)this.length;
            }
            else
            {
                return (int)this.rectHeight;
            }
        }
    }
}
