package co.notero.midiplugin

/**
 * @author Rungsikorn Rungsikavanich
 * this implementation belong to
 * https://github.com/bnkmutech/NoteroSysexTester/blob/master/app/src/main/java/com/example/midiintegration/MidiPlugin.kt
 */

import android.content.Context
import android.media.midi.MidiDeviceInfo
import android.media.midi.MidiInputPort
import android.media.midi.MidiManager
import android.os.Handler
import android.util.Log
import co.notero.midiplugin.midiDeviceAdapter.AdapterRegister

const val TAG = "NoteroMIDIPlugin"

interface OnSendMessageListener {
    fun onSend(buffer: ByteArray, offset: Int)
}

class NoteroMidiInputPort(private val inputPort: MidiInputPort) {
    var onSendMessageListeners: MutableList<OnSendMessageListener> = arrayListOf()
    val portNumber: Int get() = this.inputPort.portNumber

    fun send(msg: ByteArray, offset: Int, count: Int) {
        try {
            onSendMessageListeners.forEach { it.onSend(msg, offset) }
            inputPort.send(msg, offset, count)
        } catch (e: Exception) {
            Log.e(TAG, "send message error: ${e.message}")
        }
    }

    fun close() {
        inputPort.close()
    }
}

abstract class NoteroMidiDevice(
    private val target: MidiDeviceInfo, private val inputPort: NoteroMidiInputPort
) {
    abstract fun sendLEDControlMessage(isOn: Boolean, keyIndex: Int)
    abstract fun sendVolumeControlMessage(volumeAsPercent: Int)
    abstract fun sendMidiEvent(volumeAsPercent: ByteArray)
    open fun isInteractiveAfterMute(): Boolean = true

    fun close() {
        Log.d(TAG, "close device ${inputPort.portNumber}")
        inputPort.close()
    }

    fun getName(): Any? {
        return this.target.properties.get("name")
    }

    fun getId(): Int {
        return this.target.id
    }
}

class MidiPlugin(private val _context: Context) {
    var out: MutableList<NoteroMidiDevice> = emptyList<NoteroMidiDevice>().toMutableList()
    var isAutoManageDevice = false

    private val onSendMessageListeners: MutableList<OnSendMessageListener> = arrayListOf()

    private val deviceCallback = object : MidiManager.DeviceCallback() {
        override fun onDeviceAdded(device: MidiDeviceInfo?) {
            super.onDeviceAdded(device)
            val deviceName = device?.properties?.get("name")

            if (device == null) {
                return
            }

            Log.d(TAG, "new device added, auto discover.... $deviceName")
            discoverMidiInputPort(device)
        }

        override fun onDeviceRemoved(device: MidiDeviceInfo?) {
            super.onDeviceRemoved(device)
            val deviceName = device?.properties?.get("name")

            Log.d(TAG, "new device removed $deviceName")

            if (device == null) {
                return
            }

            closeDevice(device)
        }
    }

    fun autoManageDevice() {
        if (isAutoManageDevice) {
            Log.d(TAG, "auto manage device already active")
            return
        }

        val handler = Handler.createAsync(this._context.mainLooper)
        val midiManager: MidiManager =
            _context.getSystemService(Context.MIDI_SERVICE) as MidiManager

        midiManager.registerDeviceCallback(deviceCallback, handler)
        isAutoManageDevice = true
        discoverAllMidiInputPort()
    }

    fun setOnSendMessageListener(listener: OnSendMessageListener) {
        onSendMessageListeners.add(listener)
    }

    fun sendLEDControlMessage(isOn: Boolean, keyIndex: Int) {
        sendLEDControlMessage(isOn, keyIndex, out)
    }

    private fun sendLEDControlMessage(
        isOn: Boolean,
        keyIndex: Int,
        devices: List<NoteroMidiDevice> = out
    ) {
        devices.forEach { it.sendLEDControlMessage(isOn, keyIndex) }
    }

    fun sendVolumeControlMessage(volumeAsPercent: Int) {
        sendVolumeControlMessage(volumeAsPercent, out)
    }

    private fun sendVolumeControlMessage(
        volumeAsPercent: Int,
        devices: List<NoteroMidiDevice> = out
    ) {
        devices.forEach { it.sendVolumeControlMessage(volumeAsPercent) }
    }

