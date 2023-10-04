using System.IO;
using ForieroEngine.Audio.Recording;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Recording;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneWavRecorder : MonoBehaviour
{
    private int bufferSize;
    private int numBuffers;
    private int outputSampleRate = 44100;
    private readonly string fileName = "microphone.wav";

    public bool Recording { get; private set; } = false;

    private FileStream fileStream;

    // Audio Source //
    private AudioSource audioSource;

    private void Awake()
    {
        outputSampleRate = AudioSettings.outputSampleRate;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        AudioSettings.GetDSPBufferSize(out bufferSize, out numBuffers);
    }

    private void OnDestroy()
    {
        StopRecording();
    }

    public void StartRecording(string microphoneName = null){
    #if !UNITY_WEBGL
        microphoneName = microphoneName.MicrophoneExistsOrDefault();
        if (string.IsNullOrEmpty(microphoneName)) return;

        Debug.Log("Start recording : " + microphoneName);

        StopRecording(microphoneName);
        audioSource.clip = Microphone.Start(microphoneName, true, 100, outputSampleRate);
        audioSource.loop = true;             
        audioSource.mute = true;             
        while (!(Microphone.GetPosition(microphoneName) > 0)) { }                    
        audioSource.Play();

        if (fileStream != null)
        {
            Debug.LogError("FileStream alread open. Seems like recording session is in the proccess!");
            return;
        }

        string path = fileName.PrependPersistentPath();
        Debug.Log("Saving : " + path);
        fileStream = new FileStream(path, FileMode.Create);

        fileStream.PrepareHeader();

        Recording = true;
    #endif
    }

    public void StopRecording(string microphoneName = null){
    #if !UNITY_WEBGL
        microphoneName = microphoneName.MicrophoneExistsOrDefault();
        if (string.IsNullOrEmpty(microphoneName)) return;

        if (fileStream == null) return;

        Debug.Log("Stop recording : " + microphoneName);

        Recording = false;

        if (string.IsNullOrEmpty(microphoneName))
        {
            microphoneName = Microphone.devices[1];
        }

        Microphone.End(microphoneName);
        audioSource.Stop();

        var clip = audioSource.clip.TrimSilence(0);

        float[] data = new float[clip.samples * clip.channels];
        if (clip.GetData(data, 0)) fileStream.ConvertAndWrite(data);

        fileStream.WriteHeader(clip);
        
        fileStream = null;
    #endif
    }
}