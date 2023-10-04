namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        // MTC stands for Midi Time Code //
        public static partial class MTC
        {
            public enum FrameRate
            {
                FRAMERATE_24 = 0x0,
                FRAMERATE_25 = 0x1,
                FRAMERATE_30_DROP = 0x2, // 29.997 drop frame //
                FRAMERATE_30 = 0x3
            };

            public enum Direction
            {
                BACKWARDS = -1, // time is moving backwards ie. rewinding //
                UNKNOWN = 0, // unknown so far //
                FORWARDS = 1  // time is advancing //
            };
        }
    }
}