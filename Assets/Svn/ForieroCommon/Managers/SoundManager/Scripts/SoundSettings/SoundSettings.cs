using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class SoundSettings : Settings<SoundSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Sound", false, -1000)] public static void SoundSettingsMenu() => Select();    
#endif

    public bool log = false;
    public bool logUI = false;

#if UNITY_EDITOR
    public bool createSoundFXPlaceholders = false;
    public Object soundFXPlaceholdersDirectory;
#endif

    public AudioMixer audioMixer;

    public enum Tab
    {
        FXs,
        Music
    }
    
    public Tab tab = Tab.FXs;

    public List<SM.FXGroup> fxGroups;

    public List<SM.MusicGroup> musicGroups;

    public SM.MusicGroup FindMusicGroup(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        foreach (SM.MusicGroup item in musicGroups)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        if (instance.log) Debug.LogWarning("MusicGroup not found : " + id);

        return null;
    }

    public SM.MusicGroup FindMusicGroupWithSongId(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        foreach (SM.MusicGroup musicGroup in musicGroups)
        {
            foreach(SM.MusicItem musicItem in musicGroup.items){
                if (musicItem.id == id) return musicGroup;
                if (musicItem.clip && musicItem.clip.name == id) return musicGroup;
            }
        }

        if (instance.log) Debug.LogWarning("MusicGroup not found for songId : " + id);

        return null;
    }


    public SM.FXGroup FindFXGroup(string id)
    {
        foreach (SM.FXGroup item in fxGroups)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        if (instance.log) Debug.LogWarning("FXGroup not found : " + id);

        return null;
    }
}
