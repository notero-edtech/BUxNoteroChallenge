/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
using System;
using ForieroEngine.MIDIUnified.Plugins;

[AddComponentMenu("MIDIUnified/Generators/MidiKeyboardInput")]
public class MidiKeyboardInput : MonoBehaviour, IMidiSender
{
    public static MidiKeyboardInput singleton;
    public static Action<MidiKeyboardInput> OnInitialized;

    public string id = "";
    public string Id => id;

    public event ShortMessageEventHandler ShortMessageEvent;

    private void Update()
    {
        if (!MIDISettings.instance.keyboardSettings.active) return;

        ProceedKeyboardInput();
    }

    private void Awake()
    {
        if (singleton != null)
        {
            Debug.Log("GENERATOR MidiKeyboardInput already in scene.");
            Destroy(this);
            return;
        }

        singleton = this;

        OnInitialized?.Invoke(this);
    }

    private void OnDestroy()
    {
        singleton = null;
    }

    #region KeyboardInput

    private enum AccidentalState { none = 0, sharp = 1, flat = -1 }
    private AccidentalState accidentalState = AccidentalState.none;

    private MidiKeyboardInputBinding.KeyBindings ABCDEFGBindings = new MidiKeyboardInputBinding.KeyBindings()
    {
        keyBindings = new MidiKeyboardInputBinding.KeyBinding[]
        {
            new() { keyCode = KeyCode.A, toneEnum = ToneEnum.A, octaveShift = 0 },
            new() { keyCode = KeyCode.B, toneEnum = ToneEnum.B, octaveShift = 0 },
            new() { keyCode = KeyCode.C, toneEnum = ToneEnum.C, octaveShift = 0 },
            new() { keyCode = KeyCode.D, toneEnum = ToneEnum.D, octaveShift = 0 },
            new() { keyCode = KeyCode.E, toneEnum = ToneEnum.E, octaveShift = 0 },
            new() { keyCode = KeyCode.F, toneEnum = ToneEnum.F, octaveShift = 0 },
            new() { keyCode = KeyCode.G, toneEnum = ToneEnum.G, octaveShift = 0 },
        }
    };

    private MidiKeyboardInputBinding.KeyBindings QUERTYBindings = new MidiKeyboardInputBinding.KeyBindings()
    {
        keyBindings = new MidiKeyboardInputBinding.KeyBinding[]
        {
            new() { keyCode = KeyCode.A, toneEnum = ToneEnum.C, octaveShift = 0 },
            new() { keyCode = KeyCode.W, toneEnum = ToneEnum.CSharpDFlat, octaveShift = 0 },
            new() { keyCode = KeyCode.S, toneEnum = ToneEnum.D, octaveShift = 0 },
            new() { keyCode = KeyCode.E, toneEnum = ToneEnum.DSharpEFlat, octaveShift = 0 },
            new() { keyCode = KeyCode.D, toneEnum = ToneEnum.E, octaveShift = 0 },
            new() { keyCode = KeyCode.F, toneEnum = ToneEnum.F, octaveShift = 0 },
            new() { keyCode = KeyCode.T, toneEnum = ToneEnum.FSharpGFlat, octaveShift = 0 },
            new() { keyCode = KeyCode.G, toneEnum = ToneEnum.G, octaveShift = 0 },
            new() { keyCode = KeyCode.Y, toneEnum = ToneEnum.GSharpAFlat, octaveShift = 0 },
            new() { keyCode = KeyCode.H, toneEnum = ToneEnum.A, octaveShift = 1 },
            new() { keyCode = KeyCode.U, toneEnum = ToneEnum.ASharpBFlat, octaveShift = 1 },
            new() { keyCode = KeyCode.J, toneEnum = ToneEnum.B, octaveShift = 1 },
            new() { keyCode = KeyCode.K, toneEnum = ToneEnum.C, octaveShift = 1 },
        }
    };

    public void MuteTones()
    {
        int startIndex = MIDISettings.instance.keyboardSettings.keyboardOctave * 12;
        MidiOut.fireMidiOutEvents = false;
        for (var i = startIndex; i < startIndex + 13; i++)
        {
            if (i >= 0 && i < byte.MaxValue / 2)
            {
                SendShortMessage(CommandEnum.MIDI_NOTE_OFF, i,
                    MidiConversion.GetMidiVolume(MIDISettings.instance.keyboardSettings.customVolume), -1);
            }
        }
        MidiOut.fireMidiOutEvents = true;
    }

