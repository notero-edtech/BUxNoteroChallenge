using UnityEngine;

namespace ForieroEngine.MIDIUnified.SysEx
{
    public static partial class SysEx
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            MidiInput.MidiBytesEvent += (bytes, deviceId) =>
            {
                if (bytes == null || bytes.Length == 0) return;

                switch (bytes[0])
                {
                    case var m when (m >= 0b1111_0000 && m <= 0b1111_1111):
                        switch ((CommandEnum)m)
                        {
                            case CommandEnum.MIDI_SYSEX:
                                SysEx.MTC.MTCParser.ParseFullFrame(bytes);
                                break;
                            case CommandEnum.MIDI_TIME_CODE: SysEx.MTC.MTCParser.ParseQuarterFrame(bytes); break;
                            case CommandEnum.MIDI_START: SysEx.MC.MCParser.Parse(bytes); break;
                            case CommandEnum.MIDI_STOP: SysEx.MC.MCParser.Parse(bytes); break;
                            case CommandEnum.MIDI_CONTINUE: SysEx.MC.MCParser.Parse(bytes); break;
                            case CommandEnum.MIDI_TIME_CLOCK: SysEx.MC.MCParser.Parse(bytes); break;
                            case CommandEnum.MIDI_SONG_POS_POINTER: SysEx.MC.MCParser.Parse(bytes); break;
                        }
                        break;
                }
            };
        }
    }
}