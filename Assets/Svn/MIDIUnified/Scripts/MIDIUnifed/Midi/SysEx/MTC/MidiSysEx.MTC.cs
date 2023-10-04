using System;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        // MTC stands for Midi Time Code //
        public static partial class MTC
        {
            public static Action OnMTCRealTimeFrame;
            public static TimecodeFrame mtcRealTimeFrame = new TimecodeFrame();

            public static Action OnMTCFullFrame;
            public static TimecodeFrame mtcFullFrame = new TimecodeFrame();
        }

        public static float ToMultiplier(this MTC.FrameRate frameRate)
        {
            switch (frameRate)
            {
                case MTC.FrameRate.FRAMERATE_24: return 1f / 24f;
                case MTC.FrameRate.FRAMERATE_25: return 1f / 25f;
                case MTC.FrameRate.FRAMERATE_30_DROP: return 1f / 29.97f;
                case MTC.FrameRate.FRAMERATE_30: return 1f / 30f;
                default: return 1f / 30f;
            }
        }

        public static float ToFPS(this MTC.FrameRate frameRate)
        {
            switch (frameRate)
            {
                case MTC.FrameRate.FRAMERATE_24: return 24f;
                case MTC.FrameRate.FRAMERATE_25: return 25f;
                case MTC.FrameRate.FRAMERATE_30_DROP: return 29.97f;
                case MTC.FrameRate.FRAMERATE_30: return 30f;
                default: return 24f;
            }
        }

        public static MTC.FrameRate ToFrameRate(float fps)
        {
            int rate = Mathf.RoundToInt(fps);
            if (fps < 25) { return MTC.FrameRate.FRAMERATE_24; }
            if (fps < 29) { return MTC.FrameRate.FRAMERATE_25; }
            if (fps < 30) { return MTC.FrameRate.FRAMERATE_30_DROP; }
            return MTC.FrameRate.FRAMERATE_30; // 30 fps
        }

        public static int FramesToMilliseconds(this MTC.FrameRate frameRate, int frames)
        {
            return (int)(frameRate.ToMultiplier() * frames * 1000f);
        }

        public static int MillisecondsToFrames(this MTC.FrameRate frameRate, int milliseconds)
        {
            return (int)(milliseconds / frameRate.ToMultiplier() / 1000f);
        }
    }
}