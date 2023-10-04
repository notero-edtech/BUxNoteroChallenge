/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using DG.Tweening;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public enum PlaybackState { Pickup, Playing, WaitingForInput, Pausing, Stop, Finished, Undefined = int.MaxValue }
        private static volatile PlaybackState _playbackState = PlaybackState.Undefined;
        private static Tween _waitForInputTween;
        public static PlaybackState playbackState
        {
            get => _playbackState;
            set
            {
                if (_playbackState == value) return;
                
                switch (value)
                {
                    case PlaybackState.Playing:
                        _waitForInputTween?.Kill();
                        Time.DSP.InitDspTimes();
                        break;
                }
                
                _playbackState = value;

                switch (value)
                {
                    case PlaybackState.Pickup:
                        PlayPickupBar(TimeSignatureStruct.numerator, BPM, () => playbackState = PlaybackState.Playing);
                        break;
                    case PlaybackState.Playing:
                        _beat = null;
                        UpdateMeasuresAndBeats();
                        if (_beat != null && Time.time.Approximately(_beat.time)) Beats.Play(_beat);
                        break;
                    case PlaybackState.WaitingForInput:
                        CancelPickupBar();
                        _waitForInputTween?.Kill();
                        _waitForInputTween = DOVirtual.DelayedCall(Interaction.waitForInputFadeOutTime, () =>
                        {
                            MidiOut.AllSoundOff();
                            Beats.Cancel();
                        });
                        break;
                    case PlaybackState.Pausing:
                        CancelPickupBar();
                        MidiOut.AllSoundOff();
                        Beats.Cancel();
                        break;
                    case PlaybackState.Undefined:
                    case PlaybackState.Stop:
                        CancelPickupBar();
                        MidiOut.AllSoundOff();
                        Beats.Cancel();
                        Time.UpdateTime(0);
                        UpdateMeasuresAndBeats();
                        break;
                    case PlaybackState.Finished:
                        CancelPickupBar();
                        MidiOut.AllSoundOff();
                        break;
                }

                OnPlaybackStateChanged?.Invoke(_playbackState);
            }
        }

        public static Action<PlaybackState> OnPlaybackStateChanged;
    }
}
