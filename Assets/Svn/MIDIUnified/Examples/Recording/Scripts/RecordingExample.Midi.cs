using System.IO;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Recording;
using UnityEngine;

public partial class RecordingExample : MonoBehaviour {

    public void StartMidiInputRecording(){
        Recorders.MidiEvents.Start(MidiInput.singleton);
        audioSource.clip = bgMusic;
        audioSource.volume = bgMusicVolume;
        audioSource.Play();
    }

    public void StopMidiInputRecording(bool playBack = true)
    {
        Recorders.MidiEvents.Stop();
        audioSource.Stop();

        if (!playBack) return;

        string filePath = Recorders.MidiEvents.fileName.PrependPersistentPath();
        midiFileText.text = filePath;
        if (File.Exists(filePath))
        {
            seq.musicVolume = bgMusicVolume;
            seq.Initialize(File.ReadAllBytes(filePath), null, bgMusic);
            seq.PPQN *= 2;
            seq.Play(false);
        }
        else
        {
            Debug.LogWarning("Recorded midi does not exists yet : " + filePath);
        }
    }

    public void StopMidiPlayback()
    {
        seq.Stop();
    }
}
