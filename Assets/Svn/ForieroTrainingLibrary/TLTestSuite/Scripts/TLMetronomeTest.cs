/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training;
using UnityEngine;

public class TLMetronomeTest : MonoBehaviour
{

    public int bpm = 60;

    public void OnMetronomeStart()
    {
        TL.Providers.Metronome.Start();
    }

    public void OnMetronomeStop()
    {
        TL.Providers.Metronome.Stop();
    }

    public void OnMetronomeMute()
    {
        TL.Providers.Metronome.Mute();
    }

    public void OnMetronomeUnMute()
    {
        TL.Providers.Metronome.UnMute();
    }

    void Update()
    {
        TLUnityMetronome.instance.bpm = bpm;
    }
}
