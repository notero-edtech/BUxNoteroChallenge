/* Marek Ledvina © Foriero s.r.o. 2022 */
package com.foriero.midiunifiedplugin;

import android.Manifest;
import android.app.Activity;
import android.app.Fragment;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.ParcelUuid;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.atomic.AtomicBoolean;

import android.media.midi.*;
import android.bluetooth.*;
import android.bluetooth.le.*;

/**
 * Created by marekledvina on 6/13/17.
 */

public class MIDIUnifiedFragment extends Fragment {
    
    public MIDIUnifiedFragment() { _isInitialized = false; }

    public synchronized static MIDIUnifiedFragment Init() {

        if (theInstance != null) {
            Log.e(LogTag, "MIDIUnifiedFragment already exists!");
            return theInstance;
        }

        if (UnityPlayer.currentActivity.getPackageManager().hasSystemFeature(PackageManager.FEATURE_MIDI)) {
            Log.d(LogTag, "MIDI Supported. Initializing MIDIUnifiedFragment.");
            theInstance = new MIDIUnifiedFragment();
            UnityPlayer.currentActivity.getFragmentManager().beginTransaction().add(theInstance, "MIDIUnifiedFragment").commit();
            isInitialized = theInstance.Init(UnityPlayer.currentActivity);
        } else {
            Log.e(LogTag, "MIDI NOT Supported.");
        }

        return theInstance;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // Retain between configuration changes (like device rotation) //
        setRetainInstance(true);
    }

    @Override
    public void onDestroy() {

        if (_isInitialized) {
            theInstance.DeInit();
        } else {
            Log.e(LogTag, "Unity MIDI plugin not initialized.");
        }
        super.onDestroy();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) {
        if (grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            //DoSomething.
        }
    }

    @Override
    public void onPause() {
        super.onPause();

        if(mBluetoothAdapter == null) return;
        //scanLeDevice(false);
        //mLeDeviceListAdapter.clear();
    }

    @Override
    public void onResume() {
        super.onResume();

        if(mBluetoothAdapter == null) return;
        // Ensures Bluetooth is enabled on the device.  If Bluetooth is not currently enabled,
        // fire an intent to display a dialog asking the user to grant permission to enable it.
        // if (!mBluetoothAdapter.isEnabled()) {
        //     if (!mBluetoothAdapter.isEnabled()) {
        //         Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
        //         startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        //     }
        // }

        // Initializes list view adapter.
        // mLeDeviceListAdapter = new LeDeviceListAdapter();
        // setListAdapter(mLeDeviceListAdapter);
        // scanLeDevice(true);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        // User chose not to enable Bluetooth.
        if (requestCode == REQUEST_ENABLE_BT && resultCode == Activity.RESULT_CANCELED) { return; }

        super.onActivityResult(requestCode, resultCode, data);
    }

    public static MIDIUnifiedFragment theInstance = null;
    public static boolean isInitialized = false;

    private static List<MIDIInHelper> inputs = new ArrayList<MIDIInHelper>();
    private static List<MIDIOutHelper> outputs = new ArrayList<MIDIOutHelper>();

    private static void Lock() { _locked.set(true); }
    private static void Unlock() { _locked.set(false); }

    protected class MidiDeviceCallback extends MidiManager.DeviceCallback {
        @Override
        public void onDeviceAdded(final MidiDeviceInfo device) {
        }

        @Override
        public void onDeviceRemoved(final MidiDeviceInfo device) {
        }

        @Override
        public void onDeviceStatusChanged(final MidiDeviceStatus status) {
        }
    }

    public static final String LogTag = "MIDIUnified";
    private static final int REQUEST_BLUETOOTH_SCAN = 1;

    private Activity _mainActivity;
    private static boolean _isInitialized = false;
    private static AtomicBoolean _locked = new AtomicBoolean(false);
    private MidiManager m;
    private MidiDeviceCallback midiDeviceCallback;

    // BLUETOOTH //
    private static final int REQUEST_ENABLE_BT = 1;
    private static final long SCAN_PERIOD = 10000;
    private static final int PERMISSIONS_REQUEST_ACCESS_FINE_LOCATION = 9500; // arbitrary
    private BluetoothAdapter mBluetoothAdapter;
    private boolean mBluetoothScanning;
    private Handler mHandler = new Handler();
    private ArrayList<BluetoothDevice> mLeDevices = new ArrayList<BluetoothDevice>();
    private ArrayList<BluetoothMidiDeviceTracker> mOpenDevices = new ArrayList<BluetoothMidiDeviceTracker>();

