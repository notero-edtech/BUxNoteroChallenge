/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Inputs
        {
            public static bool block = false;
            public static bool blockUpdate = false;

            #region VOLUME
            public static float volumeLevel
            {
                get
                {
                    return Private.volumeLevel;
                }
                set
                {
                    Private.volumeLevel = value;
                }
            }

            public static Action<float> OnVolumeLevel
            {
                get
                {
                    return block ? null : Private.OnVolumeLevel;
                }
                set
                {
                    Private.OnVolumeLevel = value;
                }
            }
            #endregion

            #region TAP
            public static Action OnTapDown
            {
                get
                {
                    return block ? null : Private.OnTapDown;
                }
                set
                {
                    Private.OnTapDown = value;
                }
            }

            public static Action OnTapUp
            {
                get
                {
                    return block ? null : Private.OnTapUp;
                }
                set
                {
                    Private.OnTapUp = value;
                }
            }
            #endregion

            #region MIDI
            public static Action<int> OnMidiOn
            {
                get
                {
                    return block ? null : Private.OnMidiOn;
                }
                set
                {
                    Private.OnMidiOn = value;
                }
            }

            public static Action<int> OnMidiOff
            {
                get
                {
                    return block ? null : Private.OnMidiOff;
                }
                set
                {
                    Private.OnMidiOff = value;
                }
            }
            #endregion

            #region CLAP
            public static Action OnClap
            {
                get
                {
                    return block ? null : Private.OnClap;
                }
                set
                {
                    Private.OnClap = value;
                }
            }
            #endregion

            #region PITCH
            public static Action<int, int> OnPitch
            {
                get
                {
                    return block ? null : Private.OnPitch;
                }
                set
                {
                    Private.OnPitch = value;
                }
            }
            #endregion

            #region TUNER
            public static Action<float> OnTuner
            {
                get
                {
                    return block ? null : Private.OnTuner;
                }
                set
                {
                    Private.OnTuner = value;
                }
            }
            #endregion

            #region
            public static Action OnSpacebarDown
            {
                get
                {
                    return block ? null : Private.OnSpacebarDown;
                }
                set
                {
                    Private.OnSpacebarDown = value;
                }
            }

            public static Action OnSpacebarUp
            {
                get
                {
                    return block ? null : Private.OnSpacebarUp;
                }
                set
                {
                    Private.OnSpacebarUp = value;
                }
            }
            #endregion

            #region UPDATE
            public static Action OnUpdate
            {
                get
                {
                    return blockUpdate ? null : Private.OnUpdate;
                }
                set
                {
                    Private.OnUpdate = value;
                }
            }
            #endregion

            public static void Reset()
            {
                OnTapDown = null;
                OnTapUp = null;
                OnMidiOn = null;
                OnMidiOff = null;
                OnClap = null;
                OnPitch = null;
                OnUpdate = null;
                block = false;
                blockUpdate = false;

                Detection.Reset();
            }
        }
    }
}
