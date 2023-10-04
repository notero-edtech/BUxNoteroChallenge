using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

public class DeviceIDExample : MonoBehaviour
{
    public string MCU1Port = "MCU1";
    public string MCU2Port = "MCU2";

    // create two MCU classes to which pass the deviceId or deviceName //
    // MCU class can not be static since we need likely even more than two //
    // So we likely need 

    void Start()
    {
        // MidiINPlugin.deviceNames //
        // MidiINPlugin.connectedDevices //

        MidiINPlugin.OnDeviceConnected +=(m) =>
        {
            Debug.Log("MIDIIN CONNECTED : " + m.name + " | " + m.deviceId);
        };

        MidiINPlugin.OnDeviceDisconnected += (m) =>
        {
            Debug.Log("MIDIIN DISCONNECTED : " + m.name + " | " + m.deviceId);
        };

        MidiOUTPlugin.OnDeviceConnected += (m) =>
        {
            Debug.Log("MIDIOUT CONNECTED : " + m.name + " | " + m.deviceId);
            MidiOut.NoteOn(60, deviceId: m.deviceId);
        };

        MidiOUTPlugin.OnDeviceDisconnected += (m) =>
        {
            Debug.Log("MIDIOUT DISCONNECTED : " + m.name + " | " + m.deviceId);
        };
    }
}
