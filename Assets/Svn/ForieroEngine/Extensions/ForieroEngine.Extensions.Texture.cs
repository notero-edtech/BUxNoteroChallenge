using UnityEngine;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        static public void DrawWaveGraph(this Texture2D text, AudioClip anAudioClip, Color aColor)
        {
            if (text == null)
                return;
            try
            {
                int w = text.width;
                int h = text.height;
                int waveHeight = h / 2;
                int x = 0;
                int y = 0;
                float[] data = new float[anAudioClip.samples * anAudioClip.channels];
                anAudioClip.GetData(data, 0);
                int l = data.Length;
                for (int i = 0; i < l; i++)
                {
                    x = Mathf.RoundToInt((((float)i / (float)l) * (float)w));
                    y = Mathf.RoundToInt(((data[i] * ((float)waveHeight / 2f)) / ((float)waveHeight / 2f)) * (float)waveHeight);
                    //Debug.Log("X " + x.ToString() + " Y " + y.ToString());
                    for (int k = 0; k < Mathf.Abs(y); k++)
                    {
                        text.SetPixel(x, (int)(h / 2 + k * Mathf.Sign(y)), aColor);
                    }
                }
            }
            catch (System.Exception excp)
            {
                Debug.Log(excp.Message);
            }
            text.Apply();
        }

        static public void ClearTexture(this Texture2D text, Color aColor)
        {
            if (text == null)
                return;
            Color[] colors = new Color[text.width * text.height].Populate(aColor);
            text.SetPixels(colors);
            text.Apply();
        }

        static public void FlipPixelsHorizontally(this Texture2D text)
        {
            if (text == null)
                return;
            Color tmp;
            Color[] colors = text.GetPixels();
            for (int i = 0; i < text.height; i++)
            {
                for (int j = 0; j < text.width; j++)
                {
                    tmp = colors[i * text.width + j];
                    colors[i * text.width + j] = colors[i * text.width + text.width - 1 - j];
                    colors[i * text.width + text.width - 1 - j] = tmp;
                }
            }
            text.SetPixels(colors);
            text.Apply();
        }

        static public void FlipPixelsVertically(this Texture2D text)
        {
            if (text == null)
                return;
            Color tmp;
            Color[] colors = text.GetPixels();
            for (int i = 0; i < text.width; i++)
            {
                for (int j = 0; j < text.height; j++)
                {
                    tmp = colors[i * text.height + j];
                    colors[i * text.height + j] = colors[i * text.height + text.height - 1 - j];
                    colors[i * text.height + text.height - 1 - j] = tmp;
                }
            }
            text.SetPixels(colors);
            text.Apply();
        }
    }
}