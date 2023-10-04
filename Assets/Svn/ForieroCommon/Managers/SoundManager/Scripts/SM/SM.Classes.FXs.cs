using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ForieroEngine;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public partial class SM : MonoBehaviour
{
   [System.Serializable]
    public class FXItem : SoundItem {

    }

    [System.Serializable]
    public class FXGroup
    {
        public Tab tab = Tab.Self;

        public string id = "";
        public SMFXBank bank;
        public List<FXItem> items = new List<FXItem>();
        public AudioMixerGroup audioMixerGroup = null;
        [HideInInspector]
        public AudioSource audioSource;

        public List<AudioSource> loopAudioSources = new List<AudioSource>();

        public AudioSource AddOrGetLoopAudioSource(FXItem fxItem)
        {
            AudioSource loopSource = null;

            foreach (AudioSource ls in loopAudioSources)
            {
                if (ls && ls.clip == fxItem.clip)
                {
                    loopSource = ls;
                    break;
                }
            }

            if (!loopSource)
            {
                loopSource = audioSource.gameObject.AddComponent<AudioSource>();
                loopAudioSources.Add(loopSource);
            }

            loopSource.clip = fxItem.clip;
            loopSource.loop = true;

            return loopSource;
        }

        public AudioSource GetLoopAudioSource(FXItem fxItem)
        {
            AudioSource loopSource = null;

            foreach (AudioSource ls in loopAudioSources)
            {
                if (ls && ls.clip == fxItem.clip)
                {
                    loopSource = ls;
                    break;
                }
            }
            
            return loopSource;
        }

        public void FindFXItem(out FXItem fxItemReturn, string clipId)
        {
            fxItemReturn = null;
            foreach (FXItem fxItem in items)
            {
                if (string.IsNullOrEmpty(fxItem.id))
                {
                    if (fxItem.clip != null)
                    {
                        if (fxItem.clip.name == clipId)
                        {
                            fxItemReturn = fxItem; return;
                        }
                    }
                }
                else
                {
                    if (fxItem.clip != null)
                    {
                        if (fxItem.id == clipId)
                        {
                            fxItemReturn = fxItem; return;
                        }
                    }
                }
            }

            bank?.Find(out fxItemReturn, clipId);
        }
    }
}