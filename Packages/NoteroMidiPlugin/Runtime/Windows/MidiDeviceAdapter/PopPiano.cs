#if !UNITY_ANDROID || UNITY_EDITOR
using NAudio.Midi;
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
namespace Notero.MidiPlugin.Windows.MidiDeviceAdapter
{
    public class PopPianoMidiDeviceAdapter : MidiDevice
    {
        private MidiOut m_MidiOut;

        private PopPianoMidiDeviceAdapter(MidiOutCapabilities midiOutCapabilities, MidiOut midiOut) : base(midiOutCapabilities, midiOut)
        {
            this.m_MidiOut = midiOut;
        }

        public static DeviceAssertion DeviceAssertion => Assert;

        private static MidiDevice Assert(MidiOutCapabilities midiDeviceInfo, MidiOut midiOut)
        {
            var deviceName = midiDeviceInfo.ProductName;

            return deviceName.ToLower().StartsWith("pop piano") ? new PopPianoMidiDeviceAdapter(midiDeviceInfo, midiOut) : null;
        }

        public override void SendLEDControlMessage(bool isOn, int keyIndex)
        {
            var stateByte = isOn ? 1 : 0;
            var buffer = new byte[8];

            buffer[0] = 0xF0;
            buffer[1] = 0x4D;
            buffer[2] = 0x4C;
            buffer[3] = 0x4E;
            buffer[4] = 0x45;
            buffer[5] = (byte)keyIndex;
            buffer[6] = (byte)stateByte;
            buffer[7] = 0xF7;

            m_MidiOut.SendBuffer(buffer);
        }

        public override void SendVolumeControlMessage(int volumeAsPercent)
        {
            var buffer = new byte[5];
            var asDecimal = (16 / 100f * volumeAsPercent) * 7.5625f;

            buffer[0] = 0xF0;
            buffer[1] = 0x5F;
            buffer[2] = 0x70;
            buffer[2] = (byte)asDecimal;
            buffer[2] = 0xF7;

            m_MidiOut.SendBuffer(buffer);
        }

        public override void SendMidiEvent(byte[] buffer)
        {
            m_MidiOut.SendBuffer(buffer);
        }
    }
}
#endif
