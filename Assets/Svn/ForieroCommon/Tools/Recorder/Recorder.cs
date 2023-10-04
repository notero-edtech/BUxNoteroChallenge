#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder;
using System.Collections;

public class Recorder: MonoBehaviour
{
    public static RecorderController recorderController;

    public enum State
    {
        Recording,
        Undefined = int.MaxValue
    }

    public enum Command
    {
        Record,
        Stop,
        Undefined = int.MaxValue
    }

    public static State state = State.Undefined;
    public static Command command = Command.Undefined;
    public static string groupId = "";
    public static string itemId = "";

    IEnumerator Start()
    {
        yield return null;
        if(recorderController != null) recorderController.StartRecording();
    }

    public static void Record(string groupId, string itemId)
    {
        Recorder.groupId = groupId;
        Recorder.itemId = itemId;
        command = Command.Record;
    }

    public static void Stop(bool isPlaying = true)
    {
        command = Command.Stop;
        if (recorderController != null) recorderController.StopRecording();
        if (EditorApplication.isPlaying && isPlaying == false) EditorApplication.isPlaying = false;
    }    
}
#endif
