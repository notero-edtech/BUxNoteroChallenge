package co.notero.midiplugin.midiDeviceAdapter

import android.media.midi.MidiDeviceInfo
import co.notero.midiplugin.NoteroMidiDevice
import co.notero.midiplugin.NoteroMidiInputPort

typealias DeviceAssertion = (
    midiDeviceInfo: MidiDeviceInfo,
    inputPort: NoteroMidiInputPort,
) -> NoteroMidiDevice?

class AdapterRegister {
    companion object {
        private val assertors = arrayListOf<DeviceAssertion>(
            GZUTPianoMidiDeviceAdapter.deviceAssertion,
            PopPianoMidiDeviceAdapter.deviceAssertion,
            DreamSASMidiDeviceAdapter.deviceAssertion,
            GenericPianoMidiDeviceAdapter.deviceAssertion,
        )

        fun assertDevice(
            midiDeviceInfo: MidiDeviceInfo,
            inputPort: NoteroMidiInputPort,
        ): NoteroMidiDevice? {
            for (assertor in assertors) {
                val device = assertor(midiDeviceInfo, inputPort)
                if (device != null) {
                    return device;
                }
            }
            return null
        }
    }
}