    private void KeyDown(int aMidiIdx)
    {
        if (aMidiIdx is < 0 or >= byte.MaxValue / 2) return;
        
        if(Settings.active && Settings.evaluate) MidiINPlugin.DSP.ToneOn(aMidiIdx);
        
        SendShortMessage(CommandEnum.MIDI_NOTE_ON, aMidiIdx,
            MidiConversion.GetMidiVolume(MIDISettings.instance.keyboardSettings.customVolume), -1);
    }

    private MIDISettings.MidiKeyboardInputSettings Settings => MIDISettings.instance.keyboardSettings; 
    
    private void KeyUp(int aMidiIdx)
    {
        if (aMidiIdx is < 0 or >= byte.MaxValue / 2) return;
        
        if(Settings.active && Settings.evaluate) MidiINPlugin.DSP.ToneOff(aMidiIdx);
        
        SendShortMessage(CommandEnum.MIDI_NOTE_OFF, aMidiIdx,
            MidiConversion.GetMidiVolume(MIDISettings.instance.keyboardSettings.customVolume), -1);
    }

    private void SendShortMessage(CommandEnum aCommand, int aData1, int aData2, int deviceId)
    {
        ShortMessageEvent?.Invoke(
            MIDISettings.instance.keyboardSettings.midiChannel == ChannelEnum.None
                ? (int)aCommand
                : (int)MIDISettings.instance.keyboardSettings.midiChannel + (int)aCommand, aData1, aData2, deviceId
        );

        if (MIDISettings.instance.keyboardSettings.midiOut)
        {
            MidiOut.SendShortMessage(
                MIDISettings.instance.keyboardSettings.midiChannel == ChannelEnum.None
                    ? (int)aCommand
                    : (int)MIDISettings.instance.keyboardSettings.midiChannel + (int)aCommand, aData1, aData2, -1
            );
        }
    }

    private MidiKeyboardInputBinding.KeyBindings keyBindings = null;

    private void ProceedKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) { accidentalState = AccidentalState.sharp; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { accidentalState = AccidentalState.flat; }
        if (Input.GetKeyUp(KeyCode.UpArrow)) { accidentalState = AccidentalState.none; }
        if (Input.GetKeyUp(KeyCode.DownArrow)) { accidentalState = AccidentalState.none; }
        
        // Handle at Hendrix.Midi.MidiInputAdapter.cs
        
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     if (MIDISettings.instance.keyboardSettings.updateKeyboardOctave)
        //     {
        //         if (MIDISettings.instance.keyboardSettings.muteTonesWhenChangingOctave) MuteTones();
        //         MIDISettings.instance.keyboardSettings.keyboardOctave--;
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     if (MIDISettings.instance.keyboardSettings.updateKeyboardOctave)
        //     {
        //         if (MIDISettings.instance.keyboardSettings.muteTonesWhenChangingOctave) MuteTones();
        //         MIDISettings.instance.keyboardSettings.keyboardOctave++;
        //     }
        // }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            SendShortMessage(CommandEnum.MIDI_CONTROL_CHANGE, (int)PedalEnum.DamperPedal, 127, -1);
        }

        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            SendShortMessage(CommandEnum.MIDI_CONTROL_CHANGE, (int)PedalEnum.DamperPedal, 0, -1);
        }

        keyBindings = null;

        switch (MIDISettings.instance.keyboardSettings.keyboardInputType)
        {
            case MIDISettings.MidiKeyboardInputSettings.KeyboardInputType.Custom:
                if (MIDISettings.instance.keyboardSettings.keyboardInputBinding)
                    keyBindings = MIDISettings.instance.keyboardSettings.keyboardInputBinding.keyBindings;
                break;
            case MIDISettings.MidiKeyboardInputSettings.KeyboardInputType.ABCDEFG:
                keyBindings = ABCDEFGBindings;
                break;
            case MIDISettings.MidiKeyboardInputSettings.KeyboardInputType.QUERTY:
                keyBindings = QUERTYBindings;
                break;
        }

        if (keyBindings == null) return;
        
        for (var i = 0; i < keyBindings.keyBindings.Length; i++)
        {
            var keyBinding = keyBindings.keyBindings[i];
            if (Input.GetKeyDown(keyBinding.keyCode))
            {
                KeyDown(
                    ((MIDISettings.instance.keyboardSettings.keyboardOctave + keyBinding.octaveShift) * 12)
                    + (int)keyBinding.toneEnum - 3
                    + (int)accidentalState);
            }

            if (Input.GetKeyUp(keyBinding.keyCode))
            {
                KeyUp(
                    ((MIDISettings.instance.keyboardSettings.keyboardOctave + keyBinding.octaveShift) * 12)
                    + (int)keyBinding.toneEnum - 3
                    + (int)accidentalState);
            }
        }
    }

    #endregion
}