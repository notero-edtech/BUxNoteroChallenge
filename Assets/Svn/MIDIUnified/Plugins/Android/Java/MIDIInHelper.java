/* Marek Ledvina © Foriero s.r.o. 2022 */
package com.foriero.midiunifiedplugin;

import android.media.midi.MidiDevice;
import android.media.midi.MidiDeviceInfo;
import android.media.midi.MidiManager;
import android.media.midi.MidiOutputPort;
import android.media.midi.MidiReceiver;
import android.util.Log;

import java.io.IOException;
import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * Created by marekledvina on 6/13/17.
 */

public class MIDIInHelper implements  MidiManager.OnDeviceOpenedListener{
    public int deviceId = 0;
    public boolean opened = false;
    public MidiDeviceInfo.PortInfo portInfo;
    public MidiDevice midiDevice;
    public int portIndex = -1;

    private MidiOutputPort midiPort = null;
    public ConcurrentLinkedQueue<byte[]> queue = new ConcurrentLinkedQueue<byte[]>();

    private static int deviceIdIncrement = 0;
    private static final String LogTag = "MIDIINHelper";

    Receiver receiver = null;
    MIDIFrameReceiver frameReceiver = null;

    public MIDIInHelper(){
        deviceId = ++deviceIdIncrement;
        receiver = new Receiver();
        frameReceiver = new MIDIFrameReceiver(receiver);
    }

    @Override
    public void onDeviceOpened(MidiDevice device) {
        if (device == null) { Log.e(LogTag, "could not open device " + portInfo); return; }
        
        midiDevice = device;
        opened = true;
        midiPort = device.openOutputPort(portIndex);
        midiPort.connect(frameReceiver);        
    }
    
    protected class Receiver extends MidiReceiver {
        @Override
        public void onSend(byte[] msg, int offset, int count, long timestamp) throws IOException
        {
            byte[] m = new byte[count];
            for (int i=0;i<count;i++) { m[i] = msg[offset + i]; }
            //Log.e(LogTag, Arrays.toString(m));
            queue.add(m);
        }
    }

    private static int unsignedToBytes(byte b) { return b & 0xFF; }
    
    @Override
    public void finalize() {
        if(midiPort != null) {
            midiPort.disconnect(frameReceiver);
            try { midiPort.close(); } catch (IOException e) { Log.e(LogTag, e.getMessage()); }
        }

        if(midiDevice != null){
            try { midiDevice.close(); } catch (IOException e) { Log.e(LogTag, e.getMessage()); }
        }
    }
}
