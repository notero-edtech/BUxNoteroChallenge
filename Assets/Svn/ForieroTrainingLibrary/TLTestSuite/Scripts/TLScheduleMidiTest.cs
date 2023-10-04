/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training;
using UnityEngine;

public class TLScheduleMidiTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ScheduleRhythm()
    {
        TL.Providers.Midi.Rhythm(0);
        TL.Providers.Midi.Rhythm(1);
        TL.Providers.Midi.Rhythm(2);
        TL.Providers.Midi.Rhythm(3);
        TL.Providers.Midi.Rhythm(4);
        TL.Providers.Midi.Rhythm(5);
        TL.Providers.Midi.Rhythm(5.5f);
        TL.Providers.Midi.Rhythm(6);
        TL.Providers.Midi.Rhythm(6.5f);
        TL.Providers.Midi.Rhythm(7);
    }
}
