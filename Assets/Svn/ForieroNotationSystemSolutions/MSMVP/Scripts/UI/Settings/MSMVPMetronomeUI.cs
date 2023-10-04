/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.Training;
using Michsky.UI.ModernUIPack;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MSMVPMetronomeUI : MonoBehaviour
{
   public SwitchManager metronomeSoundSwitch;
   [Header("Heavy")]
   public Slider heavyVolumeSlider;
   public CustomDropdown heavyPercussionDropdown;
   [Header("Light")]
   public Slider lightVolumeSlider;
   public CustomDropdown lightPercussionDropdown;
   [Header("Subdivisions")]
   public Slider subdivisionsVolumeSlider;
   public CustomDropdown subdivisionsPercussionDropdown;
   public Slider subdivisionCountSlider;
   
   private void FillDropdownPercussion(CustomDropdown dropdown)
   {
      var values = System.Enum.GetNames(typeof(PercussionEnum));
      dropdown.CreateNewItemFast("None", null);
      foreach (var v in values) { if(v != "Undefined") dropdown.CreateNewItemFast(v, null); }
      dropdown.SetupDropdown();
   }

   private IEnumerator Start()
   {
      yield return new WaitUntil(() => MIDI.initialized);
      yield return null;
      
      heavyVolumeSlider.value = MIDIPercussionSettings.instance.metronome.heavy.volume;
      FillDropdownPercussion(heavyPercussionDropdown);
      heavyPercussionDropdown.selectedItemIndex = MIDIPercussionSettings.instance.metronome.heavy.percussion.ToInt() - PercussionEnum.AcousticBassDrum.ToInt();
      heavyPercussionDropdown.SetupDropdown();
      
      lightVolumeSlider.value = MIDIPercussionSettings.instance.metronome.light.volume;
      FillDropdownPercussion(lightPercussionDropdown);
      lightPercussionDropdown.selectedItemIndex = MIDIPercussionSettings.instance.metronome.light.percussion.ToInt() - PercussionEnum.AcousticBassDrum.ToInt();
      lightPercussionDropdown.SetupDropdown();
      
      subdivisionsVolumeSlider.value = MIDIPercussionSettings.instance.metronome.subdivision.volume;
      FillDropdownPercussion(subdivisionsPercussionDropdown);
      subdivisionsPercussionDropdown.selectedItemIndex = MIDIPercussionSettings.instance.metronome.subdivision.percussion.ToInt() - PercussionEnum.AcousticBassDrum.ToInt();
      subdivisionsPercussionDropdown.SetupDropdown();
      subdivisionCountSlider.value = NSPlayback.Metronome.Subdivisions;
      
      heavyVolumeSlider.onValueChanged.AddListener(HeavySliderChange);
      heavyPercussionDropdown.dropdownEvent.AddListener(HeavyDropdownChange);
      
      lightVolumeSlider.onValueChanged.AddListener(LightSliderChange);
      lightPercussionDropdown.dropdownEvent.AddListener(LightDropdownChange);
      
      subdivisionsVolumeSlider.onValueChanged.AddListener(SubdivisionsSliderChange);
      subdivisionsPercussionDropdown.dropdownEvent.AddListener(SubdivisionsDropdownChange);
      subdivisionCountSlider.onValueChanged.AddListener(SubdivisionCountSliderChange);

      metronomeSoundSwitch.isOn = !NSPlayback.Metronome.Mute;
      metronomeSoundSwitch.UpdateUI();
      metronomeSoundSwitch.OnEvents.AddListener(OnMetronomeSoundSwitchOn);
      metronomeSoundSwitch.OffEvents.AddListener(OnMetronomeSoundSwitchOff);
   }

   private void OnMetronomeSoundSwitchOn() => NSPlayback.Metronome.Mute = false;
   private void OnMetronomeSoundSwitchOff() => NSPlayback.Metronome.Mute = true;

   private void HeavyDropdownChange(int v)
   {
      MIDIPercussionSettings.instance.metronome.heavy.percussion = (PercussionEnum)(v + PercussionEnum.AcousticBassDrum.ToInt());
      TL.Exercises.settings.metronomHeavy = (TL.Enums.MIDI.PercussionEnum)(v + PercussionEnum.AcousticBassDrum.ToInt());
   }

   private void LightDropdownChange(int v)
   {
      MIDIPercussionSettings.instance.metronome.light.percussion = (PercussionEnum)(v + PercussionEnum.AcousticBassDrum.ToInt());
      TL.Exercises.settings.metronomLight = (TL.Enums.MIDI.PercussionEnum)(v + PercussionEnum.AcousticBassDrum.ToInt());
   }

   private void SubdivisionsDropdownChange(int v)
   {
      MIDIPercussionSettings.instance.metronome.subdivision.percussion = (PercussionEnum)(v + PercussionEnum.AcousticBassDrum.ToInt());
      TL.Exercises.settings.metronomSubdivision = (TL.Enums.MIDI.PercussionEnum)(v + PercussionEnum.AcousticBassDrum.ToInt());
   } 
   
   private void SubdivisionCountSliderChange(float v)
   {
      NSPlayback.Metronome.Subdivisions = Mathf.RoundToInt(v);
      TLUnityMetronome.instance.subdivision = Mathf.RoundToInt(v);
   }

   private void HeavySliderChange(float v)
   {
      MIDIPercussionSettings.instance.metronome.heavy.volume = v;
      TL.Exercises.settings.metronomHeavyAttack = v.ToAttack();
   }

   private void LightSliderChange(float v)
   {
      MIDIPercussionSettings.instance.metronome.light.volume = v;
      TL.Exercises.settings.metronomLightAttack = v.ToAttack();
   }

   private void SubdivisionsSliderChange(float v)
   {
      MIDIPercussionSettings.instance.metronome.subdivision.volume = v;
      TL.Exercises.settings.metronomSubdivisionAttack = v.ToAttack();
   }
}
