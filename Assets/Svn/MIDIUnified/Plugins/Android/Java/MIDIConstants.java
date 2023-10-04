/* Marek Ledvina © Foriero s.r.o. 2022 */
package com.foriero.midiunifiedplugin;

import java.util.UUID;

/**
 * MIDI related constants and static methods.
 * These values are defined in the MIDI Standard 1.0
 * available from the MIDI Manufacturers Association.
 */

public class MIDIConstants {
    //  Bluetooth MIDI Gatt service UUID
    public static final UUID MIDI_SERVICE = UUID.fromString("03B80E5A-EDE8-4B33-A751-6CE34EC4C700");
    // Bluetooth MIDI Gatt characteristic UUID
    public static final UUID MIDI_CHARACTERISTIC = UUID.fromString("7772E5DB-3868-4112-A1A9-F2669D106BF3");
    // Descriptor UUID for enabling characteristic changed notifications
    public static final UUID CLIENT_CHARACTERISTIC_CONFIG = UUID.fromString("00002902-0000-1000-8000-00805f9b34fb");

    protected final static String TAG = "MidiTools";
    public static final byte STATUS_COMMAND_MASK = (byte) 0xF0;
    public static final byte STATUS_CHANNEL_MASK = (byte) 0x0F;

    // Channel voice messages.
    public static final byte STATUS_NOTE_OFF = (byte) 0x80;
    public static final byte STATUS_NOTE_ON = (byte) 0x90;
    public static final byte STATUS_POLYPHONIC_AFTERTOUCH = (byte) 0xA0;
    public static final byte STATUS_CONTROL_CHANGE = (byte) 0xB0;
    public static final byte STATUS_PROGRAM_CHANGE = (byte) 0xC0;
    public static final byte STATUS_CHANNEL_PRESSURE = (byte) 0xD0;
    public static final byte STATUS_PITCH_BEND = (byte) 0xE0;

    // System Common Messages.
    public static final byte STATUS_SYSTEM_EXCLUSIVE = (byte) 0xF0;
    public static final byte STATUS_MIDI_TIME_CODE = (byte) 0xF1;
    public static final byte STATUS_SONG_POSITION = (byte) 0xF2;
    public static final byte STATUS_SONG_SELECT = (byte) 0xF3;
    public static final byte STATUS_TUNE_REQUEST = (byte) 0xF6;
    public static final byte STATUS_END_SYSEX = (byte) 0xF7;

    // System Real-Time Messages
    public static final byte STATUS_TIMING_CLOCK = (byte) 0xF8;
    public static final byte STATUS_START = (byte) 0xFA;
    public static final byte STATUS_CONTINUE = (byte) 0xFB;
    public static final byte STATUS_STOP = (byte) 0xFC;
    public static final byte STATUS_ACTIVE_SENSING = (byte) 0xFE;
    public static final byte STATUS_RESET = (byte) 0xFF;

    /** Number of bytes in a message nc from 8c to Ec */
    public final static int CHANNEL_BYTE_LENGTHS[] = { 3, 3, 3, 3, 2, 2, 3 };

    /** Number of bytes in a message Fn from F0 to FF */
    public final static int SYSTEM_BYTE_LENGTHS[] = { 1, 2, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

    public final static int MAX_CHANNELS = 16;

    /**
     * MIDI messages, except for SysEx, are 1,2 or 3 bytes long.
     * You can tell how long a MIDI message is from the first status byte.
     * Do not call this for SysEx, which has variable length.
     * @param statusByte
     * @return number of bytes in a complete message, zero if data byte passed
     */
    public static int getBytesPerMessage(byte statusByte) {
        // Java bytes are signed so we need to mask off the high bits
        // to get a value between 0 and 255.
        int statusInt = statusByte & 0xFF;
        if (statusInt >= 0xF0) {
            // System messages use low nibble for size.
            return SYSTEM_BYTE_LENGTHS[statusInt & 0x0F];
        } else if(statusInt >= 0x80) {
            // Channel voice messages use high nibble for size.
            return CHANNEL_BYTE_LENGTHS[(statusInt >> 4) - 8];
        } else {
            return 0; // data byte
        }
    }

    /**
     * @param msg
     * @param offset
     * @param count
     * @return true if the entire message is ActiveSensing commands
     */
    public static boolean isAllActiveSensing(byte[] msg, int offset,
            int count) {
        // Count bytes that are not active sensing.
        int goodBytes = 0;
        for (int i = 0; i < count; i++) {
            byte b = msg[offset + i];
            if (b != MIDIConstants.STATUS_ACTIVE_SENSING) {
                goodBytes++;
            }
        }
        return (goodBytes == 0);
    }
}
