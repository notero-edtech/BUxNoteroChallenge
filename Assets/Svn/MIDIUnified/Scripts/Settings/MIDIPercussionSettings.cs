using System;
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class MIDIPercussionSettings : Settings<MIDIPercussionSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/Percussion")] public static void MIDIPercussionSettingsMenu() => Select();   
#endif
        
    public bool initialize = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitBeforeSceneLoad()
    {
        foreach (var p in System.Enum.GetValues(typeof(PercussionEnum)))
        {
            var c = instance.GetAudioClipInternal((PercussionEnum)p);
            instance._percussionClips.Add((PercussionEnum)p, c);
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAfterSceneLoad()
    {
        if (!instance.initialize) return;
        InitPercussion();
    }

    public static void InitPercussion()
    {
        if (MIDIPercussion.instance != null) return;
        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
        new GameObject("MIDIPercussion").AddComponent<MIDIPercussion>();
        if (Debug.isDebugBuild) Debug.Log("METHOD STOPWATCH (MidiPercussionSettings - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    [Tooltip("")]
    public int maxScheduledUnits = 20;

    [Serializable] public class PercussionItem
    {
        public bool enabled = true;
        [Tooltip("")] public PercussionEnum percussion;
        [Tooltip("")] [Range(0f, 1f)] public float volume = 1f;
        public int Attack { get => volume.ToAttack(); set => volume = value.ToVolume(); }
    }

    [Serializable] public class Metronome
    {
        public PercussionItem heavy = new () { percussion = PercussionEnum.HighWoodBlock, volume = 1f };
        public PercussionItem light = new () { percussion = PercussionEnum.LowWoodBlock, volume = 1f };
        public PercussionItem subdivision = new () { percussion = PercussionEnum.BassDrum1, volume = 1f };
    }

    [Serializable] public class DrumSet
    {
        public PercussionItem hiHatClosed = new () { percussion = PercussionEnum.ClosedHiHat, volume = 1f };
        public PercussionItem hiHatOpen = new () { percussion = PercussionEnum.OpenHiHat, volume = 1f };
        public PercussionItem crash = new () { percussion = PercussionEnum.CrashCymbal1, volume = 1f };
        public PercussionItem snare = new () { percussion = PercussionEnum.AcousticSnare, volume = 1f };
        public PercussionItem tom1 = new () { percussion = PercussionEnum.HighTom, volume = 1f };
        public PercussionItem tom2 = new () { percussion = PercussionEnum.HighMidTom, volume = 1f };
        public PercussionItem tom3 = new () { percussion = PercussionEnum.LowTom, volume = 1f };
        public PercussionItem ride = new () { percussion = PercussionEnum.RideBell, volume = 1f };
        public PercussionItem kick = new () { percussion = PercussionEnum.BassDrum1, volume = 1f };
    }

    public Metronome metronome;
    public PercussionItem rhythm = new () { percussion = PercussionEnum.HandClap, volume = 1f };
    public DrumSet drumSet;
    
    [Tooltip("")]
    public AudioClip[] percussionClips;
    private readonly SortedDictionary<PercussionEnum, AudioClip> _percussionClips = new SortedDictionary<PercussionEnum, AudioClip>();

    private AudioClip GetAudioClipInternal(PercussionEnum percussionEnum)
    {
        var index = (int)percussionEnum - (int)PercussionEnum.AcousticBassDrum;
        if (index >= 0 && index < percussionClips.Length) { return percussionClips[index]; }
        return null;
    }

    public AudioClip GetAudioClip(PercussionEnum percussionEnum) => _percussionClips[percussionEnum];
    
    public static PercussionEnum GetPercussionEnum(BeatType beatType)
    {
        return beatType switch
        {
            BeatType.Heavy => instance.metronome.heavy.percussion,
            BeatType.Light => instance.metronome.light.percussion,
            BeatType.Subdivision => instance.metronome.subdivision.percussion,
            _ => PercussionEnum.HighWoodBlock
        };
    }
    
    public static float GetPercussionVolume(BeatType beatType)
    {
        return beatType switch
        {
            BeatType.Heavy => instance.metronome.heavy.volume,
            BeatType.Light => instance.metronome.light.volume,
            BeatType.Subdivision => instance.metronome.subdivision.volume,
            _ => 80f/128f
        };
    }
    
    public static int GetPercussionAttack(BeatType beatType)
    {
        return beatType switch
        {
            BeatType.Heavy => instance.metronome.heavy.Attack,
            BeatType.Light => instance.metronome.light.Attack,
            BeatType.Subdivision => instance.metronome.subdivision.Attack,
            _ => 80
        };
    }
}
