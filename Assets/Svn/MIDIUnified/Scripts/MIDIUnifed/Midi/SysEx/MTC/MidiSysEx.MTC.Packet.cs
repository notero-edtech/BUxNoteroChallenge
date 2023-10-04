using UnityEngine;

namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        // MTC stands for Midi Time Code //
        public static partial class MTC
        {
            public struct TimecodeFrame
            {
                // hours 0-23 //
                public int hours;
                // minutes 0-59 //      
                public int minutes;
                // seconds 0-59 //
                public int seconds;
                // frames 0-XX (depending on framerate) // 
                public int frames;
                // 0x0: 24, 0x1: 25, 0x2: 29.97, 0x3: 30 //
                public FrameRate frameRate;

                public int FramesInMilliseconds() => frameRate.FramesToMilliseconds(frames);
                public float FPS() => frameRate.ToFPS();

                public override string ToString(){ return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + (frameRate == FrameRate.FRAMERATE_30_DROP ? ";" : ":") + frames.ToString("00"); }

                public float ToSeconds()
                {
                    float time = hours * 60f * 60f;
                    time += minutes * 60f;
                    time += seconds;
                    time += frameRate.FramesToMilliseconds(frames) / 1000f;
                    return time;
                }
                               
                public void FromSeconds(float seconds, FrameRate frameRate = FrameRate.FRAMERATE_24)
                {
                    this.seconds = (int)seconds % 60;
                    minutes = (int)((seconds - this.seconds) * 0.01666666f) % 60;
                    hours = (int)(seconds * 0.00027777778f) % 60;
                    // round fractional part of seconds for ms
                    int ms = (int)(Mathf.Floor((seconds - Mathf.Floor(seconds)) * 1000f) + 0.5f);
                    frames = frameRate.MillisecondsToFrames(ms);
                    this.frameRate = frameRate;
                }

                public void Reset()
                {
                    hours = 0;
                    minutes = 0;
                    seconds = 0;
                    frames = 0;
                    frameRate = FrameRate.FRAMERATE_24;
                }
            };

            public struct QuarterFrame
            {
                // data //
                public TimecodeFrame timecodeFrame;

                // protocol handling //
                public int count; // current received QF message count //
                public bool receivedFirst; // did we receive the first message? (0x0* frames) //
                public bool receivedLast; // did we receive the last message? (0x7* hours) //
                public int lastDataByte; // last received data byte for direction detection //
                public Direction direction; // forwards or backwards? //

                public void Reset()
                {
                    count = 0;
                    receivedFirst = false;
                    receivedLast = false;
                    lastDataByte = 0x00;
                    direction = Direction.UNKNOWN;
                    timecodeFrame.Reset();
                }
            };
        }
    }
}