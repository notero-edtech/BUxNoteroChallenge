package co.notero.midiplugin.midiDeviceAdapter

import android.media.midi.MidiDeviceInfo
import android.util.Log
import co.notero.midiplugin.NoteroMidiDevice
import co.notero.midiplugin.NoteroMidiInputPort
import co.notero.midiplugin.TAG


/**
 * Any general piano midi device will
 * fallback to this adapter
 * as send a default LED message
 * and no sound level control
 */
class GenericPianoMidiDeviceAdapter(
    target: MidiDeviceInfo, private val inputPort: NoteroMidiInputPort
) : NoteroMidiDevice(target, inputPort) {
    companion object {

        val deviceAssertion: DeviceAssertion =
            { midiDeviceInfo: MidiDeviceInfo, inputPort: NoteroMidiInputPort ->
                assert(
                    midiDeviceInfo, inputPort
                )
            }

        fun assert(
            midiDeviceInfo: MidiDeviceInfo,
            inputPort: NoteroMidiInputPort,
        ): NoteroMidiDevice? {
            val p = GenericPianoMidiDeviceAdapter(midiDeviceInfo, inputPort)
            return p
        }

    }

    override fun sendLEDControlMessage(isOn: Boolean, keyIndex: Int) {
        Log.d(TAG, "Send Key ${if (isOn) "On" else "Off"} for key ${keyIndex.toByte()}")
        val stateByte = if (isOn) 0x01 else 0x00
        val messageLen = 8
        val buffer = ByteArray(messageLen)
        buffer[0] = 0xF0.toByte()
        buffer[1] = 0x4D.toByte()
        buffer[2] = 0x4C.toByte()
        buffer[3] = 0x4E.toByte()
        buffer[4] = 0x45.toByte()
        buffer[5] = keyIndex.toByte()
        buffer[6] = stateByte.toByte()
        buffer[7] = 0xF7.toByte()
        inputPort.send(buffer, 0, messageLen)
    }

    override fun sendVolumeControlMessage(volumeAsPercent: Int) {

    }

    override fun sendMidiEvent(buffer: ByteArray) {
        inputPort.send(buffer, 0, buffer.size)
    }
}
