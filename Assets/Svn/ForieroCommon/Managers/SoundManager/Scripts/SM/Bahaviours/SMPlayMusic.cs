using UnityEngine;
using System.Collections;

public class SMPlayMusic : MonoBehaviour
{
    public enum Action
    {
        Play,
        Next,
        Prev,
        Stop,
        Pause
    }

    public Action action = Action.Play;
    public string groupId = "";
    public string songId = "";

    public float volume = 1f;
    public float time = 1f;

    void Start()
    {
        switch (action)
        {
            case Action.Play:
                SM.PlayMusic(groupId,songId);
                //SM.FadeAllMusicToVolume(volume, time);
                break;
            case Action.Next:
                SM.NextMusic(groupId);
                //SM.FadeAllMusicToVolume(volume, time);
                break;
            case Action.Prev:
                SM.PrevMusic(groupId);
                //SM.FadeAllMusicToVolume(volume, time);
                break;
            case Action.Stop:
                SM.StopMusic(groupId);
                break;
            case Action.Pause:
                SM.PauseMusic(groupId);
                break;
        }
    }
}
