/* Copyright © Marek Ledvina, Foriero s.r.o. */

using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {       
        partial class SequencerThread
        {
            volatile MidiState _state = MidiState.None;
            public MidiState state {
                get { return _state; }
                set {
                    if (value != _state)
                    {                        
                        _state = value;
                        switch (value)
                        {
                            case MidiState.PickUpBar: OnPickUpBar(midi.bar); break;
                            case MidiState.Finished: OnFinished(); break;
                            case MidiState.Playing: if (value == MidiState.Pausing) OnContinue(); else OnPlay(); break;
                            case MidiState.Pausing: OnPause(); break;
                        }
                    }
                }
            }

            public volatile SynchronizationContext synchronizationContext = SynchronizationContext.Midi;
                        
            public volatile bool midiOut = true;
            public volatile bool synth = true;

            bool cancelUpdate = false;
            bool midiFinished = false;
            bool musicFinished = false;
            bool vocalsFinished = false;
        }
    }
}
