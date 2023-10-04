/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using ForieroEngine.Music.Detection.Pitch;
using DG.Tweening;

public class DetectPitchTest : MonoBehaviour
{

    public enum StateEnum
    {
        None,
        Detecting,
        Detected
    }

    public enum DetectMethod
    {
        AutoCorrelator,
        FFT,
        PitchTracker
    }

    public DetectMethod detectMethod = DetectMethod.AutoCorrelator;
    public int treshhold = 50;
    public StateEnum state = StateEnum.None;

    public Text spriteFont;

    int lastSample = 0;

    AudioClip c;

    public int sampleRate = 44100;

    AutoCorrelator ac;
    FFTDetector fft;

    public Image clapImage;
    Tweener clapImageTweener = null;

    void Start()
    {
#if !UNITY_WEBGL
        ac = new AutoCorrelator(sampleRate);
        fft = new FFTDetector(sampleRate);
        c = Microphone.Start(null, true, 100, sampleRate);
        while (Microphone.GetPosition(null) < 0)
        {
        }
#endif
    }

    float pitch = 0f;
    float lastPitch = 0f;

    int[] cumulators = new int[12];
    string noteName = "";

    void Update()
    {
#if !UNITY_WEBGL
        if (state != StateEnum.Detecting)
            return;

        int pos = Microphone.GetPosition(null);

        int diff = pos - lastSample;

        if (diff > 0)
        {

            float[] samples = new float[diff * c.channels];

            c.GetData(samples, lastSample);

            switch (detectMethod)
            {
                case DetectMethod.AutoCorrelator:
                    pitch = ac.DetectPitch(samples, samples.Length);

                    if (lastPitch == 0 && pitch > 0)
                    {
                        clapImage.color = Color.green;

                        if (clapImageTweener != null)
                        {
                            clapImageTweener.Kill();
                        }

                        clapImageTweener = clapImage.DOColor(Color.white, 0.3f);
                    }

                    break;
                case DetectMethod.FFT:
                    pitch = fft.DetectPitch(samples, samples.Length);
                    break;
            }

        }

        lastPitch = pitch;

        lastSample = pos;

        noteName = Freq2Note.DominantNote(pitch);
        if (noteName != "C 0")
        {
            switch (noteName.Substring(0, 2))
            {
                case "A ":
                    cumulators[0]++;
                    break;
                case "A#":
                    cumulators[1]++;
                    break;
                case "B ":
                    cumulators[2]++;
                    break;
                case "C ":
                    cumulators[3]++;
                    break;
                case "C#":
                    cumulators[4]++;
                    break;
                case "D ":
                    cumulators[5]++;
                    break;
                case "D#":
                    cumulators[6]++;
                    break;
                case "E ":
                    cumulators[7]++;
                    break;
                case "F ":
                    cumulators[8]++;
                    break;
                case "F#":
                    cumulators[9]++;
                    break;
                case "G ":
                    cumulators[10]++;
                    break;
                case "G#":
                    cumulators[11]++;
                    break;
            }
            if (cumulators[0] > treshhold)
                PitchDetected("A");
            if (cumulators[1] > treshhold)
                PitchDetected("A#");
            if (cumulators[2] > treshhold)
                PitchDetected("B");
            if (cumulators[3] > treshhold)
                PitchDetected("C");
            if (cumulators[4] > treshhold)
                PitchDetected("C#");
            if (cumulators[5] > treshhold)
                PitchDetected("D");
            if (cumulators[6] > treshhold)
                PitchDetected("D#");
            if (cumulators[7] > treshhold)
                PitchDetected("E");
            if (cumulators[8] > treshhold)
                PitchDetected("F");
            if (cumulators[9] > treshhold)
                PitchDetected("F#");
            if (cumulators[10] > treshhold)
                PitchDetected("G");
            if (cumulators[11] > treshhold)
                PitchDetected("G#");
        }
#endif
    }

    public void StartDetection()
    {
        ResetDetection();
        state = StateEnum.Detecting;
    }

    public void ResetDetection()
    {
        state = StateEnum.None;
        cumulators = new int[12];
    }

    public void StopDetection()
    {
        ResetDetection();
    }

    void PitchDetected(string aPitch)
    {
        Debug.Log(aPitch);
        state = StateEnum.Detected;
        spriteFont.text = aPitch;
        ResetDetection();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Detect"))
        {
            state = StateEnum.Detecting;
            spriteFont.text = ".";
        }
        GUILayout.Label(pitch.ToString());
        GUILayout.Label(Freq2Note.DominantFreq(pitch).ToString());
        GUILayout.Label(Freq2Note.DominantNote(pitch));
    }

}
