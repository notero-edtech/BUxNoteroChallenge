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
    public class MusicItem : SoundItem
    {

    }

    [System.Serializable]
    public class MusicGroup
    {
        public Tab tab = Tab.Self;

        public enum State
        {
            Play,
            Stop,
            Undefined = int.MaxValue
        }

        public IEnumerator Update()
        {
            while (true)
            {
                if (currentAudioSource && currentAudioSource.clip)
                {
                    if (state == State.Play
                        && currentAudioSource.time > currentAudioSource.clip.length - crossfade
                        && !crossfading
                        && !currentAudioSource.loop
                       )
                    {
                        Next(false);
                        if (currentMusicItem.loop)
                        {
                            var restTime = currentAudioSource.clip.length - currentAudioSource.time;
                            var freeAudioSource = currentAudioSource == audioSource1 ? audioSource2 : audioSource1;
                            freeAudioSource.clip = currentMusicItem.clip;
                            freeAudioSource.loop = currentMusicItem.loop;
                            freeAudioSource.PlayScheduled(AudioSettings.dspTime + restTime);
                            freeAudioSource.volume = volume * currentMusicItem.volume;
                            yield return new WaitForSecondsRealtime(restTime + 0.2f);
                            currentAudioSource = freeAudioSource;
                            Log("PLAY Loop - Group Id: <color=red>" + id + "</color> Clip Id: <color=red>" + currentMusicItem.id + "</color> Clip Name: <color=red>" + currentAudioSource.clip.name + ".ogg</color>");
                        } else
                        {
                            Play(force: true);                            
                        }
                    }
                }
                yield return null;
            }
        }

        public State state = State.Undefined;

        public string id = "";
        public bool shufle = true;
        [Range(0f, 1f)]
        public float volume = 1f;
        public float crossfade = 1f;
        public SMMusicBank bank;
        public List<MusicItem> items = new List<MusicItem>();
        public AudioMixerGroup audioMixerGroup = null;

        List<MusicItem> joinedItems = null;

        [HideInInspector]
        public AudioSource audioSource1;
        [HideInInspector]
        public AudioSource audioSource2;

        [HideInInspector]
        public MusicItem currentMusicItem = null;
        [HideInInspector]
        public AudioSource currentAudioSource;

        Tween tween1;
        Tween tween2;
        Tween tweenFadeVolume;

        public bool crossfading => tween1 != null || tween2 != null;
        public bool fading => tween1 != null || tween2 != null || tweenFadeVolume != null;        

        public void Init()
        {
            currentAudioSource = audioSource1;

            joinedItems = new List<MusicItem>();
            joinedItems.AddRange(items);

            if (bank) joinedItems.AddRange(bank.items);

            if (joinedItems.Count == 0)
            {
                Debug.LogWarning("Music Group is empty!");
                return;
            }

            if (shufle) { currentMusicItem = joinedItems[Random.Range(0, joinedItems.Count)]; }
            else { currentMusicItem = joinedItems[0]; }
        }
                
        void KillTween1()
        {
            tween1?.Kill();
            tween1 = null;
        }

        void KillTween2()
        {
            tween2?.Kill();
            tween2 = null;
        }

        void KillTweenFade()
        {
            tweenFadeVolume?.Kill();
            tweenFadeVolume = null;
        }

        public void FadeVolume(float toVolume, float time)
        {
            KillTween1();
            KillTween2();
            KillTweenFade();

            if (time >= 0f)
            {
                tweenFadeVolume = DOVirtual.Float(volume, toVolume, time, (f) =>
                {
                    //volume = f;
                    if (currentMusicItem != null)
                    {
                        audioSource1.volume = currentMusicItem.volume * volume * f;
                        audioSource2.volume = currentMusicItem.volume * volume * f;
                    }
                }).OnComplete(() =>
                {
                    tweenFadeVolume = null;
                });
            }
            else
            {
                //volume = toVolume;
                audioSource1.volume = volume * toVolume;
                audioSource2.volume = volume * toVolume;
            }
        }
                
        public void Play(string songId = "", bool force = false)
        {
            if (!audioSource1 || !audioSource2 || !currentAudioSource) return;
            
            if (state == State.Play && !force) if (audioSource1.isPlaying || audioSource2.isPlaying) return;
            
            state = State.Play;

            if (joinedItems.Count == 0)
                return;

            if (!string.IsNullOrEmpty(songId))
            {
                foreach (MusicItem mi in joinedItems)
                {
                    if (songId == mi.id || (mi.clip != null && songId == mi.clip.name))
                    {
                        currentMusicItem = mi;
                        break;
                    }
                }
            }

            if (currentMusicItem.clip == currentAudioSource.clip && currentAudioSource.isPlaying)
            {
                Log("SM : Crossfading into the same song! - " + currentMusicItem.clip.name);
            }

            if (currentAudioSource.isPlaying)
            {
                KillTween1();
                KillTween2();
                KillTweenFade();

                if (currentAudioSource == audioSource1)
                {
                    tween1 = DOVirtual.Float(audioSource1.volume, 0f, crossfade, (f) =>
                    {
                        audioSource1.volume = f;
                    }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        audioSource1.Stop();
                        tween1 = null;
                    });

                    tween2 = DOVirtual.Float(audioSource2.volume, currentMusicItem.volume * volume, crossfade, (f) =>
                    {
                        audioSource2.volume = f;
                    }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        tween2 = null;
                    });

                    currentAudioSource = audioSource2;
                }
                else
                {
                    tween1 = DOVirtual.Float(audioSource1.volume, currentMusicItem.volume * volume, crossfade, (f) =>
                    {
                        audioSource1.volume = f;
                    }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        tween1 = null;
                    });

                    tween2 = DOVirtual.Float(audioSource2.volume, 0f, crossfade, (f) =>
                    {
                        audioSource2.volume = f;
                    }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        audioSource2.Stop();
                        tween2 = null;
                    });

                    currentAudioSource = audioSource1;
                }

                currentAudioSource.clip = currentMusicItem.clip;
                currentAudioSource.loop = currentMusicItem.loop;

                Log("SM : Play - " + currentAudioSource.clip.name);
                currentAudioSource.Play();
            }
            else
            {
                if (currentAudioSource == audioSource1)
                {
                    KillTween1();
                    KillTweenFade();

                    tween1 = DOVirtual.Float(audioSource1.volume, currentMusicItem.volume * volume, crossfade, (f) =>
                    {
                        audioSource1.volume = f;
                    }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        tween1 = null;
                    });
                }
                else
                {
                    KillTween2();
                    KillTweenFade();

                    tween2 = DOVirtual.Float(audioSource2.volume, currentMusicItem.volume * volume, crossfade, (f) =>
                    {
                        audioSource2.volume = f;
                    }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        tween2 = null;
                    });
                }

                currentAudioSource.clip = currentMusicItem.clip;
                currentAudioSource.loop = currentMusicItem.loop;

                if (currentAudioSource.time > 0f)
                {
                    Log("SM : Unpause - " + currentAudioSource.clip.name);
                    currentAudioSource.UnPause();
                }
                else
                {
                    Log("SM : Play - " + currentAudioSource.clip.name);
                    currentAudioSource.Play();
                }
            }
        }

        public void Stop(float time = -1f)
        {
            state = State.Stop;

            if (joinedItems.Count == 0)
               return;

            KillTweenFade();

            KillTween1();

            if (audioSource1)
            {
                tween1 = DOVirtual.Float(audioSource1.volume, 0f, Mathf.Approximately(time, -1f) ? crossfade : time, (f) =>
                {
                    if (audioSource1) audioSource1.volume = f;
                }).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if (audioSource1) audioSource1.Stop();
                    tween1 = null;
                });
            }

            KillTween2();

            if (audioSource2)
            {
                tween2 = DOVirtual.Float(audioSource2.volume, 0f, Mathf.Approximately(time, -1f) ? crossfade : time, (f) =>
                {
                    if (audioSource2) audioSource2.volume = f;
                }).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if (audioSource2) audioSource2.Stop();
                    tween2 = null;

                });
            }
        }

        public void Pause()
        {
            if (joinedItems.Count == 0)
                return;

            KillTweenFade();

            KillTween1();

            if (audioSource1)
            {
                tween1 = DOVirtual.Float(audioSource1.volume, 0f, crossfade, (f) =>
                {
                    if (audioSource1) audioSource1.volume = f;
                }).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if (audioSource1)
                    {
                        if (currentAudioSource == audioSource1 && audioSource1.isPlaying)
                        {
                            audioSource1.Pause();
                        }
                        else
                        {
                            audioSource1.Stop();
                        }
                    }
                    tween1 = null;
                });
            }

            KillTween2();

            if (audioSource2)
            {
                tween2 = DOVirtual.Float(audioSource2.volume, 0f, crossfade, (f) =>
                {
                    if (audioSource2) audioSource2.volume = f;
                }).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if (audioSource2)
                    {
                        if (currentAudioSource == audioSource2 && audioSource2.isPlaying)
                        {
                            audioSource2.Pause();
                        }
                        else
                        {
                            audioSource2.Stop();
                        }
                    }
                    tween2 = null;
                });
            }
        }

        public void Next(bool play = true)
        {
            if (joinedItems.Count == 0)
                return;

            if (currentAudioSource.isPlaying)
            {
                if (shufle)
                {
                    MusicItem item;
                    do
                    {
                        item = joinedItems[Random.Range(0, joinedItems.Count)];
                    } while (currentMusicItem == item && joinedItems.Count > 1);
                    currentMusicItem = item;
                }
                else
                {
                    int currentIndex = -1;
                    for (int i = 0; i < joinedItems.Count; i++)
                    {
                        if (joinedItems[i] == currentMusicItem)
                        {
                            currentIndex = i;
                            break;
                        }
                    }

                    currentIndex++;
                    if (currentIndex > joinedItems.Count - 1)
                    {
                        currentIndex = 0;
                    }

                    currentMusicItem = joinedItems[currentIndex];
                }
                if(play) Play();
            }
            else
            {
                if (shufle)
                {
                    MusicItem item;
                    do
                    {
                        item = joinedItems[Random.Range(0, joinedItems.Count)];
                    } while (currentMusicItem == item && joinedItems.Count > 1);
                    currentMusicItem = item;
                }
                if(play) Play();
            }
        }

        public void Prev(bool play = true)
        {
            if (joinedItems.Count == 0)
                return;

            if (currentAudioSource.isPlaying)
            {
                if (shufle)
                {
                    MusicItem item;
                    do
                    {
                        item = joinedItems[Random.Range(0, joinedItems.Count)];
                    } while (currentMusicItem == item && joinedItems.Count > 1);
                    currentMusicItem = item;
                }
                else
                {
                    int currentIndex = -1;
                    for (int i = 0; i < joinedItems.Count; i++)
                    {
                        if (joinedItems[i] == currentMusicItem)
                        {
                            currentIndex = i;
                            break;
                        }
                    }

                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = joinedItems.Count - 1;
                    }

                    currentMusicItem = joinedItems[currentIndex];
                }
                if(play) Play(force: true);
            }
            else
            {
                if (shufle)
                {
                    MusicItem item;
                    do
                    {
                        item = joinedItems[Random.Range(0, joinedItems.Count)];
                    } while (currentMusicItem == item && joinedItems.Count > 1);
                    currentMusicItem = item;

                }
                if(play) Play(force: true);
            }
        }

        public void FindMusicItem(out MusicItem musicItemReturn, string musicId)
        {
            musicItemReturn = null;
            foreach (MusicItem musicItem in items)
            {
                if (string.IsNullOrEmpty(musicItem.id))
                {
                    if (musicItem.clip != null)
                    {
                        if (musicItem.clip.name == musicId)
                        {
                            musicItemReturn = musicItem; return;
                        }
                    }
                }
                else
                {
                    if (musicItem.clip != null)
                    {
                        if (musicItem.id == musicId)
                        {
                            musicItemReturn = musicItem; return;
                        }
                    }
                }
            }

            bank?.Find(out musicItemReturn, musicId);
        }
    }
}
