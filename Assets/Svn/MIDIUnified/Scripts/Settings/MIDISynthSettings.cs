/* Copyright © Marek Ledvina, Foriero s.r.o. */
using UnityEngine;
using ForieroEngine.MIDIUnified.Synthesizer;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class MIDISynthSettings : Settings<MIDISynthSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/Synth")] public static void MIDISynthSettingsMenu() => Select();  
#endif
    	
    [Header("Threading")]
    [Tooltip("Dispatch midi messages via non blocking threaded queue.")]
    public bool threaded = true;
    [Tooltip("Queued Messages thread sleeping time in milliseconds.")]
    [Range(1, 10)]public int sleep = 2;
    public bool active = true;
	
    [Header("Wwise")]
    public bool wwise = false;

    [Header("Platform Synthesizers")]
    [Tooltip("ANDROID synthetizer settings.")]
    public Synth.SynthSettingsANDROID android;
    [Tooltip("IOS synthetizer settings.")]
    public Synth.SynthSettingsIOS ios;
    [Tooltip("OSX synthetizer settings.")]
    public Synth.SynthSettingsOSX osx;
    [Tooltip("LINUX synthetizer settings.")]
    public Synth.SynthSettingsLINUX linux;
    [Tooltip("WIN synthetizer settings.")]
    public Synth.SynthSettingsWIN win;
    [Tooltip("WSA ( WINDOWS 10 ) synthetizer settings.")]
    public Synth.SynthSettingsWSA wsa;
    [Tooltip("WEBGL synthetizer settings.")]
    public Synth.SynthSettingsWEBGL webgl;

	public static Synth.SynthSettings GetPlatformSettings()
	{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return instance.osx;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		return instance.win;
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
		return instance.linux;
#elif UNITY_IOS
		return instance.ios;
#elif UNITY_WSA
		return instance.wsa;
#elif UNITY_WEBGL
		return instance.webgl;
#elif UNITY_ANDROID
		return instance.android;
#else
		return null;
#endif
	}    
}
