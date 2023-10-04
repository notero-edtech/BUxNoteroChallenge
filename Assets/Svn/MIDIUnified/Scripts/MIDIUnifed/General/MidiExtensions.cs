using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public static class MidiExtensions
{
    public static void Log(this byte[] data, int deviceId,  string prefix = "", string format = "X")
    {
        if (data == null) return;

        var s = "";
        foreach (var b in data) { s += b.ToString(format) + " "; }
        s = (string.IsNullOrEmpty(prefix) ? "" : prefix + " ") + s.TrimEnd();
        s += " | " + deviceId;
        Debug.Log(s);
    }

    #region int	
    
    public static int ToInt(this System.Enum enumValue) => System.Convert.ToInt32(enumValue);
    public static ControllerEnum ToControllerEnum(this int i) => (ControllerEnum)i;
    public static int ToRawMidiCommand(int command, int channel) => channel + command;
    public static int ToMidiCommand(this int i) => i & 0xF0;
    public static int ToMidiChannel(this int i) => i & 0x0F;
    public static int ToAttack(this float volume) => (int)(Mathf.Clamp01(volume) * 127);
    public static float ToVolume(this int attack) => Mathf.Clamp(attack, 0, 127) / 127f;
    public static Color ToMidiColor(this int i) => MidiConversion.GetToneColorFromMidiIndex(i);
    public static bool IsToneON(this int i) => (ToMidiCommand(i) == (int)CommandEnum.MIDI_NOTE_ON);
    public static bool IsToneOFF(this int i) => (ToMidiCommand(i) == (int)CommandEnum.MIDI_NOTE_OFF);
    public static int ShiftL(this int i, int bits) => i << bits;
    public static int ShiftR(this int i, int bits) => i >> bits;
    public static int WriteBit(this int i, byte bit, bool bitValue) { if (bitValue) return (1 << bit) | i;else return i & ~(1 << bit); }
    public static bool ReadBit(this int i, byte bit) => ((1 << bit) & i) == (1 << bit);
    public static bool IsInByteRange(this int i) => (i >= 0 && i <= byte.MaxValue);
    public static bool IsInMidiRange(this int pitch) => pitch >= 0 && pitch <= 127;
    public static int Octave(this int i) => (i < 0 ? (i - 11) / 12 : i / 12) - 1;
    public static int PositionInOctave(this int i) => i < 0 ? 11 - ((-i - 1) % 12) : i % 12;
    public static bool IsInChannelRange(this int i) => i >= 0 && i <= 15;
    
    public static bool IsWhiteKey(this int i) =>
        i.BaseMidiIndex() switch
        {
            0 => true, 1 => false, 2 => true, 3 => false,
            4 => true, 5 => true, 6 => false, 7 => true,
            8 => false, 9 => true, 10 => false, 11 => true,
            _ => true
        };

    public static bool IsBlackKey(this int i) =>
        i.BaseMidiIndex() switch
        {
            0 => false, 1 => true, 2 => false, 3 => true,
            4 => false, 5 => false, 6 => true, 7 => false,
            8 => true, 9 => false, 10 => true, 11 => false,
            _ => true
        };

    public static int BaseMidiIndex(this int i) => MidiConversion.BaseMidiIndex(i);
    public static int MidiIndex(this OctaveEnum o, int shift = 24) => ((int) (o == OctaveEnum.None ? OctaveEnum.Octave4 : o) * 12) + shift;
    
    public static int PrevWhiteKey(this int i) => (--i).IsWhiteKey() ? i : PrevWhiteKey(i);
    public static int NextWhiteKey(this int i) => (++i).IsWhiteKey() ? i : NextWhiteKey(i);
    public static int PrevBlackKey(this int i) => (--i).IsBlackKey() ? i : PrevBlackKey(i);
    public static int NextBlackKey(this int i) => (++i).IsBlackKey() ? i : NextBlackKey(i);
    
    #endregion

    #region byte	
    
    public static byte ToRawMidiCommand(byte command, byte channel) => (byte)(channel + command);
    public static ControllerEnum ToControllerEnum(this byte i) => (ControllerEnum)i;
    public static byte ToMidiCommand(this byte i) => (byte)(i & 0xF0);
    public static byte ToMidiChannel(this byte i) => (byte)(i & 0x0F);
    public static Color ToMidiColor(this byte i) => MidiConversion.GetToneColorFromMidiIndex(i);
    public static bool IsToneON(this byte i) => (ToMidiCommand(i) == (byte)CommandEnum.MIDI_NOTE_ON);
    public static bool IsToneOFF(this byte i) => (ToMidiCommand(i) == (byte)CommandEnum.MIDI_NOTE_OFF);
    public static bool IsControlChange(this byte i) => (ToMidiCommand(i) == (byte)CommandEnum.MIDI_CONTROL_CHANGE);
    public static byte ShiftL(this byte i, int bits) => (byte)(i << bits);
    public static byte ShiftR(this byte i, int bits) => (byte)(i >> bits);
    public static bool IsInByteRange(this byte i) => (i >= 0 && i <= byte.MaxValue);
    public static bool IsInMidiRange(this byte pitch) => pitch >= 0 && pitch <= 127;
    public static byte Octave(this byte i) => (byte)((i < 0 ? (i - 11) / 12 : i / 12) - 1);
    public static byte PositionInOctave(this byte i) => (byte)(i < 0 ? 11 - ((-i - 1) % 12) : i % 12);
    public static bool IsInChannelRange(this byte i) => i >= 0 && i <= 15;
    public static bool IsWhiteKey(this byte i) =>
        i.BaseMidiIndex() switch
        {
            0 => true, 1 => false, 2 => true, 3 => false,
            4 => true, 5 => true, 6 => false, 7 => true,
            8 => false, 9 => true, 10 => false, 11 => true,
            _ => true
        };

    public static bool IsBlackKey(this byte i) =>
        i.BaseMidiIndex() switch
        {
            0 => false, 1 => true, 2 => false, 3 => true,
            4 => false, 5 => false, 6 => true, 7 => false,
            8 => true, 9 => false, 10 => true, 11 => false,
            _ => true
        };

    public static byte BaseMidiIndex(this byte i) => (byte)MidiConversion.BaseMidiIndex(i);
    public static byte PrevWhiteKey(this byte i) => (--i).IsWhiteKey() ? i : PrevWhiteKey(i);
    public static byte NextWhiteKey(this byte i) => (++i).IsWhiteKey() ? i : NextWhiteKey(i);
    public static byte PrevBlackKey(this byte i) => (--i).IsBlackKey() ? i : PrevBlackKey(i);
    public static byte NextBlackKey(this byte i) => (++i).IsBlackKey() ? i : NextBlackKey(i);
    
   #endregion
}