using System.Collections;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;

public partial class RecordingExample : MonoBehaviour {

    AudioSource audioSource;
    public MidiSeqKaraokeScript seq;
    public AudioClip bgMusic;
    [Range(0f, 1f)]
    public float bgMusicVolume = 1;

    public Text synthFileText;
    public Text midiFileText;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator Start()
    {
        yield return null;
        MIDISettings.instance.inputSettings.active = true;
    }

    private void OnDestroy()
    {
        StopMidiInputRecording(false);
        StopSynthRecording(false);
    }

    public void OpenDirectory()
    {
        string path = "file:/" + "".PrependPersistentPath();
        Debug.Log(path);
        Application.OpenURL(path);
    }
}
