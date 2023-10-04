using System.Collections;
using System.IO;
using ForieroEngine.Audio.Recording;
using ForieroEngine.Extensions;
using UnityEngine;

public class OnAudioFilterReadWavRecorder : MonoBehaviour
{    
    int outputRate = 44100;
    readonly string fileName = "onaudiofilterread.wav";
        
    public volatile bool recording;
    volatile bool canRecord;
                    
    FileStream fileStream;

    void Awake() => outputRate = AudioSettings.outputSampleRate;
    void OnDestroy() => StopRecording();

    public void PrepareRecording(string filePath = "")
    {
        if (fileStream != null && recording)
        {
            Debug.LogError("FileStream already opened. Seems like recording session is in the proccess!");
            return;
        }

        string path = string.IsNullOrEmpty(filePath) ? fileName.PrependPersistentPath() : filePath;
        Debug.Log("Saving : " + path);

        fileStream = new FileStream(path, FileMode.Create);

        fileStream.PrepareHeader();
    }

    public void StartRecording(string filePath = ""){
        PrepareRecording(filePath);
        recording = true;
    }

    public void StopRecording(){
        recording = false;
        fileStream.WriteHeader(outputRate);
        fileStream = null;
    }

    IEnumerator Start()
    {
        yield return null;
        canRecord = true;
    }
      
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (recording && canRecord) fileStream.ConvertAndWrite(data);
    }
}