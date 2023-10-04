/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static void Reset()
        {
            _zoom = 1f;
            _measure = _tmpMeasure = null;
            _beat = _tmpBeat = null;

            Time.Reset();

            _speed = 1;
            _tpqn = 100;
            //_playbackMode = PlaybackMode.Undefined;
            _playbackState = PlaybackState.Undefined;
            measures = new Dictionary<int, Measure>();

            NSRollingPlayback.Reset();
            NSTickerPlayback.Reset();
        }

        public static void ResetMeasures()
        {
            _beat = null;
            _measure = _tmpMeasure = null;
            _beat = _tmpBeat = null;

            Time.Reset();

            _playbackState = PlaybackState.Undefined;
            measures = new Dictionary<int, Measure>();

            NSRollingPlayback.ResetMeasures();
            NSTickerPlayback.ResetMeasures();
        }

        public static Action OnSongAnalyzed;
        public static Action OnSongInitialized;

        public static scorepartwise ScorePartWise { get; set; } = null;
        public static MIDIFile MidiFile { get; set; } = null;

        public static bool dragging = false;

        // Arguments ( Part Index, Stave Index, Options ) //
        public static TimeSignatureStruct TimeSignatureStruct { get; private set; } = new() { numerator = 4, denominator = 4 };
        public static void SetTimeSignatureAndInvoke(int part, int stave, NSTimeSignature.Options options) {
            TimeSignatureStruct = options.timeSignatureStruct;
            OnTimeSignatureChanged?.Invoke(part, stave, options);
        }
        public static Action<int, int, NSTimeSignature.Options> OnTimeSignatureChanged;
        public static Action<int, int, NSKeySignature.Options> OnKeySignatureChanged;
        public static Action<int, int, NSMetronomeMark.Options> OnMetronomeMarkChanged;

        #region PickUp Bar
        public static bool PickupBar
        {
            get => NSPlaybackSettings.instance.pickupBar;
            set { NSPlaybackSettings.instance.pickupBar = value; OnPickupBarChanged?.Invoke(value); }
        }
        public static Action<bool> OnPickupBarChanged;
        #endregion
        
        #region zoom
        private static float _zoom = 1f;
        public static float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = Mathf.Clamp(value, NSSettingsStatic.zoomMin, NSSettingsStatic.zoomMax);
                OnZoomChanged?.Invoke(_zoom);
            }
        }
        public static Action<float> OnZoomChanged;
        #endregion

        private static Measure _measure = null;
        public static Measure measure => _measure;
        public static Action<Measure> OnMeasureChanged;

        private static Beat _beat = null;
        public static Beat beat => _beat;
        public static Action<Beat> OnBeatChanged;

        private static Measure _tmpMeasure;
        private static Beat _tmpBeat;

        public static Action<double> OnTimeChanged;
        public static Action<double> OnTotalTimeChanged;

        public static int TotalMeasures => measures.Count;

        #region Speed
        private static volatile float _speed = 1;
        public static float speed
        {
            get => _speed;
            set
            {
                _speed = value;
                Beats.Cancel();
                Time.DSP.InitDspTimes();
                Beats.Schedule();
                OnSpeedChanged?.Invoke(_speed);
            }
        }
        public static Action<double> OnSpeedChanged;
        #endregion

        #region Semiton
        private static volatile int _semitone = 1;
        public static int Semitone
        {
            get => _semitone;
            set { _semitone = value; OnSemitoneChanged?.Invoke(_semitone); }
        }
        public static Action<int> OnSemitoneChanged;
        #endregion
        
        #region Tempo Per Quarter Note 
        private static double _tpqn = 100;
        public static double TPQN
        {
            get => _tpqn * _speed;
            set { _tpqn = value; OnTPQNChanged?.Invoke(_tpqn * speed); }
        }
        public static Action<double> OnTPQNChanged;
        #endregion
        
        #region BPM 
        public static double BPM => _tpqn * TimeSignatureStruct.denominator / 4f * _speed;
        #endregion

        #region PlaybackMode
        public enum PlaybackMode { Rolling, Ticker, Undefined = int.MaxValue }
        private static PlaybackMode _playbackMode = PlaybackMode.Undefined;
        public static PlaybackMode playbackMode
        {
            get => _playbackMode;
            set { _playbackMode = value; OnPlaybackModeChanged?.Invoke(_playbackMode); }
        }
        public static Action<PlaybackMode> OnPlaybackModeChanged;
        #endregion
       
        #region TimeUpdateMode
        public enum UpdateMode { Update, FixedUpdate, LateUpdate, Undefined = int.MaxValue }
        public static UpdateMode updateCameraMode = UpdateMode.Update;
        public static UpdateMode updateTimeMode = UpdateMode.Update;
        #endregion

        public static double NormalizedMeasureTime
        {
            get
            {
                if (_measure == null) return 0;
                var nextTime = _measure.nextMeasure?.time ?? Time.TotalTime;
                var lenght = nextTime - _measure.time;
                return ((Time.time - _measure.time) / lenght).Clamp(0, 1);
            }
        }

        public static double NormalizedBeatTime
        {
            get
            {
                if (playbackState == PlaybackState.Pickup) return NSPlaybackBehaviour.Instance.NormalizedBeatTime;
                if (_beat == null) return 0;
                var nextTime = _beat.nextBeat?.time ?? Time.TotalTime;
                var lenght = nextTime - _beat.time;
                return ((Time.time - _beat.time) / lenght).Clamp(0, 1);
            }
        }

        private static bool _pendulumFlip = false;
        private static float _pendulumAngle = 0;

        private static void UpdateMeasuresAndBeats()
        {
            _tmpMeasure = GetMeasureByTime(Time.time);

            if (_tmpMeasure == null) { _measure = null; _beat = null; }
            else if (_tmpMeasure != _measure) { _measure = _tmpMeasure; OnMeasureChanged?.Invoke(_measure); }

            if (_measure == null) return;

            _tmpBeat = _measure.GetBeatByTime(Time.time);

            if (_tmpBeat == null) _beat = null;
            else if (_tmpBeat != _beat)
            {
                var forward = _tmpBeat.time > (_beat?.time ?? -1);

                _beat = _tmpBeat;

                if (_pendulumFlip && _pendulumAngle > 0 && forward) _pendulumFlip = false;
                else if (!_pendulumFlip && _pendulumAngle < 0 && forward) _pendulumFlip = true;
                else if (_pendulumFlip && _pendulumAngle < 0 && !forward) _pendulumFlip = false;
                else if (!_pendulumFlip && _pendulumAngle > 0 && !forward) _pendulumFlip = true;

                Beats.Schedule();
                OnBeatChanged?.Invoke(_beat);
            }
        }

        private static void PlayPickupBar(int beats, double bpm, Action onPlay, float delay = 0.1f)
        {
            if (!PickupBar) { playbackState = PlaybackState.Playing; return; }
            if(NSPlaybackBehaviour.Instance) NSPlaybackBehaviour.Instance.PickupBar(beats, bpm, onPlay, delay);
        }
        private static void CancelPickupBar() { if(NSPlaybackBehaviour.Instance)  NSPlaybackBehaviour.Instance.CancelPickupBar(); }
    }
}
