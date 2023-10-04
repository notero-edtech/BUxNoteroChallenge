using System.Collections.Generic;
using System.Linq;
#if !UNITY_ANDROID || UNITY_EDITOR
using NAudio.Midi;
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
namespace Notero.MidiPlugin.Windows.MidiDeviceAdapter
{
    public delegate MidiDevice DeviceAssertion(MidiOutCapabilities midiDeviceInfo, MidiOut midiOut);

    public static class AdapterRegister
    {
        private static List<DeviceAssertion> m_Assertors = new List<DeviceAssertion>()
        {
            GZUTPianoMidiDeviceAdapter.DeviceAssertion,
            PopPianoMidiDeviceAdapter.DeviceAssertion,
            DreamSASMidiDeviceAdapter.DeviceAssertion,
            GenericPianoMidiDeviceAdapter.DeviceAssertion
        };

        public static MidiDevice AssertDevice(MidiOutCapabilities midiDeviceInfo, MidiOut midiOut)
        {
            return m_Assertors.Select(assertor => assertor(midiDeviceInfo, midiOut)).FirstOrDefault(device => device != null);
        }
    }
}
#endif
