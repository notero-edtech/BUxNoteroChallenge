/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.Training.Classes.Providers;

public partial class TLUnityMetronome : MonoBehaviour
{
    public int bpm
    {
        get => TLUnityMetronomeProvider.provider.bpm;
        set => TLUnityMetronomeProvider.provider.bpm = value;
    }

    public int subdivision
    {
        get => TLUnityMetronomeProvider.provider.subdivisions;
        set => TLUnityMetronomeProvider.provider.subdivisions = value;
    }

    public int beats
    {
        get => TLUnityMetronomeProvider.provider.beats;
        set => TLUnityMetronomeProvider.provider.beats = value;
    }

    public double totalTime
    {
        get => TLUnityMetronomeProvider.provider.totalTime;
        set => TLUnityMetronomeProvider.provider.totalTime = value;
    }

    public double measureTime
    {
        get => TLUnityMetronomeProvider.provider.measureTime;
        set => TLUnityMetronomeProvider.provider.measureTime = value;
    }

    public bool pickupBar
    {
        get => TLUnityMetronomeProvider.provider.pickupBar;
        set => TLUnityMetronomeProvider.provider.pickupBar = value;
    }
}
