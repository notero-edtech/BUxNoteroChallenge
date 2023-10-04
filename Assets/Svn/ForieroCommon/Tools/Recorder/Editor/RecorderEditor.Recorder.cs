using System.Collections;
using System.IO;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

public partial class RecorderEditor : EditorWindow
{
    public static class RecorderInternal
    {                
        public static void Start()
        {            
            if (Recorder.recorderController == null && data.HasRecordFlag())
            {
                var current = data.GetCurrent();
                if (current != null)
                {
                    current.record = false;
                    data.Save();

                    var recorderOption = current.GetRecordingOption();
                    var recorderControllerSettings = current.GetRecorderControllerSettings();
                    var movieRecorderSettings = current.GetMovieRecorderSettings();
                    var imageRecorderSettings = current.GetImageRecorderSettings();
                    var audioRecorderSettings = current.GetAudioRecorderSettings();

                    if (recorderControllerSettings == null)
                    {
                        Debug.LogError("Please set RecorderControllerSettings!!!");
                        return;
                    }

                    switch (recorderOption)
                    {
                        case RecorderEditorData.RecordingOptions.Movie:
                            if (movieRecorderSettings == null)
                            {
                                Debug.LogError("Please set MovieRecorderSettings eighter for group or item!!!");
                                return;
                            }

                            break;
                        case RecorderEditorData.RecordingOptions.Image:
                            if (imageRecorderSettings == null)
                            {
                                Debug.LogError("Please set ImageRecorderSettings eighter for group or item!!!");
                                return;
                            }

                            break;
                        case RecorderEditorData.RecordingOptions.Audio:
                            if (audioRecorderSettings == null)
                            {
                                Debug.LogError("Please set ImageRecorderSettings eighter for group or item!!!");
                                return;
                            }

                            break;
                        case RecorderEditorData.RecordingOptions.Undefined:
                            Debug.LogWarning("Undefined recording options.");
                            return;
                    }

                    Recorder.recorderController = new RecorderController(recorderControllerSettings);
                    recorderControllerSettings.SetRecordModeToManual();

                    OnAudioFilterReadWavRecorder audioRecorder = null;

                    switch (recorderOption)
                    {
                        case RecorderEditorData.RecordingOptions.Movie:
                            FindObjectOfType<AudioListener>().gameObject.AddComponent<Recorder>();
                            recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);
                            movieRecorderSettings.OutputFile = current.GetPath();
                            break;
                        case RecorderEditorData.RecordingOptions.Image:
                            FindObjectOfType<AudioListener>().gameObject.AddComponent<Recorder>();
                            recorderControllerSettings.AddRecorderSettings(imageRecorderSettings);
                            if (current.mov)
                            {
                                var p = current.GetTempImagePath();
                                if (Directory.Exists(p)) Directory.Delete(p, true);
                                Directory.CreateDirectory(p);
                                imageRecorderSettings.OutputFile = (p + "/<Frame>").FixOSPath();
                            }
                            else
                            {
                                imageRecorderSettings.OutputFile = current.GetPath();
                            }
                            break;
                        case RecorderEditorData.RecordingOptions.Audio:                            
                            audioRecorder = FindObjectOfType<AudioListener>().gameObject.AddComponent<OnAudioFilterReadWavRecorder>();
                            audioRecorder.PrepareRecording(current.GetPath() + ".wav");
                            break;
                        case RecorderEditorData.RecordingOptions.Undefined:
                            break;
                    }
                    
                    EditorApplication.isPaused = false;
                    current.recording = true;
                                       
                    switch (recorderOption)
                    {
                        case RecorderEditorData.RecordingOptions.Movie:                            
                        case RecorderEditorData.RecordingOptions.Image:                            
                            break;
                        case RecorderEditorData.RecordingOptions.Audio:
                            if (audioRecorder)
                            {
                                audioRecorderSettings.audioMixerSnapshot.TransitionTo(0.01f);
                                audioRecorder.recording = true;
                            }
                            break;
                        case RecorderEditorData.RecordingOptions.Undefined:
                            break;
                    }
                }
            }
        }
    }
}
