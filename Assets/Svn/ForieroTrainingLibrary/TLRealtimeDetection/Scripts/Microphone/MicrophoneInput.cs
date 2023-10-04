/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    AudioSource audioSrc;

    public static MicrophoneInput singleton;

    public static bool analyze = false;
    public static float sensitivity = 1.0f;
    public static int samplerate = 22048;
    public static float averageVolume = 0.0f;
    public static float frequency = 0.0f;

    public static bool analyzeAverageVolume = true;
    public static bool analyzeFundamentalFrequency = false;

    public static void StartMic(string MicName)
    {
#if !UNITY_WEBGL
        if (!singleton) return;
        if (analyze) StopMic(MicName);
        singleton.audioSrc.clip = Microphone.Start(MicName, true, 10, samplerate);
        singleton.audioSrc.loop = true;                                                     // Set the AudioClip to loop
        singleton.audioSrc.mute = true;                                                     // Mute the sound, we don't want the player to hear it
        while (!(Microphone.GetPosition(MicName) > 0)) { }                                  // Wait until the recording has started
        singleton.audioSrc.Play();                                                          // Play the audio source!
        analyze = true;
#endif
    }

    public static void StopMic(string aMicName)
    {
#if !UNITY_WEBGL
        if (!singleton)
            return;
        singleton.audioSrc.Stop();
        Microphone.End(aMicName);
        averageVolume = 0f;
        frequency = 0f;
        analyze = false;
#endif
    }

    public static void DrawLevelMeter(Rect aRect, float sensitivity)
    {
        GUI.Box(new Rect(aRect.x, aRect.y, aRect.width, aRect.height), "", "mic_color");
        float level = (aRect.height) * MicrophoneInput.averageVolume * sensitivity;
        level = Mathf.Clamp(level, 0f, aRect.height);
        GUI.Box(new Rect(aRect.x, aRect.y, aRect.width, (aRect.height - level) < 16f ? 16f : aRect.height - level), "", "mic_overlay");
        GUI.Box(new Rect(aRect.x, aRect.y, aRect.width, aRect.height), "", "mic_box");
    }

    void OnEnable()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    void Awake()
    {
        singleton = this;
    }

    void Update()
    {
        if (analyze)
        {
            if (analyzeAverageVolume)
                averageVolume = GetAveragedVolume() * sensitivity;
            if (analyzeFundamentalFrequency)
                frequency = GetFundamentalFrequency();
        }
    }

    //	void OnGUI(){
    //		GUILayout.Label(averageVolume.ToString());
    //	}

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audioSrc.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

    float GetFundamentalFrequency()
    {
        float fundamentalFrequency = 0.0f;
        float[] data = new float[8192];
        audioSrc.GetSpectrumData(data, 0, FFTWindow.BlackmanHarris);
        float s = 0.0f;
        int i = 0;
        for (int j = 1; j < 8192; j++)
        {
            if (s < data[j])
            {
                s = data[j];
                i = j;
            }
        }
        fundamentalFrequency = i * samplerate / 8192;
        return fundamentalFrequency;
    }

}
