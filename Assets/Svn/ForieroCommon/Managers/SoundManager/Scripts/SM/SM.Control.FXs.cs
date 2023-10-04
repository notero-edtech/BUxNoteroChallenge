using ForieroEngine;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public partial class SM : MonoBehaviour
{
    static FXItem fxItem = null;

    public static void PlayLoop(string clipId, float volume = -1f, string fxGroupId = "", float delay = 0f)
    {
        if (string.IsNullOrEmpty(clipId)) return;

        if (delay > 0f)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                PlayLoop(clipId, volume, fxGroupId);
            });
        }
        else
        {
            fxItem = null;
            foreach (FXGroup fxGroup in SoundSettings.instance.fxGroups)
            {
                if (!string.IsNullOrEmpty(fxGroupId) && fxGroup.id != fxGroupId)
                {
                    continue;
                }

                fxGroup.FindFXItem(out fxItem, clipId);

                if (fxItem != null)
                {
                    var source = fxGroup.AddOrGetLoopAudioSource(fxItem);
                    source.volume = volume <= 0f ? fxItem.volume : volume;
                    source.Play();
                    LogUI("PLAY SFX LOOP - Group ID: <color=blue>" + fxGroupId + "</color> Clip ID: <color=red>" + clipId + "</color> Clip Name: <color=blue>" + fxItem.clip.name + ".ogg</color>", LogEnum.Message);
                    return;
                }
            }

#if UNITY_EDITOR
            if (SoundSettings.instance.createSoundFXPlaceholders && SoundSettings.instance.soundFXPlaceholdersDirectory)
            {
                string assetPathDirectory = AssetDatabase.GetAssetPath(SoundSettings.instance.soundFXPlaceholdersDirectory) + "/" +
                    (string.IsNullOrEmpty(fxGroupId) ? "" : (fxGroupId + "/"));

                SMEditor.CreateSoundFXPlaceholder(clipId, assetPathDirectory);
            }
#endif

            Log("PLAY LOOP - SFX NOT FOUND - Group ID: <color=red>" + fxGroupId + "</color> Clip ID : <color=red>" + clipId + "</color> Clip Name: <color=red> " + clipId + ".ogg </color> ", LogEnum.Warning);
        }
    }

    public static void StopAllLoops()
    {
        foreach (FXGroup fxGroup in SoundSettings.instance.fxGroups)
        {
            foreach (FXItem fxItem in fxGroup.items)
            {                  
                var source = fxGroup.GetLoopAudioSource(fxItem);
                if (source && source.isPlaying)
                {
                    source.Stop();                   
                    LogUI("STOP SFX LOOP - Group ID: <color=red>" + fxGroup.id + "</color> Clip ID: <color=red>" + fxItem.id + "</color> Clip Name: " + fxItem.clip.name + ".ogg</color>", LogEnum.Message);
                }
            }
        }
    }
       
    public static void StopLoop(string clipId, string fxGroupId = "", float delay = 0f)
    {

        if (string.IsNullOrEmpty(clipId)) return;

        if (delay > 0f)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                StopLoop(clipId, fxGroupId);
            });
        }
        else
        {
            fxItem = null;
            foreach (FXGroup fxGroup in SoundSettings.instance.fxGroups)
            {
                if (!string.IsNullOrEmpty(fxGroupId) && fxGroup.id != fxGroupId)
                {
                    continue;
                }

                fxGroup.FindFXItem(out fxItem, clipId);

                if (fxItem != null)
                {
                    var source = fxGroup.AddOrGetLoopAudioSource(fxItem);
                    source.Stop();
                    LogUI("STOP SFX LOOP - Group ID: <color=red>" + fxGroupId + "</color> Clip ID: <color=red>" + clipId + "</color> Clip Name: " + fxItem.clip.name + ".ogg</color>", LogEnum.Message);
                    return;
                }
            }

            Log("SoundFX NOT FOUND - Group ID : " + fxGroupId + " Clip ID : " + clipId, LogEnum.Warning);
        }
    }

    public static void FindFXItem(out FXItem fxItem, string clipId, string fxGroupId = "")
    {
        fxItem = null;
        foreach (FXGroup fxGroup in SoundSettings.instance.fxGroups)
        {
            if (!string.IsNullOrEmpty(fxGroupId) && fxGroup.id != fxGroupId)
            {
                continue;
            }

            fxGroup.FindFXItem(out fxItem, clipId);

            if (fxItem != null) return;
        }
    }

    public static void PlayFX(string clipId, float volume = -1f, string fxGroupId = "", float delay = 0f)
    {
        if (string.IsNullOrEmpty(clipId)) return;

        if (delay > 0f)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                PlayFX(clipId, volume, fxGroupId);
            });
        }
        else
        {
            fxItem = null;
            foreach (FXGroup fxGroup in SoundSettings.instance.fxGroups)
            {
                if (!string.IsNullOrEmpty(fxGroupId) && fxGroup.id != fxGroupId)
                {
                    continue;
                }

                fxGroup.FindFXItem(out fxItem, clipId);

                if (fxItem != null)
                {
                    fxGroup.audioSource.PlayOneShot(fxItem.clip, volume <= 0f ? fxItem.volume : volume);
                    LogUI("PLAY SFX - Group ID: <color=blue>" + fxGroupId + "</color> Clip ID: <color=blue>" + clipId + "</color> Clip Name: <color=blue>" + fxItem.clip.name + ".ogg</color>", LogEnum.Message);
                    return;
                }
            }

#if UNITY_EDITOR
            if (SoundSettings.instance.createSoundFXPlaceholders && SoundSettings.instance.soundFXPlaceholdersDirectory)
            {
                string assetPathDirectory = AssetDatabase.GetAssetPath(SoundSettings.instance.soundFXPlaceholdersDirectory) + "/" +
                    (string.IsNullOrEmpty(fxGroupId) ? "" : (fxGroupId + "/"));

                SMEditor.CreateSoundFXPlaceholder(clipId, assetPathDirectory);
            }
#endif

            Log("PLAY LOOP - SFX NOT FOUND - Group ID: <color=red>" + fxGroupId + "</color> Clip ID : <color=red>" + clipId + "</color> Clip Name: <color=red> " + clipId + ".ogg </color> ", LogEnum.Warning);
        }
    }

}