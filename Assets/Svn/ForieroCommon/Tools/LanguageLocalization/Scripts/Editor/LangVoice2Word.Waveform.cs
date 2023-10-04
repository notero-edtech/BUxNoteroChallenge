using UnityEditor;
using UnityEngine;

public partial class LangVoice2Word : EditorWindow
{
    public static Texture2D AudioWaveform(AudioClip aud, int width, int height, Color color)
    {
        int step = Mathf.CeilToInt((aud.samples * aud.channels) / width);
        float[] samples = new float[aud.samples * aud.channels];

        aud.GetData(samples, 0);

        Texture2D img = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] xy = new Color[width * height];
        for (int x = 0; x < width * height; x++)
        {
            xy[x] = new Color(0, 0, 0, 0);
        }

        img.SetPixels(xy);

        int i = 0;
        while (i < width)
        {
            int barHeight = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(samples[i * step]) * height, 0, height));
            int add = samples[i * step] > 0 ? 1 : -1;
            for (int j = 0; j < barHeight; j++)
            {
                img.SetPixel(i, Mathf.FloorToInt(height / 2) - (Mathf.FloorToInt(barHeight / 2) * add) + (j * add), color);
            }
            ++i;

        }

        img.Apply();
        return img;
    }

    public static Texture2D AudioWaveform(AudioClip clip, float from, float to, int width, int height, Color color)
    {

        //float timeFraction = (to - from) / clip.length;

        int shift = Mathf.FloorToInt(clip.samples * clip.channels * from / clip.length);

        int step = Mathf.CeilToInt(
            ((clip.samples * clip.channels) * ((to - from) / clip.length)) / width
            );

        float[] samples = new float[clip.samples * clip.channels];

        clip.GetData(samples, 0);

        Texture2D img = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] xy = new Color[width * height];
        for (int x = 0; x < width * height; x++)
        {
            xy[x] = new Color(0, 0, 0, 0);
        }

        img.SetPixels(xy);

        int i = 0;
        while (i < width && shift + i * step < samples.Length)
        {

            int barHeight = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(samples[shift + i * step]) * height, 0, height));

            int add = samples[shift + i * step] > 0 ? 1 : -1;

            for (int j = 0; j < barHeight; j++)
            {
                img.SetPixel(i, Mathf.FloorToInt(height / 2) - (Mathf.FloorToInt(barHeight / 2) * add) + (j * add), color);
            }
            ++i;

        }

        img.Apply();
        return img;
    }
}