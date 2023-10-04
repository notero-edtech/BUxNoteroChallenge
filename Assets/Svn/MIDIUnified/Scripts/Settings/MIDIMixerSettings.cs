using ForieroEngine.Settings;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class MIDIMixerSettings : Settings<MIDIMixerSettings>, ISettingsProvider
{
#if UNITY_EDITOR
	[MenuItem("Foriero/Settings/Midi/Mixer")] public static void MIDIMixerSettingsMenu() => Select();	
#endif
		
	public AudioMixer mixer;
	public AudioMixerGroup metronome;
	public AudioMixerGroup microphone;
	public AudioMixerGroup accompaniment;
	public AudioMixerGroup instrument;
}
