/* Copyright © Marek Ledvina, Foriero s.r.o. */
using ForieroEngine.MIDIUnified;
using ForieroEngine.Settings;
using UnityEngine;

public partial class MIDISettings : Settings<MIDISettings>
{
   [System.Serializable]
   public class MidiInstrumentsSettings
   {
       public bool initialize = true;
       public ProgramEnum[] instruments = new ProgramEnum[16] {
           ProgramEnum.None, ProgramEnum.None, ProgramEnum.None, ProgramEnum.None,
           ProgramEnum.None, ProgramEnum.None, ProgramEnum.None, ProgramEnum.None,
           ProgramEnum.None, ProgramEnum.None, ProgramEnum.None, ProgramEnum.None,
           ProgramEnum.None, ProgramEnum.None, ProgramEnum.None, ProgramEnum.None
       };
   }

   [System.Serializable]
   public class MidiInstrumentSettings
   {
       public bool active = true;
       [Tooltip("Evaluate input.")]
       public bool evaluate = false;
   }
    
    [System.Serializable]
    public class MidiInputSettings
    {
        public bool initialize = true;

        public bool active = true;
        [Tooltip("Evaluate input.")]
        public bool evaluate = false;
        [Tooltip("Messages will be queued in non-blocking queue and proceeded on separate thread.")]
        public bool threaded = true;
        [Tooltip("Queued Messages thread sleeping time in milliseconds.")]
        [Range(1, 10)] public int sleep = 2;
        [Tooltip("Sending midi messages directly to outputs once they are received.")]
        public bool through = false;
        [Tooltip("Sending midi messages directly to Synth once they are received.")]
        public bool synth = false;
        public bool midiOut = true;

        public enum UpdateEnum
        {
            Update,
            LateUpdate,
            FixedUpdate
        }
        
        [Tooltip("Update loop on which to proceed the messages.")]
        public UpdateEnum update = UpdateEnum.Update;

        public bool useCustomVolume = false;
        [Tooltip("This value overrides volume data so you won't be able to hear pressed key dynamics.")]
        [Range(0, 1)] public float customVolume = 1f;
        [Tooltip("This value multiplies volume data to make it softer or louder.")]
        [Range(0, 10)] public float multiplyVolume = 1f;
        public ChannelEnum midiChannel = ChannelEnum.None;
        public bool cleanBuffer = true;

        [Header("Log")]
        public bool logAll = false;
        [Tooltip("1000_0000 : 1110_1111")]
        public bool logShortMessages = false;
        [Tooltip("11111_0000 : 1111_1111")]
        public bool logSystemMessages = false;
    }
    
    [System.Serializable]
    public class MidiOutputSettings
    {
        public bool active = true;

        [Tooltip("Messages will be queued in non-blocking queue and proceeded on separate thread.")]
        public bool threaded = true;
        [Tooltip("Queued Messages thread sleeping time in milliseconds.")]
        [Range(1, 10)] public int sleep = 2;
        public bool synth = true;
        
        [Header("Log")]
        public bool logAll = false;
        [Tooltip("1000_0000 : 1110_1111")]
        public bool logShortMessages = false;
        [Tooltip("11111_0000 : 1111_1111")]
        public bool logSystemMessages = false;
    }
        
    [System.Serializable]
    public class MidiKeyboardInputSettings
    {
        public bool initialize = true;

        public bool active = true;
        [Tooltip("Evaluate input.")]
        public bool evaluate = false;
        public bool synth = false;
        public bool midiOut = true;
        
        public int keyboardOctave = 4;
        public bool updateKeyboardOctave = false;
        public bool muteTonesWhenChangingOctave = false;
        [Range(0, 1)]
        public float customVolume = 1f;

        public ChannelEnum midiChannel = ChannelEnum.None;

        public enum KeyboardInputType
        {
            ABCDEFG,
            QUERTY, 
            Custom,
            Undefined = int.MaxValue
        }

        public KeyboardInputType keyboardInputType = KeyboardInputType.QUERTY;
        public MidiKeyboardInputBinding keyboardInputBinding;
    }
       
    [System.Serializable]
    public class MidiPlaymakerInputSettings
    {
        public bool initialize = true;

        public bool active = true;
        [Tooltip("Evaluate input.")]
        public bool evaluate = false;
        public bool synth = false;
        public bool midiOut = true;

        public ChannelEnum midiChannel = ChannelEnum.None;
        public bool useCustomVolume = false;
        public float customVolume = 1f;
    }
}