    // Device scan callback.
    private ScanCallback mLeScanCallback = new ScanCallback() {
        @Override
        public void onScanResult(int callbackType, ScanResult result) {
            //if (UnityPlayer.currentActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) return;
            if(!mLeDevices.contains(result.getDevice())) {
                Log.d(LogTag, "Bluetooth Scan Result: " + result.getDevice().getName());
                mLeDevices.add(result.getDevice());
                m.openBluetoothDevice(result.getDevice(),
                        new MidiManager.OnDeviceOpenedListener() {
                            @Override
                            public void onDeviceOpened(MidiDevice device) {
                               
                            }
                        }, null);
            }
        }

        @Override
        public void onBatchScanResults(List<ScanResult> results) {
            //if (UnityPlayer.currentActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) return;
            Log.d(LogTag, "Bluetooth Batch Scan Results: " + results.size());
            for(ScanResult result: results) {
                if(!mLeDevices.contains(result.getDevice())) {
                    Log.d(LogTag, "Bluetooth Batch Scan Result: " + result.getDevice().getName());
                    mLeDevices.add(result.getDevice());
                    m.openBluetoothDevice(result.getDevice(),
                            new MidiManager.OnDeviceOpenedListener() {
                                @Override
                                public void onDeviceOpened(MidiDevice device) {

                                }
                            }, null);
                }
            }
        }

        @Override
        public void onScanFailed(int errorCode) {
            //if (UnityPlayer.currentActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) return;
            Log.e(LogTag, "Bluetooth Scan Failed.");
        }
    };

    private void startScanningLeDevices() {

        if(mBluetoothScanning == true) return;

        Log.d (LogTag, "Scanning LE Devices...");

        // Stops scanning after a pre-defined scan period.
        mHandler.postDelayed(new Runnable() {
            @Override
            public void run() {
                mBluetoothScanning = false;
                if (UnityPlayer.currentActivity.checkSelfPermission(Manifest.permission.BLUETOOTH) != PackageManager.PERMISSION_GRANTED) { return; }
                mBluetoothAdapter.getBluetoothLeScanner().stopScan(mLeScanCallback);
            }
        }, SCAN_PERIOD);

        mLeDevices.clear();
        mBluetoothScanning = true;
        ScanFilter.Builder scanFilterBuilder = new ScanFilter.Builder();
        scanFilterBuilder.setServiceUuid(new ParcelUuid(MIDIConstants.MIDI_SERVICE));
        ScanFilter scanFilter = scanFilterBuilder.build();
        ArrayList<ScanFilter> filter = new ArrayList<ScanFilter>();
        filter.add(scanFilter);

        ScanSettings.Builder scanSettingsBuilder = new ScanSettings.Builder();
        scanSettingsBuilder.setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY);
        scanSettingsBuilder.setCallbackType(ScanSettings.CALLBACK_TYPE_ALL_MATCHES);
        ScanSettings scanSettings = scanSettingsBuilder.build();

