/* Marek Ledvina © Foriero s.r.o. 2022 */
package com.foriero.midiunifiedplugin;

import android.media.midi.MidiDevice;
import android.media.midi.MidiDeviceInfo;
import android.media.midi.MidiInputPort;
import android.media.midi.MidiManager;
import android.util.Log;

import java.io.IOException;

/**
 * Created by marekledvina on 6/13/17.
 */

public class MIDIOutHelper implements  MidiManager.OnDeviceOpenedListener{
    public int deviceId = 0;
    public boolean opened = false;
    public MidiDeviceInfo.PortInfo portInfo;
    public MidiDevice midiDevice;
    public int portIndex = -1;

    private MidiInputPort midiPort = null;

    private static int deviceIdIncrement = 0;
    private static final String LogTag = "MIDIOUTHelper";

    public MIDIOutHelper(){
        deviceId = ++deviceIdIncrement;
    }

    @Override
    public void onDeviceOpened(MidiDevice device) {
        if (device == null) { Log.e(LogTag, "could not open device " + portInfo); return; }
        
        midiDevice = device;
        opened = true;
        midiPort = device.openInputPort(portIndex);        
    }

    public void Send(byte[] bytes){
        if(midiPort != null) { 
            try { midiPort.send(bytes, 0, bytes.length); } catch (IOException e) { Log.e(LogTag, e.getMessage()); }
        }
    }

    @Override
    public void finalize() {
        if(midiPort != null){            
            try { midiPort.close(); } catch (IOException e) { Log.e(LogTag, e.getMessage()); }
        }

        if(midiDevice != null){
            try { midiDevice.close(); } catch (IOException e) { Log.e(LogTag, e.getMessage()); }
        }
    }
}