    fun sendMidiEvent(data: ByteArray){
        sendMidiEvent(data, out)
    }

    private fun sendMidiEvent(data: ByteArray, devices: List<NoteroMidiDevice> = out){
        devices.forEach { it.sendMidiEvent(data) }
    }

    fun discoverAllMidiInputPort() {
        val midiManager: MidiManager =
            _context.getSystemService(Context.MIDI_SERVICE) as MidiManager
        val devices = midiManager.devices.filter { it.inputPortCount > 0 }

        for (device in devices) {
            val deviceName: String =
                device.properties.getString(MidiDeviceInfo.PROPERTY_NAME) ?: continue
            discoverMidiInputPort(deviceName)
        }

    }

    fun discoverMidiInputPort(target: MidiDeviceInfo) {
        val deviceName = target.properties.get("name")
        val midiManager: MidiManager =
            _context.getSystemService(Context.MIDI_SERVICE) as MidiManager
        val inputPortInfo: MidiDeviceInfo.PortInfo? =
            target.ports.find { it.type == MidiDeviceInfo.PortInfo.TYPE_INPUT }

        if (inputPortInfo == null) {
            Log.d(TAG, "no output port for $deviceName")
            return
        }

        val portIndex = inputPortInfo.portNumber
        Log.d(TAG, "number of ports ${target.ports.size}")

        val handler = Handler.createAsync(this._context.mainLooper)
        midiManager.openDevice(target, {
            Log.d(TAG, "Device is ready")
            val inputPort = it.openInputPort(portIndex)
            if (inputPort != null) {
                Log.d(TAG, "Input Port Connected $inputPort")
                Log.d(TAG, "$deviceName ready to send message")
                val noteroInputPort = NoteroMidiInputPort(inputPort)
                noteroInputPort.onSendMessageListeners = this.onSendMessageListeners
                val midiDeviceAdapter = AdapterRegister.assertDevice(target, noteroInputPort)
                if (midiDeviceAdapter != null) {
                    out.add(midiDeviceAdapter)
                }
            } else {
                Log.d(
                    TAG, "cannot connect to input port $portIndex, maybe you already connect to it?"
                )
            }
        }, handler)
    }

    private fun discoverMidiInputPort(deviceName: String) {
        Log.d(TAG, "get MIDI Device")
        val midiManager: MidiManager =
            _context.getSystemService(Context.MIDI_SERVICE) as MidiManager
        val devices = midiManager.devices.filter { it.inputPortCount > 0 }

        Log.d(TAG, "done")

        val target: MidiDeviceInfo? =
            devices.find { it.properties.getString(MidiDeviceInfo.PROPERTY_NAME) == deviceName }
        if (target == null) {
            Log.d(TAG, "no midi device $deviceName")
            return
        }

        discoverMidiInputPort(target)
    }

    fun isInteractiveAfterMute(name: String): Boolean {
        Log.d(TAG, "get MIDI Device")
        val midiManager: MidiManager =
            _context.getSystemService(Context.MIDI_SERVICE) as MidiManager
        val devices = midiManager.devices.filter { it.inputPortCount > 0 }
        Log.d(TAG, "done")

        val target = out.find { it.getName()?.toString()?.toLowerCase()?.startsWith(name) == true }

        if (target == null) {
            Log.d(TAG, "no midi device with NAME = $name")
            return true
        }

        return target.isInteractiveAfterMute()
    }

    fun discoverMidiInputPort(id: Int) {
        Log.d(TAG, "get MIDI Device")
        val midiManager: MidiManager =
            _context.getSystemService(Context.MIDI_SERVICE) as MidiManager
        val devices = midiManager.devices.filter { it.inputPortCount > 0 }
        Log.d(TAG, "done")

        val target = devices.find { it.id == id }

        if (target == null) {
            Log.d(TAG, "no midi device with ID = $id")
            return
        }

        discoverMidiInputPort(target)
    }

    fun closeDevice(id: Int) {
        val closeDevice = out.find { it.getId() == id } ?: return

        closeDevice.close()
        out.remove(closeDevice)
    }

    fun closeDevice(target: MidiDeviceInfo) {
        val closeDevice = out.find { it.getId() == target.id } ?: return

        closeDevice.close()
        out.remove(closeDevice)
    }

    fun closeAllMidiDevice() {
        out.forEach {
            it.close()
        }
        out = arrayListOf()
    }
}