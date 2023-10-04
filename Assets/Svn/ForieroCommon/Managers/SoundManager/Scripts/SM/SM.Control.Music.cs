using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class SM : MonoBehaviour
{
    public static void FindMusicItem(out MusicItem musicItem, string musicId, string musicGroupId = "")
    {
        musicItem = null;
        foreach (MusicGroup musicGroup in SoundSettings.instance.musicGroups)
        {
            if (!string.IsNullOrEmpty(musicGroupId) && musicGroup.id != musicGroupId)
            {
                continue;
            }

            musicGroup.FindMusicItem(out musicItem, musicId);

            if (musicItem != null) return;
        }
    }


    public static void ResetAllMusicToVolume(float volume)
    {
        foreach (MusicGroup item in SoundSettings.instance.musicGroups)
        {
            item.FadeVolume(volume, -1);
        }
    }

    public static void FadeAllMusicToVolume(float volume, float time)
    {
        foreach (MusicGroup item in SoundSettings.instance.musicGroups)
        {
            item.FadeVolume(volume, time);
        }
    }

    public static void FadeMusicToVolume(string musicGroupId, float volume, float time)
    {
        SoundSettings.instance.FindMusicGroup(musicGroupId)?.FadeVolume(volume, time);
    }

    public static void PlayMusic(string musicGroupId = "", string songId = "", bool force = true)
    {
        var musicGroup = SoundSettings.instance.FindMusicGroup(musicGroupId);
        if(musicGroup == null) SoundSettings.instance.FindMusicGroupWithSongId(songId)?.Play(songId);
                         
        musicGroup?.Play(songId, force);

        if (musicGroup == null) Log($"Can not find groupId: {musicGroupId} or songId: {songId}");
    }

    public static void StopAllMusic(float time = -1f)
    {
        foreach (MusicGroup item in SoundSettings.instance.musicGroups)
        {
            item.Stop(time);
        }
    }

    public static void StopMusic(string musicGroupId)
    {
        SoundSettings.instance.FindMusicGroup(musicGroupId)?.Stop();
    }

    public static void PauseMusic(string musicGroupId)
    {
        SoundSettings.instance.FindMusicGroup(musicGroupId)?.Pause();
    }

    public static void NextMusic(string musicGroupId)
    {
        SoundSettings.instance.FindMusicGroup(musicGroupId)?.Next();
    }

    public static void PrevMusic(string musicGroupId)
    {
        SoundSettings.instance.FindMusicGroup(musicGroupId)?.Prev();
    }
}