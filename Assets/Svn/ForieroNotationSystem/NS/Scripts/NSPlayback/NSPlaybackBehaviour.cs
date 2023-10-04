/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public class NSPlaybackBehaviour : MonoBehaviour
    {
        public static NSPlaybackBehaviour Instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            NSPlayback.Time.DSP.Init();            
            var fpp = new GameObject("NSPlaybackBehaviour");
            DontDestroyOnLoad(fpp);
            fpp.AddComponent<NSPlaybackBehaviour>();
        }

        private void Awake() { Instance = this; }
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => MidiInput.singleton);
            NSPlayback.Interaction.Init();
        }

        private void OnDestroy()
        {
            NSPlayback.Time.DSP.Terminate();
            Instance = null;
        }

        public void OnApplicationQuit() => NSPlayback.Time.DSP.Terminate();

        private Coroutine _c = null;
        private readonly List<double> _beatTimes = new();
        private IEnumerator SchedulePlay(Action onPlay)
        {
            void SetNormalizedBeatTime(int i)
            {
                var l = _beatTimes[i] - _beatTimes[i - 1];
                var c = AudioSettings.dspTime >= _beatTimes[i - 1] ? AudioSettings.dspTime - _beatTimes[i - 1] : 0;
                NormalizedBeatTime = (c / l).Clamp(0,1);
            }
            if (_beatTimes.Count <= 1) yield break;
            var i = 1;
            while (i < _beatTimes.Count - 1)
            {
                if (AudioSettings.dspTime >= _beatTimes[i]) {
                    i++;
                    NSPlayback.Metronome.PendulumFlip = !NSPlayback.Metronome.PendulumFlip;
                    NSPlayback.OnBeatChanged?.Invoke(new NSPlayback.Beat(i - 1, (float)_beatTimes[i], NSPlayback.measure));
                } 
                SetNormalizedBeatTime(i);
                yield return null;
            }

            while (AudioSettings.dspTime < _beatTimes[i])
            {
                SetNormalizedBeatTime(i);
                yield return null;
            }
            _c = null;
            onPlay?.Invoke();
        }

        public double NormalizedBeatTime { get; private set; } = 0;

        public void PickupBar(int beats, double bpm, Action onPlay, float delay = 0.1f)
        {
            if(_c != null) StopCoroutine(_c);
            _beatTimes.Clear();
            var t = 0.0;
            _beatTimes.Add(AudioSettings.dspTime);
            for (var b = 1; b <= beats; b++)
            {
                var beatTime = delay + (b - 1) * 60f / bpm;

                if (b == 1)
                {
                    t = MidiOut.SchedulePercussion(
                        MIDIPercussionSettings.GetPercussionEnum(BeatType.Heavy),
                        MIDIPercussionSettings.GetPercussionAttack(BeatType.Heavy),
                        beatTime,
                        false
                    );
                    _beatTimes.Add(t);
                }
                else
                {
                    t = MidiOut.SchedulePercussion(
                        MIDIPercussionSettings.GetPercussionEnum(BeatType.Light),
                        MIDIPercussionSettings.GetPercussionAttack(BeatType.Light),
                        beatTime,
                        false
                    );
                    _beatTimes.Add(t);
                }
            }

            t += 60f / bpm;
            _beatTimes.Add(t);
            _c = StartCoroutine(SchedulePlay(onPlay));
        }

        public void CancelPickupBar()
        {
            if (_c == null) return;
            if(NSPlayback.beat != null) NSPlayback.OnBeatChanged?.Invoke(NSPlayback.beat);
            StopCoroutine(_c);
        }
    }
}
