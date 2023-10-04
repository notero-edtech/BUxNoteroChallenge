namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        // MTC stands for Midi Time Code //
        public static partial class MTC
        {
            public static class MTCParser
            {
                // number of bytes in a FF message //
                const int FULLFRAME_LEN = 10;

                public static bool IsFullFrameTimeCode(byte[] bytes)
                {
                    if (bytes.Length < FULLFRAME_LEN) return false;

                    return (bytes[1] == 0x7F) && // universal message
                            (bytes[2] == 0x7F) && // global broadcast
                            (bytes[3] == 0x01) && // time code
                            (bytes[4] == 0x01) && // full frame
                            (bytes[9] == 0xF7);   // end of sysex
                }

                public static bool ParseFullFrame(byte[] bytes)
                {
                    if (!IsFullFrameTimeCode(bytes)) return false;

                    mtcFullFrame.hours = bytes[5] & 0x1F;
                    mtcFullFrame.frameRate = (FrameRate)((bytes[5] & 0x60) >> 5);
                    mtcFullFrame.minutes = bytes[6];
                    mtcFullFrame.seconds = bytes[7];
                    mtcFullFrame.frames = bytes[8];

                    OnMTCFullFrame?.Invoke();
                    return true;
                }

                // number of QF messages to make up a full MTC frame //
                const int QUARTERFRAME_LEN = 8;

                static QuarterFrame quarterFrame;

                public static bool ParseQuarterFrame(byte[] bytes)
                {
                    bool complete = false;
                    int dataByte = bytes[1];
                    int msgType = dataByte & 0xF0;

                    if (quarterFrame.direction == Direction.UNKNOWN && quarterFrame.count > 1)
                    {
                        int lastMsgType = quarterFrame.lastDataByte & 0xF0;
                        if (lastMsgType < msgType)
                        {
                            quarterFrame.direction = Direction.FORWARDS;
                        }
                        else if (lastMsgType > msgType)
                        {
                            quarterFrame.direction = Direction.BACKWARDS;
                        }
                    }

                    quarterFrame.lastDataByte = dataByte;

                    switch (msgType)
                    {
                        case 0x00: // frame LSB
                            quarterFrame.timecodeFrame.frames = dataByte & 0x0F;
                            quarterFrame.count += 1;
                            quarterFrame.receivedFirst = true;
                            if (quarterFrame.count >= QUARTERFRAME_LEN &&
                                quarterFrame.direction == Direction.BACKWARDS &&
                                quarterFrame.receivedLast)
                            {
                                complete = true;
                            }
                            break;
                        case 0x10: // frame MSB
                            quarterFrame.timecodeFrame.frames |= (dataByte & 0x01) << 4;
                            quarterFrame.count += 1;
                            break;
                        case 0x20: // second LSB
                            quarterFrame.timecodeFrame.seconds = dataByte & 0x0F;
                            quarterFrame.count += 1;
                            break;
                        case 0x30: // second MSB
                            quarterFrame.timecodeFrame.seconds |= (dataByte & 0x03) << 4;
                            quarterFrame.count += 1;
                            break;
                        case 0x40: // minute LSB
                            quarterFrame.timecodeFrame.minutes = dataByte & 0x0F;
                            quarterFrame.count += 1;
                            break;
                        case 0x50: // minute MSB
                            quarterFrame.timecodeFrame.minutes |= (dataByte & 0x03) << 4;
                            quarterFrame.count += 1;
                            break;
                        case 0x60: // hours LSB
                            quarterFrame.timecodeFrame.hours = dataByte & 0x0F;
                            quarterFrame.count += 1;
                            break;
                        case 0x70: // hours MSB & framerate
                            quarterFrame.timecodeFrame.hours |= (dataByte & 0x01) << 4;
                            quarterFrame.timecodeFrame.frameRate = (FrameRate)((dataByte & 0x06) >> 1);
                            quarterFrame.count += 1;
                            quarterFrame.receivedLast = true;
                            if (quarterFrame.count >= QUARTERFRAME_LEN &&
                                quarterFrame.direction == Direction.FORWARDS &&
                                quarterFrame.receivedFirst)
                            {
                                complete = true;
                            }
                            break;
                        default:
                            return false;
                    }

                    // update time using the (hopefully) complete message
                    if (complete)
                    {
                        // add 2 frames to compensate for time it takes to receive 8 QF messages
                        quarterFrame.timecodeFrame.frames += 2;
                        mtcRealTimeFrame.hours = quarterFrame.timecodeFrame.hours;
                        mtcRealTimeFrame.minutes = quarterFrame.timecodeFrame.minutes;
                        mtcRealTimeFrame.seconds = quarterFrame.timecodeFrame.seconds;
                        mtcRealTimeFrame.frames = quarterFrame.timecodeFrame.frames;
                        mtcRealTimeFrame.frameRate = quarterFrame.timecodeFrame.frameRate;

                        OnMTCRealTimeFrame?.Invoke();

                        quarterFrame.Reset();
                        return true;
                    }

                    return false;
                }
            }
        }
    }
}