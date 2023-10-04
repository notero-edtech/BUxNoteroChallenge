#if MIDIUNIFIED_BETA
using System.IO;
using ForieroEngine.Audio.Recording;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Recording;
using UnityEngine;

public partial class TinySoundFontSynth : MonoBehaviour
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
            audioSource.clip = bgClip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        record = true;

    }

    public void StopRecording()
    {
        if (!initialized || !record) return;

        record = false;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.Play(); 
        //audioSource.volume = 0;
        fileStream.WriteHeader(outputSampleRate);
        fileStream = null;
    }
}
#endif
