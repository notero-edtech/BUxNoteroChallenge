/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {        
        public static class Interaction
        {
            public static bool waitForInput = true;
            public static readonly float waitForInputFadeOutTime = 0.1f;
            
            // each stave gets its own midi channel //
            public static Dictionary<int, bool> midiChannelInteractive = new ();
            public static Dictionary<int, bool> midiChannelSound = new ();

            public static void Init()
            {
                midiChannelInteractive = new Dictionary<int, bool>();
                for (var i = 0; i < 16; i++) midiChannelInteractive.Add(i, false);
                
                midiChannelSound = new Dictionary<int, bool>();
                for (var i = 0; i < 16; i++) midiChannelSound.Add(i, true);

                if(!MidiInput.singleton) Debug.LogError("MidiInput.singleton is NULL!!!");
            }

            public static bool IsNoteOn(int aMidiId) => MidiINPlugin.DSP.Tone[aMidiId].On;
            public static bool IsPedalOn(MIDIUnified.PedalEnum aPedal) => MidiINPlugin.DSP.ControlChange[(int)aPedal].On;
        }
    }
}
