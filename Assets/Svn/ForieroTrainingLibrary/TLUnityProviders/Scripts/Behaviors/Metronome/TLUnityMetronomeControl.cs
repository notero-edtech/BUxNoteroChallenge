/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.Training;
using UnityEngine;

public class TLUnityMetronomeControl : MonoBehaviour
{
    public int beatsMin = 20;
    public int beatsMax = 208;

    private void Start()
    {
        MidiOut.InitPercussion();
        TLUnityMetronome.instance.pendulumOnset = true;
        TLUnityMetronome.instance.pickupBar = true;
    }

    private void OnDisable() { if (TLUnityMetronome.instance.state == TLUnityMetronome.State.Running) TL.Providers.Metronome.Stop(); }

    private void OnDestroy() { TL.Providers.Metronome.Stop(); }

    public void OnStartStopClick()
    {
        switch (TLUnityMetronome.instance.state)
        {
            case TLUnityMetronome.State.Stop: TL.Providers.Metronome.Start(); break;
            case TLUnityMetronome.State.Start: TL.Providers.Metronome.Stop(); break;
            case TLUnityMetronome.State.Running: TL.Providers.Metronome.Stop(); break;
        }
    }

    public void OnStartClick() { TL.Providers.Metronome.Start(); }
    public void OnStopClick() { TL.Providers.Metronome.Stop(); }
    public void SetBPM(int bpm) { bpm = Mathf.Clamp(bpm, beatsMin, beatsMax); TL.Providers.Metronome.bpm = bpm; }
    public void AddBPM(int i)
    {
        int bpm = Mathf.FloorToInt(TL.Providers.Metronome.bpm / Mathf.Abs(i)) * Mathf.Abs(i) + i;
        bpm = Mathf.Clamp(bpm, beatsMin, beatsMax);
        TL.Providers.Metronome.bpm = bpm;
    }

    public void AddSubdivisions(int subdivisions)
    {
        TL.Providers.Metronome.subdivisions += subdivisions;
        TL.Providers.Metronome.subdivisions = Mathf.Clamp(TL.Providers.Metronome.subdivisions, 0, 6);
    }

    public void AddBeats(int beats)
    {
        TL.Providers.Metronome.beats += beats;
        TL.Providers.Metronome.beats = Mathf.Clamp(TL.Providers.Metronome.beats, 1, 6);
    }
}
