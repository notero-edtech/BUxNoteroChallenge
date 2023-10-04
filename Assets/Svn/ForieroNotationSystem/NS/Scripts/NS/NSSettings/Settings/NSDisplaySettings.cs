/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using UnityEngine;
using ForieroEngine.Settings;
using ForieroEngine.Music.NotationSystem;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Display Settings")]
[SettingsManager] public partial class NSDisplaySettings : Settings<NSDisplaySettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Display Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();
    
    [Header("Display")]
    [Tooltip("")]
    public NoteDisplayEnum noteDisplayEnum = NoteDisplayEnum.Normal;
    [Tooltip("")]
    public bool notesColored = false;
    [Tooltip("")]
    public bool notes = true;
    [Tooltip("")]
    public bool rests = true;
    [Tooltip("")]
    public bool stems = true;
    [Tooltip("")]
    public bool durationBars = true;
    
    [Tooltip("")]
    public Color backgroundColor = Color.white;
    
    [Header("Display - In Development")]
    [Tooltip("")]
    public bool tuplets = false;
    [Tooltip("")]
    public bool beams = false;
    [Tooltip("")]
    public bool ties = false;
    [Tooltip("")]
    public bool slurs = false;
    [Tooltip("")]
    public bool fingering = false;
    [Tooltip("")]
    public bool lyrics = false;
    [Tooltip("")]
    public bool pedal = false;
    
    [Tooltip("")]
    //[HideInInspector]
    public DynamicsItems dynamics;
  
    [Tooltip("")]
    //[HideInInspector]
    public ArticulationsItems articulations;

    [Tooltip("")]
    //[HideInInspector]
    public OrnamentsItems ornaments;
    
    [Tooltip("")]
    //[HideInInspector]
    public ChordsItems chords;
    
    public static bool NotesColored {
        get => instance.notesColored;
        set => instance.notesColored = value;
    }
    
    public static bool Notes
    {
        get => instance.notes;
        set => instance.notes = value;
    }
		
    public static bool Rests
    {
        get => instance.rests;
        set => instance.rests = value;
    }
    
    public static bool Stems
    {
        get => instance.stems;
        set => instance.stems = value;
    }
	
    public static bool Tuplets
    {
        get => instance.tuplets;
        set => instance.tuplets = value;
    }
		
    public static bool Beams
    {
        get => instance.beams;
        set => instance.beams = value;
    }
		
    public static bool Ties
    {
        get => instance.ties;
        set => instance.ties = value;
    }
		
    public static bool Slurs
    {
        get => instance.slurs;
        set => instance.slurs = value;
    }
    
    public static bool Fingering
    {
        get => instance.fingering;
        set => instance.fingering = value;
    }
    
    public static bool Lyrics
    {
        get => instance.lyrics;
        set => instance.lyrics = value;
    }
    
    public static bool Pedal
    {
        get => instance.pedal;
        set => instance.pedal = value;
    }

    public static bool DurationBars
    {
        get => instance.durationBars;
        set => instance.durationBars = value;
    }
    
    public static Color BackgroundColor
    {
        get => instance.backgroundColor;
        set => instance.backgroundColor = value;
    }
}
