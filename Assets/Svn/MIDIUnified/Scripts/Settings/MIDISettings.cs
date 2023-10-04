/* Copyright © Marek Ledvina, Foriero s.r.o. */

using System;
using UnityEngine;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Settings;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class MIDISettings : Settings<MIDISettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/Midi")] public static void MIDISettingsMenu() => Select();    
#endif
       
    public static bool IsDebug => instance.debug;

    [Tooltip("Log MIDIUnified debug messages!")]
    public bool debug = false;

    [HideInInspector]
    public static string soundFontPersistentPath = "";

    [Header("Initialize MIDI?")]
    [Tooltip("If you don't want to initialize MIDIUnified at all set this to FALSE!")]
    public bool initialize = true;
        
    [Header("Company or App identifier")]
    public string appId = "com.foriero.midiunified";

    [Header("MIDI IN")]
    [Tooltip("If TRUE code will try to connect 'defaultMidiIn' port.")]
    public bool forceDefaultMidiIn = false;
    [Tooltip("If TRUE code will try to re-connect lastly opened ports.")]
    public bool restoreMidiInConnections = true;
    [Tooltip("Set it to -1 if you want to skip auto default connection attempt!")]
    [Range(-1, 10)] public int defaultMidiIn = 0;
    public string[] virtualIns = Array.Empty<string>();

    [Header("MIDI OUT")]
    [Tooltip("If TRUE code will try to connect 'defaultMidiOut' port.")]
    public bool forceDefaultMidiOut = false;
    [Tooltip("If TRUE code will try to re-connect lastly opened ports.")]
    public bool restoreMidiOutConnections = true;
    [Tooltip("Set it to -1 if you want to skip auto default connection attempt!")]
    [Range(-1, 10)] public int defaultMidiOut = -1;
    public string[] virtualOuts = Array.Empty<string>();

    [Header("Infinite Midi Loop")]
    [Tooltip("Prevents connection of IN and OUT ports with the same name.")]
    public bool midiInOutExclusive = true;
    [Tooltip("Watch Midi IN/OUT infinite loop!")]
    public bool watchInfiniteLoop = true;
    [Tooltip("Threshold for infinite loop.")]
    public int infiniteLoopThreshold = 1000;

    [HideInInspector]
    public int synthChannelMask = -1;
    [HideInInspector]
    public int channelMask = -1;

    [Header("Midi Instruments")]
    [Tooltip("Ignore 'Program Messages' also called 'Instrument Messages' when midi file starts to play")]
    public bool ignoreProgramMessages = false;
   
    [Tooltip("MIDI Instruments Settings.")]
    public MidiInstrumentsSettings instrumentsSettings;
    
    [Tooltip("MIDI Instrument Settings.")]
    public MidiInstrumentSettings instrumentSettings;
    
    [Tooltip("MIDI IN Settings.")]
    public MidiInputSettings inputSettings;

    [Tooltip("MIDI OUT Settings.")]
    public MidiOutputSettings outputSettings;
        
    [Tooltip("( MIDI ) Keyboard Settings. Please note that this is physical keyaboard on which you type. We map AWSEDFTGYHUJK to mimic real MIDI IN on your normal keyboard.")]
    public MidiKeyboardInputSettings keyboardSettings;

    [Tooltip("MIDI Playmaker Settings needs to be activated if you want to send and receive midi messages from Playmaker.")]
    public MidiPlaymakerInputSettings playmakerSettings;
}
