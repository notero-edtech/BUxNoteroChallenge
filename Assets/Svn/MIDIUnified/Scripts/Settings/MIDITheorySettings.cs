using ForieroEngine.MIDIUnified;
using ForieroEngine.Settings;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class MIDITheorySettings : Settings<MIDITheorySettings>, ISettingsProvider
{
#if UNITY_EDITOR
	[MenuItem("Foriero/Settings/Midi/Theory")] public static void MIDITheorySettingsMenu() => Select();	
#endif
		
	public KeySignatureEnum keySignature = KeySignatureEnum.CMaj_AMin;
	
	public TheorySystemEnum theorySystem = TheorySystemEnum.Undefined;
	public TonesSystemEnum tonesSystem = TonesSystemEnum.Undefined;
	
	[Header("Solfege")]
	public SolfegeSystemEnum solfegeSystem = SolfegeSystemEnum.Fixed;
	public SolfegeDisplayEnum solfegeDisplay = SolfegeDisplayEnum.Syllabic;
}
