using System;
using ForieroEngine.MIDIUnified;
using UnityEngine;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class MidiKeyboardInputBinding : Settings<MidiKeyboardInputBinding>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/KeyboardInputBinding")] public static void KeyboardInputBindingMenu() => Select();
#endif

    [Serializable]
    public class KeyBinding
    {
        public KeyCode keyCode;
        public ToneEnum toneEnum;
        [Range(-1, 1)]
        public int octaveShift;
    }

    [Serializable]
    public class KeyBindings
    {
        public KeyBinding[] keyBindings;
    }

    public KeyBindings keyBindings;
}
