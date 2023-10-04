package co.notero.midiplugin.midiDeviceAdapter

import android.media.midi.MidiDeviceInfo
import android.util.Log
import co.notero.midiplugin.NoteroMidiDevice
import co.notero.midiplugin.NoteroMidiInputPort
import co.notero.midiplugin.TAG
import kotlin.math.roundToInt

class GZUTPianoMidiDeviceAdapter(
    target: MidiDeviceInfo,
    private val inputPort: NoteroMidiInputPort
) : NoteroMidiDevice(target, inputPort) {
    companion object {

        val deviceAssertion: DeviceAssertion = { midiDeviceInfo: MidiDeviceInfo, inputPort: NoteroMidiInputPort -> assert(midiDeviceInfo,inputPort) }

        fun assert(
            midiDeviceInfo: MidiDeviceInfo,
            inputPort: NoteroMidiInputPort,
        ): NoteroMidiDevice? {
            val deviceName = midiDeviceInfo.properties.getString("name")

            // `lowercase` is not supported by unity need to use `toLowerCase`
            if (deviceName?.toLowerCase()?.startsWith("gzu-tek") == true) {
                val p = GZUTPianoMidiDeviceAdapter(midiDeviceInfo, inputPort)
                return p;
            }
            return null;
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
        val messageLen = 5
        val buffer = ByteArray(messageLen)
        val asDecimal = (16 / 100f * volumeAsPercent) * 7.5625f
        buffer[0] = 0xF0.toByte()
        buffer[1] = 0xAF.toByte()
        buffer[2] = 0x70.toByte()
        buffer[3] = asDecimal.roundToInt().toByte()
        buffer[4] = 0xF7.toByte()
        inputPort.send(buffer, 0, messageLen)
    }

    override fun sendMidiEvent(buffer: ByteArray) {
        inputPort.send(buffer, 0, buffer.size)
    }
}
