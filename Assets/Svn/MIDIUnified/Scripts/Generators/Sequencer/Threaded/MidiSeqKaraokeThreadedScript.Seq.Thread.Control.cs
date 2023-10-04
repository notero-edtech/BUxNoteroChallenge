/* Copyright © Marek Ledvina, Foriero s.r.o. */

using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{    
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {
            void Play(bool pickUp) { if (pickUp) PlayWithPickupBar(); else Play(); } 
            
            void Play() { }
            void PlayWithPickupBar() { }

			void Stop() { }
            void Pause() { }
            void Reset() {
                midi.ResetForPlaying();
                time.ResetForPlaying();
                lyrics.ResetForPlaying();

            //    SetMusicTime(0);
            //    SetVocalsTime(0);
            }

            void StartPickupBar()
            {
                pickupBar.Start(60 / midi.bars[midi.bar].tempo / (midi.timeSignatureDenominator / 4) / speed, midi.timeSignatureNumerator);
                state = MidiState.PickUpBar;
                //TimeKeeper.BeatEvent(timeSignatureNumerator - pickUpBarCounter, timeSignatureNumerator, timeSignatureDenominator);                
            }
        }        
    }
}
