/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ForieroEngine.Music.Training;
using UnityEngine;
using UnityEngine.UI;

public class TLMicrophoneTest : MonoBehaviour
{

    public Image image;
    public Image volumeMeter;
    public Text pitchText;
    public Text clapText;
    public Text freqText;

    Tweener tweener = null;

    int counter = 0;

    float lastVolumeLevel = 0f;
    float volumeLevelVelocity = 0f;

    public float volumeSmoothDampTime = 0.3f;

    void Start()
    {
        TL.Inputs.OnClap += () =>
        {
            counter++;

            clapText.text = counter.ToString();

            image.color = Color.green;

            if (tweener != null)
            {
                tweener.Kill();
            }

            tweener = image.DOColor(Color.white, 0.3f);
        };

        TL.Inputs.OnTuner += (float f) =>
        {
            freqText.text = f.ToString() + "Hz";
        };

        TL.Inputs.OnPitch += (int i, int offsetInCents) =>
        {
            pitchText.text = i.ToString() + " " + offsetInCents.ToString();
        };
    }

    void Update()
    {
        volumeMeter.material.SetFloat("_Level", Mathf.SmoothDamp(lastVolumeLevel, TL.Inputs.volumeLevel, ref volumeLevelVelocity, volumeSmoothDampTime, 10, Time.deltaTime));

        lastVolumeLevel = TL.Inputs.volumeLevel;
    }
}
