using System.Collections;
using System.IO;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Recording;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public partial class RecordingExample : MonoBehaviour {

    public Text speedText;
    public Slider speedSlider;
    public Text semitoneText;
    public Slider semitoneSlider;

    public void StartSynthRecording(){
        Recorders.Synth.Start(bgMusic, bgMusicVolume, speedSlider.value, (int)semitoneSlider.value);
    }

    public void StopSynthRecording(bool playBack = true){
        Recorders.Synth.Stop();

        if (!playBack) return;

        StartCoroutine(LoadClip());
    }

    IEnumerator LoadClip(){
        string filePath = Recorders.Synth.fileName.PrependPersistentPath();
        synthFileText.text = filePath;
        if (File.Exists(filePath))
        {
            string protocolFilePath = ForieroEngineExtensions.PlatformFileProtocol() + filePath;
            var www = UnityWebRequestMultimedia.GetAudioClip(protocolFilePath, AudioType.WAV);
            yield return www.SendWebRequest();
            audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.volume = 1f;
            audioSource.Play();
        } else {
            Debug.LogWarning("Recorded synth wav does not exists yet : " + filePath);
        }
    }

    public void StopSynthPlayback()
    {
        audioSource.Stop();
    }

    public void SpeedSynth()
    {
        float s = Mathf.Round(speedSlider.value * 10f) / 10f;

        speedText.text = "Speed : " + s.ToString("0.0") + "x";

        if (!BASS24SynthPlugin.audioSourceBass24) return;

        BASS24SynthPlugin.audioSourceBass24.speed = s;
    }

    public void SemitoneSynth()
    {
        int s = (int)semitoneSlider.value;

        semitoneText.text = "Semitone : " + (s > 0 ? "+" : "") + s.ToString();

        if (!BASS24SynthPlugin.audioSourceBass24) return;

        BASS24SynthPlugin.audioSourceBass24.semitone = s;
    }
}
