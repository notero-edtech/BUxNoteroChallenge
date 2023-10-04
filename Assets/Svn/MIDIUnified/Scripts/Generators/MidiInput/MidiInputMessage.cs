using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Plugins
{
    public struct ScheduledMidiMessage
    {
        public MidiMessage midiMessage;
        public ITimeProvider timeProvider;
        public double time;
    }

    public struct MidiMessage
    {
        public byte Command => CommandAndChannel.ToMidiCommand();
        public byte Channel => CommandAndChannel.ToMidiChannel();
        public byte CommandAndChannel;
        public byte Data1;
        public byte Data2;
        public int DataSize;
        public byte[] Data;
        public int DeviceId;
        public bool Editor;
        public bool Through;
        public bool Synth;
        public double Time;
        
        public MidiMessage(MidiMessage m)
        {
            this.CommandAndChannel = m.CommandAndChannel;
            this.Data1 = m.Data1;
            this.Data2 = m.Data2;
            this.DataSize = m.DataSize;
            this.Data = m.Data;
            this.DeviceId = m.DeviceId;
            this.Editor = m.Editor;
            this.Through = m.Through;
            this.Synth = m.Synth;
            this.Time = m.Time;
        }

        public MidiMessage(int aCommand, int aChannel, int aData1, int aData2, int aDeviceId = -1, bool anEditor = false, bool through = false, bool synth = false, double time = -1)
        {
            this.CommandAndChannel = (byte)(aCommand + aChannel);
            this.Data1 = (byte)aData1;
            this.Data2 = (byte)aData2;
            this.DataSize = 3;
            this.Data = new byte[3] { CommandAndChannel, Data1, Data2 };
            this.DeviceId = aDeviceId;
            this.Editor = anEditor;
            this.Through = through;
            this.Synth = synth;
            this.Time = time;
            
        }

        public MidiMessage(int aCommand, int aData1, int aData2, int aDeviceId = -1, bool anEditor = false, bool through = false, bool synth = false, double time = -1)
        {
            this.CommandAndChannel = (byte)aCommand;
            this.Data1 = (byte)aData1;
            this.Data2 = (byte)aData2;
            this.DataSize = 3;
            this.Data = new byte[3] { CommandAndChannel, Data1, Data2 };
            this.DeviceId = aDeviceId;
            this.Editor = anEditor;
            this.Through = through;
            this.Synth = synth;
            this.Time = time;
        }

        public void CopyData(NativeMidiMessage m)
        {
            this.CommandAndChannel = m.command;
            this.Data1 = m.data1;
            this.Data2 = m.data2;
            this.DataSize = m.dataSize;
            if (m.data == IntPtr.Zero) Data = Array.Empty<byte>();
            else
            {
                Data = new byte[m.dataSize];
                Marshal.Copy(m.data, Data, 0, m.dataSize);
            }
//          this.deviceId = m.deviceId;
//          this.synth = synth;
//          this.editor = m.editor;
//          this.dspTime = m.dspTime;
        }

        public void SetShortMessageBytes(byte[] bytes)
        {
            if (bytes.Length < 3) { Debug.LogWarning("MidiMessage: Bytes don't have size greater or equal 3 bytes!!!"); return; }

            this.CommandAndChannel = bytes[0];
            this.Data1 = bytes[1];
            this.Data2 = bytes[2];
            this.DataSize = 3;
            this.Data = new byte[3];
            Buffer.BlockCopy(bytes, 0, Data, 0, 3);
        }

        public void SetSystemMessageBytes(byte[] bytes, int size)
        {
            this.Data = new byte[size];
            this.DataSize = size;
            Buffer.BlockCopy(bytes, 0, Data, 0, size);
        }
        public byte[] GetShortMessageBytes() => new byte[3] { CommandAndChannel, Data1, Data2 };
        public override string ToString() => $"{CommandAndChannel};{Data1};{Data2}";
        public void Log(string prefix = "", string format = "X")
        {
            if (Data == null) return;
            var s = "";
            foreach (var b in Data) { s += b.ToString(format) + " "; }
            s = (string.IsNullOrEmpty(prefix) ? "" : prefix + " ") + s.TrimEnd();
            s += " | LENGTH : " + Data.Length + " | DEVICEID : " + DeviceId;
            Debug.Log(s);
        }
    }
}