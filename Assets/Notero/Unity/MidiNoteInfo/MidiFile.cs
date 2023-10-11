using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Notero.Unity.MidiNoteInfo
{
    public class MidiFile
    {
        public readonly int Format;

        public readonly int TicksPerQuarterNote;

        public readonly MidiTrack[] Tracks;

        public readonly int TracksCount;

        public float BPM { get; private set; }
        public float TopNumberTimeSignature { get; private set; }
        public float BottomNumberTimeSignature { get; private set; }
        public float AccidentalNumber { get; private set; }
        public float Scale { get; private set; }

        private int m_MidiDuration = int.MinValue;

        public MidiFile(Stream stream)
            : this(Reader.ReadAllBytesFromStream(stream)) { }

        public MidiFile(string path)
            : this(File.ReadAllBytes(path)) { }

        public MidiFile(byte[] data)
        {
            int position = 0;

            if(Reader.ReadString(data, ref position, 4) != "MThd")
            {
                throw new FormatException("Invalid file header (expected MThd)");
            }

            if(Reader.Read32(data, ref position) != 6)
            {
                throw new FormatException("Invalid header length (expected 6)");
            }

            Format = Reader.Read16(data, ref position);
            TracksCount = Reader.Read16(data, ref position);
            TicksPerQuarterNote = Reader.Read16(data, ref position);

            if((TicksPerQuarterNote & 0x8000) != 0)
            {
                throw new FormatException("Invalid timing mode (SMPTE timecode not supported)");
            }

            Tracks = new MidiTrack[TracksCount];

            for(int i = 0; i < TracksCount; i++)
            {
                Tracks[i] = ParseTrack(i, data, ref position);
            }
        }

        public int GetMidiDuration()
        {
            Assert.IsNotNull(Tracks, $"{nameof(MidiFile)} there is no track in this file");

            if(m_MidiDuration == int.MinValue)
            {
                foreach(MidiTrack track in Tracks)
                {
                    m_MidiDuration = Math.Max(track.MidiEvents.Last().Time, m_MidiDuration);
                }
            }

            return m_MidiDuration;
        }

        public double GetMidiDurationInMilliseconds()
        {
            return MidiNoteInfoHelper.ConvertMidiTimeToMilliseconds(GetMidiDuration(), BPM, TicksPerQuarterNote);
        }

        public double GetMidiDurationInSeconds()
        {
            return MidiNoteInfoHelper.ConvertMidiTimeToSeconds(GetMidiDuration(), BPM, TicksPerQuarterNote);
        }

        private bool ParseMetaEvent(
            byte[] data,
            ref int position,
            byte metaEventType,
            ref byte data1,
            ref byte data2)
        {
            switch(metaEventType)
            {
                case (byte)MetaEventType.Tempo:
                    int mspqn = data[position + 1] << 16 | data[position + 2] << 8 | data[position + 3];
                    var quarterNoterPerMinute = 60000000.0 / mspqn;
                    data1 = (byte)quarterNoterPerMinute;
                    position += 4;
                    BPM = (float)Math.Round(quarterNoterPerMinute, 1);
                    return true;

                case (byte)MetaEventType.TimeSignature:
                    data1 = data[position + 1];
                    data2 = (byte)Math.Pow(2.0, data[position + 2]);
                    position += 5;
                    TopNumberTimeSignature = data1;
                    BottomNumberTimeSignature = data2;
                    return true;

                case (byte)MetaEventType.KeySignature:
                    data1 = data[position + 1];
                    data2 = data[position + 2];
                    AccidentalNumber = data1;
                    Scale = data2;
                    position += 3;
                    return true;

                // Ignore Other Meta Events
                default:
                    int length = Reader.ReadVarInt(data, ref position);
                    position += length;
                    return false;
            }
        }

        private MidiTrack ParseTrack(int index, byte[] data, ref int position)
        {
            if(Reader.ReadString(data, ref position, 4) != "MTrk")
            {
                throw new FormatException("Invalid track header (expected MTrk)");
            }

            int trackLength = Reader.Read32(data, ref position);
            int trackEnd = position + trackLength;

            MidiTrack track = new MidiTrack { Index = index };
            int time = 0;
            byte status = 0;

            while(position < trackEnd)
            {
                time += Reader.ReadVarInt(data, ref position);

                byte peekByte = data[position];

                // If the most significant bit is set then this is a status byte
                if((peekByte & 0x80) != 0)
                {
                    status = peekByte;
                    ++position;
                }

                // If the most significant nibble is not an 0xF this is a channel event
                if((status & 0xF0) != 0xF0)
                {
                    // Separate event type from channel into two
                    byte eventType = (byte)(status & 0xF0);
                    byte channel = (byte)((status & 0x0F) + 1);

                    byte data1 = data[position++];

                    // If the event type doesn't start with 0b110 it has two bytes of data (i.e. except 0xC0 and 0xD0)
                    byte data2 = (eventType & 0xE0) != 0xC0 ? data[position++] : (byte)0;

                    // Convert NoteOn events with 0 velocity into NoteOff events
                    if(eventType == (byte)MidiEventType.NoteOn && data2 == 0)
                    {
                        eventType = (byte)MidiEventType.NoteOff;
                    }

                    track.MidiEvents.Add(
                        new MidiEvent { Time = time, Type = eventType, Arg1 = channel, Arg2 = data1, Arg3 = data2 });
                }
                else
                {
                    if(status == 0xFF)
                    {
                        // Meta Event
                        byte metaEventType = Reader.Read8(data, ref position);

                        // There is a group of meta event types reserved for text events which we store separately
                        if(metaEventType >= 0x01 && metaEventType <= 0x0F)
                        {
                            int textLength = Reader.ReadVarInt(data, ref position);
                            string textValue = Reader.ReadString(data, ref position, textLength);
                            TextEvent textEvent = new TextEvent { Time = time, Type = metaEventType, Value = textValue };
                            track.TextEvents.Add(textEvent);
                        }
                        else
                        {
                            byte data1 = 0;
                            byte data2 = 0;

                            // We only handle the few meta events we care about and skip the rest
                            if(ParseMetaEvent(data, ref position, metaEventType, ref data1, ref data2))
                            {
                                track.MidiEvents.Add(
                                    new MidiEvent
                                    {
                                        Time = time,
                                        Type = status,
                                        Arg1 = metaEventType,
                                        Arg2 = data1,
                                        Arg3 = data2
                                    });
                            }
                        }
                    }
                    else if(status == 0xF0 || status == 0xF7)
                    {
                        // SysEx event
                        int length = Reader.ReadVarInt(data, ref position);
                        position += length;
                    }
                    else
                    {
                        ++position;
                    }
                }
            }

            return track;
        }

        private static class Reader
        {
            public static int Read16(byte[] data, ref int i)
            {
                return data[i++] << 8 | data[i++];
            }

            public static int Read32(byte[] data, ref int i)
            {
                return data[i++] << 24 | data[i++] << 16 | data[i++] << 8 | data[i++];
            }

            public static byte Read8(byte[] data, ref int i)
            {
                return data[i++];
            }

            public static byte[] ReadAllBytesFromStream(Stream input)
            {
                byte[] buffer = new byte[16 * 1024];
                using(MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    return ms.ToArray();
                }
            }

            public static string ReadString(byte[] data, ref int i, int length)
            {
                string result = Encoding.ASCII.GetString(data, i, length);
                i += length;
                return result;
            }

            public static int ReadVarInt(byte[] data, ref int i)
            {
                int result = data[i++];

                if((result & 0x80) == 0)
                {
                    return result;
                }

                result &= 0x7F;

                for(int j = 0; j < 3; j++)
                {
                    int value = data[i++];

                    result = result << 7 | value & 0x7F;

                    if((value & 0x80) == 0)
                    {
                        break;
                    }
                }

                return result;
            }
        }
    }

    public class MidiTrack
    {
        public int Index;

        public List<MidiEvent> MidiEvents = new List<MidiEvent>();

        public List<TextEvent> TextEvents = new List<TextEvent>();
    }

    public struct MidiEvent
    {
        public int Time;

        public byte Type;

        public byte Arg1;

        public byte Arg2;

        public byte Arg3;

        public MidiEventType MidiEventType => (MidiEventType)Type;

        public MetaEventType MetaEventType => (MetaEventType)Arg1;

        public int Channel => Arg1;

        // remove 1 octave to shift octave down to global format
        public int Note => Arg2 - 12;

        public int Velocity => Arg3;

        public ControlChangeType ControlChangeType => (ControlChangeType)Arg2;

        public int Value => Arg3;
    }

    public struct TextEvent
    {
        public int Time;

        public byte Type;

        public string Value;

        public TextEventType TextEventType => (TextEventType)Type;
    }

    public enum MidiEventType : byte
    {
        NoteOff = 0x80,

        NoteOn = 0x90,

        KeyAfterTouch = 0xA0,

        ControlChange = 0xB0,

        ProgramChange = 0xC0,

        ChannelAfterTouch = 0xD0,

        PitchBendChange = 0xE0,

        MetaEvent = 0xFF
    }

    public enum ControlChangeType : byte
    {
        BankSelect = 0x00,

        Modulation = 0x01,

        Volume = 0x07,

        Balance = 0x08,

        Pan = 0x0A,

        Sustain = 0x40
    }

    public enum TextEventType : byte
    {
        Text = 0x01,

        TrackName = 0x03,

        Lyric = 0x05,
    }

    public enum MetaEventType : byte
    {
        Tempo = 0x51,

        TimeSignature = 0x58,

        KeySignature = 0x59
    }
}