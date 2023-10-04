package co.notero.midiplugin.midiDeviceAdapter

import android.media.midi.MidiDeviceInfo
import android.util.Log
import co.notero.midiplugin.NoteroMidiDevice
import co.notero.midiplugin.NoteroMidiInputPort
import co.notero.midiplugin.TAG
import kotlin.math.roundToInt

class PopPianoMidiDeviceAdapter (
    target: MidiDeviceInfo,
    private val inputPort: NoteroMidiInputPort
) : NoteroMidiDevice(target, inputPort) {
    companion object {
        val deviceAssertion: DeviceAssertion = { midiDeviceInfo: MidiDeviceInfo, inputPort: NoteroMidiInputPort -> assert(midiDeviceInfo,inputPort) }
        fun assert(
            midiDeviceInfo: MidiDeviceInfo,
            inputPort: NoteroMidiInputPort,
        ): NoteroMidiDevice? {

            // need to use `toLowerCase` due to unsupported `lowercase` in unity
            if (midiDeviceInfo.properties.getString("name")?.toLowerCase()?.trim() == "pop piano") {
                val buffer = ByteArray(15)
                Thread.sleep(300) // need explanation here why??
                buffer[0] = 0xF0.toByte()
                buffer[1] = 0x0F.toByte()
                buffer[2] = 0x33.toByte()
                buffer[3] = 0x16.toByte()
                buffer[4] = 0x0.toByte()
                buffer[5] = 0x0.toByte()
                buffer[6] = 0x0.toByte()
                buffer[7] = 0x0.toByte()
                buffer[8] = 0x0.toByte()
                buffer[9] = 0x0.toByte()
                buffer[10] = 0x0.toByte()
                buffer[11] = 0x0.toByte()
                buffer[12] = 0x0.toByte()
                buffer[13] = 0x0.toByte()
                buffer[14] = 0xF7.toByte()
                inputPort.send(buffer, 0, 15)
                val p = PopPianoMidiDeviceAdapter(midiDeviceInfo, inputPort)
                return p
            }
            return null;
        }


    }

    override fun sendLEDControlMessage(isOn: Boolean, keyIndex: Int) {
        Log.d(TAG, "Send Key ${if (isOn) "On" else "Off"} for key ${keyIndex.toByte()}")
        val stateByte = if (isOn) 17 else 0x00
        val messageLen = 8
        val buffer = ByteArray(messageLen)
        buffer[0] = 0xF0.toByte()
        buffer[1] = 0x4D.toByte()
        buffer[2] = 0x4C.toByte()
        buffer[3] = 0x4E.toByte()
        buffer[4] = 0x45.toByte()
        buffer[5] = keyIndex.toByte()
        buffer[6] = stateByte.toByte()
        buffer[7] = 128.toByte()
        buffer[7] = 0xF7.toByte()
        inputPort.send(buffer, 0, messageLen)
    }

    override fun sendVolumeControlMessage(volumeAsPercent: Int) {
        val messageLen = 5
        val buffer = ByteArray(messageLen)
        val asDecimal = (16 / 100f * volumeAsPercent) * 7.5625f;
        buffer[0] = 0xF0.toByte()
        buffer[1] = 0x5F.toByte()
        buffer[2] = 0x70.toByte()
        buffer[3] = asDecimal.roundToInt().toByte()
        buffer[4] = 0xF7.toByte()
        inputPort.send(buffer, 0, messageLen)
    }

    override fun sendMidiEvent(buffer: ByteArray) {
        inputPort.send(buffer, 0, buffer.size)
    }

}
