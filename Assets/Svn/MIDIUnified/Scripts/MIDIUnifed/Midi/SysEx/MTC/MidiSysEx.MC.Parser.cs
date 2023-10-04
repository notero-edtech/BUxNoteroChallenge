namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        public static partial class MC
        {
            public static class MCParser
            {
                public static bool Parse(byte[] bytes)
                {
                    if (bytes == null || bytes.Length == 0) return false;

                    switch ((CommandEnum)bytes[0])
                    {
                        case CommandEnum.MIDI_START:
                            ticks = 1;
                            bar = 1;
                            beat = 1;
                            OnStart?.Invoke();
                            return true;
                        case CommandEnum.MIDI_STOP:
                            OnStop?.Invoke();
                            return true;
                        case CommandEnum.MIDI_CONTINUE:
                            OnContinue?.Invoke();
                            return true;
                        case CommandEnum.MIDI_TIME_CLOCK:
                            ParseTick();
                            return true;
                        case CommandEnum.MIDI_SONG_POS_POINTER:
                            return ParseSongPositionPointer(bytes);
                    }
                    return false;
                }

                static bool ParseSongPositionPointer(byte[] bytes)
                {
                    if (bytes.Length < 3) { return false; }

                    if (bytes.Length < 3) { return false; }
                    total16thTicks = (bytes[2] << 7) + bytes[1];

                    int b = total16thTicks * (timeDenominator / 4);

                    bar = b / (4 * timeNumerator) + 1;
                    beat = (b - (bar - 1) * (4 * timeNumerator)) / 4 + 1;

                    int totalBeats = (bar - 1) * timeNumerator + bar - 1;

                    ticks = (int)(totalBeats * total24thTicks * 4 / (float)timeDenominator);

                    OnBar?.Invoke();
                    OnBeat?.Invoke();
                    OnTick?.Invoke();

                    return true;
                }

                static void ParseTick()
                {
                    ticks++;
                    if (ticks * (timeDenominator / 4) % total24thTicks == 0)
                    {
                        beat = (ticks * (timeDenominator / 4) / total24thTicks) % timeNumerator + 1;
                       
                        if (beat == 1)
                        {
                            bar++;

                            OnBar?.Invoke();
                        }

                        OnBeat?.Invoke();
                        OnTick?.Invoke();
                    }
                }
            }
        }
    }
}