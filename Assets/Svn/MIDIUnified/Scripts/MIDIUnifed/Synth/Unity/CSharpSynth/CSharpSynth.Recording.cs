using System.IO;
using ForieroEngine.Audio.Recording;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Recording;
using UnityEngine;

public partial class CSharpSynth : MonoBehaviour
{
    private bool record;

    private FileStream fileStream;

    public void StartRecording(AudioClip bgClip = null, float volume = 1f, float speed = 1f, int semitone = 0)
    {
        if (!initialized) return;

        if (fileStream != null)
        {
            Debug.LogError("FileStream alread open. Seems like recording session is in the proccess!");
            return;
        }

        string path = Recorders.Synth.fileName.PrependPersistentPath();
        Debug.Log("Saving : " + path);

        fileStream = new FileStream(path, FileMode.Create);
        fileStream.PrepareHeader();

        if (bgClip)
        {
            _audioSource.clip = bgClip;
            _audioSource.volume = volume;
            _audioSource.Play();
        }

        record = true;

    }

    public void StopRecording()
    {
        if (!initialized || !record) return;

        record = false;
        _audioSource.Stop();
        _audioSource.clip = null;
        _audioSource.volume = 1f;
        _audioSource.Play(); 
        //audioSource.volume = 0;
        fileStream.WriteHeader(_outputSampleRate);
        fileStream = null;
    }
}