        if (UnityPlayer.currentActivity.checkSelfPermission(Manifest.permission.BLUETOOTH) != PackageManager.PERMISSION_GRANTED) { return; }
        mBluetoothAdapter.getBluetoothLeScanner().startScan(filter, scanSettings, mLeScanCallback);
    }

    private static class BluetoothMidiDeviceTracker {
        final public BluetoothDevice bluetoothDevice;
        final public MidiDevice midiDevice;
        public int inputOpenCount;
        public int outputOpenCount;

        /**
         * @param bluetoothDevice
         * @param midiDevice
         */
        public BluetoothMidiDeviceTracker(BluetoothDevice bluetoothDevice, MidiDevice midiDevice) {
            this.bluetoothDevice = bluetoothDevice;
            this.midiDevice = midiDevice;
        }

        @Override
        public int hashCode() {
            return midiDevice.getInfo().hashCode();
        }

        @Override
        public boolean equals(Object o) {
            if (o instanceof BluetoothMidiDeviceTracker) {
                BluetoothMidiDeviceTracker other = (BluetoothMidiDeviceTracker) o;
                return midiDevice.getInfo().equals(other.midiDevice.getInfo());
            } else {
                return false;
            }
        }
    }

    public boolean Init (Activity mainActivity)
    {
        if (!mainActivity.getPackageManager().hasSystemFeature(PackageManager.FEATURE_MIDI))
        {
            Log.e (LogTag, "MIDI not supported on this device.");
            return false;
        }

        Log.d (LogTag, "MIDI supported on this device and fragment plugin is being initialized");

        _mainActivity = mainActivity;

        m = (MidiManager)mainActivity.getSystemService(Context.MIDI_SERVICE);

        if(m == null)  { Log.e (LogTag, "MIDIUnifiedFragment MidiManager is NULL!"); return false; }

        midiDeviceCallback = new MidiDeviceCallback();
        //m.registerDeviceCallback(midiDeviceCallback, new Handler(Looper.getMainLooper()));
        m.registerDeviceCallback(midiDeviceCallback, null);

        InitBluetooth(mainActivity);

        Log.d (LogTag, "MIDIUnifiedFragment MidiManager Initialized.");
        _isInitialized = true;

        return true;
    }

    private void InitBluetooth(Activity mainActivity){
        Log.d (LogTag, "InitBluetooth"); 

        int targetSdkVersion = mainActivity.getApplicationInfo().targetSdkVersion;

        // Use this check to determine whether Bluetooth classic is supported on the device.
        // Then you can selectively disable BLE-related features.
        if (!mainActivity.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH)) {
            Log.e (LogTag, "Don't have System Feature : PackageManager.FEATURE_BLUETOOTH"); return;
        }

        Log.d (LogTag, "Has System Feature : PackageManager.FEATURE_BLUETOOTH");
            
        if (mainActivity.checkSelfPermission(Manifest.permission.BLUETOOTH) != PackageManager.PERMISSION_GRANTED) {
            Log.d (LogTag, "Asking for : Manifest.permission.BLUETOOTH");
            requestPermissions(new String[] { Manifest.permission.BLUETOOTH }, 0);
        }
               
        if(mainActivity.checkSelfPermission(Manifest.permission.BLUETOOTH) == PackageManager.PERMISSION_GRANTED){

            final BluetoothManager bluetoothManager = (BluetoothManager) mainActivity.getSystemService(Context.BLUETOOTH_SERVICE);
            mBluetoothAdapter = bluetoothManager.getAdapter();

            if(!mBluetoothAdapter.isEnabled()){
                 Log.e (LogTag, "BLUETOOTH is disabled!");
                 // possibly hook on onEnabled event and then continue with scanning //
                 return;
            }
            
            if (mainActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_ADMIN) != PackageManager.PERMISSION_GRANTED) {
                Log.d (LogTag, "Asking for : Manifest.permission.BLUETOOTH_ADMIN");
                requestPermissions(new String[] { Manifest.permission.BLUETOOTH_ADMIN }, 0);
            }

            if (mainActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_PRIVILEGED) != PackageManager.PERMISSION_GRANTED) {
                Log.d (LogTag, "Asking for : Manifest.permission.BLUETOOTH_PRIVILEGED");
                requestPermissions(new String[] { Manifest.permission.BLUETOOTH_PRIVILEGED }, 0);
            }   
            

            if(targetSdkVersion > 30){
                if (mainActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) {
                    Log.d (LogTag, "Asking for : Manifest.permission.BLUETOOTH_CONNECT");
                    requestPermissions(new String[] { Manifest.permission.BLUETOOTH_CONNECT }, 0);
                }

                if (mainActivity.checkSelfPermission(Manifest.permission.BLUETOOTH_SCAN) != PackageManager.PERMISSION_GRANTED) {
                    Log.d (LogTag, "Asking for : Manifest.permission.BLUETOOTH_SCAN");
                    requestPermissions(new String[] { Manifest.permission.BLUETOOTH_SCAN }, 0);
                }                
            } else {
                if (mainActivity.checkSelfPermission(Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                    Log.d (LogTag, "Asking for : Manifest.permission.ACCESS_COARSE_LOCATION");
                    requestPermissions(new String[] { Manifest.permission.ACCESS_COARSE_LOCATION }, 0);
                }

                if(targetSdkVersion == 28 || targetSdkVersion == 29){
                    if (mainActivity.checkSelfPermission(Manifest.permission.ACCESS_BACKGROUND_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                        Log.d (LogTag, "Asking for : Manifest.permission.ACCESS_BACKGROUND_LOCATION");
                        requestPermissions(new String[] { Manifest.permission.ACCESS_BACKGROUND_LOCATION }, 0);
                    }
                }
            }            
        } else {
             Log.e (LogTag, "BLUETOOTH permission not granted!"); return;
        }

        // Use this check to determine whether BLE is supported on the device. Then
        // you can selectively disable BLE-related features.
        if (!mainActivity.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
            Log.e (LogTag, "Don't have System Feature : PackageManager.FEATURE_BLUETOOTH_LE"); return;
        }   

        Log.d (LogTag, "Has System Feature : PackageManager.FEATURE_BLUETOOTH_LE");

        startScanningLeDevices();     
    }

    public void DeInit()
    {
        if(m != null){ m.unregisterDeviceCallback(midiDeviceCallback); }

        m = null;
        midiDeviceCallback = null;
        _isInitialized = false;
    }

    private static String GetMidiDeviceName(MidiDeviceInfo device, MidiDeviceInfo.PortInfo portInfo)
    {
        String name = device.getProperties().getString(MidiDeviceInfo.PROPERTY_PRODUCT);
        if(name == null) name = device.getProperties().getString(MidiDeviceInfo.PROPERTY_NAME);
        return name + " " + (portInfo.getType() == 1 ? "OUT" : "IN");
    }

    // MIDI IN //

    public int MIDIIN_ConnectDevice(int index){

        if(!_isInitialized) return -1;

        Lock();

        int i = 0;
        int portIndex = 0;

        for (MidiDeviceInfo midiDeviceInfo:m.getDevices())
        {
            portIndex = 0;
            for(MidiDeviceInfo.PortInfo portInfo: midiDeviceInfo.getPorts()) {

                if (portInfo.getType() == MidiDeviceInfo.PortInfo.TYPE_OUTPUT) {
                    if (index == i) {
                        for (MIDIInHelper midiHelper : inputs) {
                            if (midiHelper.portInfo == portInfo) {

                                int r = midiHelper.deviceId;

                                Unlock(); return r;
                            }
                        }

                        MIDIInHelper midiHelper = new MIDIInHelper();
                        inputs.add(midiHelper);
                        midiHelper.portInfo = portInfo;
                        midiHelper.portIndex = portIndex;
                        m.openDevice(midiDeviceInfo, midiHelper, new Handler(Looper.getMainLooper()));

                        int r = midiHelper.deviceId;

                        Unlock(); return r;
                    }
                    i++;
                    portIndex++;
                }
            }
        }

        Unlock(); return -1;
    }

    public void MIDIIN_DisconnectDevice(int id){

        if(!_isInitialized) return;
        Lock();
        MIDIInHelper foundMidiHelper = null;
        for (MIDIInHelper midiHelper : inputs) {
            if(midiHelper.deviceId == id){
                foundMidiHelper = midiHelper;
                break;
            }
        }

        if(foundMidiHelper != null){
            try { foundMidiHelper.midiDevice.close(); } catch (IOException e) { Log.e(LogTag, "Could not close midi device!"); }
            inputs.remove(foundMidiHelper);
        }
        Unlock();
    }

    public void MIDIIN_DisconnectDevices ()
    {
        if(!_isInitialized) return;
        Lock();
        for (MIDIInHelper midiHelper : inputs) {
            try { midiHelper.midiDevice.close(); } catch (IOException e) { Log.e(LogTag, "Could not close midi device!"); }
        }
        inputs.clear(); Unlock();
    }

    public int MIDIIN_DeviceCount ()
    {
        if(!_isInitialized) return 0;
        Lock(); int count = 0;
        for (MidiDeviceInfo midiDeviceInfo:m.getDevices()) { count += midiDeviceInfo.getOutputPortCount(); }
        Unlock(); return count;
    }

    public String MIDIIN_DeviceName (int index)
    {
        if(!_isInitialized) return "";

        Lock();

        int i = 0;
        for (MidiDeviceInfo midiDeviceInfo : m.getDevices())
        {
            for(MidiDeviceInfo.PortInfo portInfo: midiDeviceInfo.getPorts()) {
                if (portInfo.getType() == MidiDeviceInfo.PortInfo.TYPE_OUTPUT) {
                    if (index == i) {

                        String r = GetMidiDeviceName(midiDeviceInfo,portInfo);

                        Unlock(); return r;
                    }
                    i++;
                }
            }
        }

        Unlock(); return "";
    }

    public static byte[] MidiMessage = null;

    public static int MIDIIN_PopMidiMessage ()
    {
        if(!_isInitialized) return 0;
        if(_locked.get()) return 0;

        MidiMessage = null;
        
        for (MIDIInHelper helper : inputs) {
            MidiMessage = helper.queue.poll();
            if (MidiMessage != null) { MidiMessage = byte2byte(MidiMessage, helper.deviceId); return 1; }            
        }

        return 0;
    }

    // MIDI OUT //
    public int MIDIOUT_ConnectDevice (int index)
    {
        if(!_isInitialized) return -1;

        Lock();

        int i = 0;
        int portIndex = 0;

        for (MidiDeviceInfo midiDeviceInfo:m.getDevices())
        {
            portIndex = 0;
            for(MidiDeviceInfo.PortInfo portInfo: midiDeviceInfo.getPorts()) {
                if (portInfo.getType() == MidiDeviceInfo.PortInfo.TYPE_INPUT) {
                    if (index == i) {
                        for (MIDIOutHelper midiHelper : outputs) {
                            if (midiHelper.portInfo == portInfo) {

                                int r = midiHelper.deviceId;

                                Unlock(); return r;
                            }
                        }

                        MIDIOutHelper midiHelper = new MIDIOutHelper();
                        outputs.add(midiHelper);
                        midiHelper.portIndex = portIndex;
                        midiHelper.portInfo = portInfo;
                        m.openDevice(midiDeviceInfo, midiHelper, new Handler(Looper.getMainLooper()));

                        int r = midiHelper.deviceId;
                        Unlock(); return r;
                    }
                    i++;
                    portIndex++;
                }
            }
        }

        Unlock(); return -1;
    }

    public void MIDIOUT_DisconnectDevice (int id)
    {
        if(!_isInitialized) return;
        Lock();
        MIDIOutHelper foundMidiHelper = null;
        for (MIDIOutHelper midiHelper : outputs) {
            if(midiHelper.deviceId == id){ foundMidiHelper = midiHelper; break; }
        }

        if(foundMidiHelper != null){
            try { foundMidiHelper.midiDevice.close(); } catch (IOException e) { Log.e(LogTag, "Could not close midi device!"); }
            inputs.remove(foundMidiHelper);
        }
        Unlock();
    }

    public void MIDIOUT_DisconnectDevices ()
    {
        if(!_isInitialized) return;
        Lock();
        for (MIDIOutHelper midiHelper : outputs) {
            try { midiHelper.midiDevice.close(); } catch (IOException e) { Log.e(LogTag, "Could not close midi device!"); }
        }
        inputs.clear();
        Unlock();
    }

    public String MIDIOUT_DeviceName (int index)
    {
        if(!_isInitialized) return "";
        Lock();
        int i = 0;
        for (MidiDeviceInfo midiDeviceInfo:m.getDevices())
        {
            for(MidiDeviceInfo.PortInfo portInfo: midiDeviceInfo.getPorts()) {
                if (portInfo.getType() == MidiDeviceInfo.PortInfo.TYPE_INPUT) {
                    if (index == i) {
                        String r = GetMidiDeviceName(midiDeviceInfo,portInfo);
                        Unlock(); return r;
                    }
                    i++;
                }
            }
        }
        Unlock(); return "";
    }

    public int MIDIOUT_DeviceCount ()
    {
        if(!_isInitialized) return 0;
        Lock();
        int count = 0;
        for (MidiDeviceInfo midiDeviceInfo:m.getDevices()) count += midiDeviceInfo.getInputPortCount();
        Unlock();
        return count;
    }


    public void MIDIOUT_SendData (int[] dataBuffer, int deviceId)
    {
        if(!_isInitialized) return;
        if(_locked.get()) return;
        for(MIDIOutHelper helper:outputs){ if(deviceId >=0 && deviceId != helper.deviceId) continue; helper.Send(int2byte(dataBuffer)); }
    }

    public static byte[] int2byte(int[] src)
    {
        if(src == null) return null;
        int srcLength = src.length;
        byte[] dst = new byte[srcLength];
        for (int i=0; i<srcLength; i++) { dst[i] = (byte)src[i]; }
        return dst;
    }

    public static byte[] byte2byte(byte[] src, int deviceId)
    {
        if(src == null) return null;
        int srcLength = src.length + 1;
        byte[] dst = new byte[srcLength];
        dst[0] = (byte)deviceId;
        for (int i=1; i<srcLength; i++) { dst[i] = src[i-1]; }
        return dst;
    }

    public static int[] byte2int(byte[] src, int deviceId)
    {
        if(src == null) return null;
        int srcLength = src.length + 1;
        int[] dst = new int[srcLength];
        dst[0] = deviceId;
        for (int i=1; i<srcLength; i++) { dst[i] = (int)src[i-1]; }
        return dst;
    }
}
