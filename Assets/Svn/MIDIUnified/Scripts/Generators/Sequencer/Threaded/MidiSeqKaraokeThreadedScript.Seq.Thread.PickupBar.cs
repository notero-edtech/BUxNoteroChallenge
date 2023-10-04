using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {
            class PickupBar
            {
                public int timeSignatureNumerator = 4;               
                public double beatTime = 0;
                
                public double lastDspTime = 0;
                public double time = 0;
                public int counter = 0;
                
                public void Start(double beatTime, int timeSignatureNumerator)
                {
                    this.lastDspTime = AudioSettings.dspTime;
                    this.time = this.beatTime = beatTime;
                    this.counter = this.timeSignatureNumerator = timeSignatureNumerator;                    
                }

                //if (pickUpBarCounter > 0)
                //{
                //    if (!onPickupBarBeginFired && OnPickupBarBegin != null)
                //    {
                //        OnPickupBarBegin(bar);
                //        onPickupBarBeginFired = true;
                //    }
                //    pickUpBarCounter--;
                //    TimeKeeper.BeatEvent(timeSignatureNumerator - pickUpBarCounter, timeSignatureNumerator, timeSignatureDenominator);
                //    pickUpCoroutine = this.FireAction(60 / bars[bar].tempo / (timeSignatureDenominator / 4) / speed, PickUpBar);
                //}
                //else
                //{
                //    CancelPickUpBarCounting();
                //    OnPickupBarEnd?.Invoke();

                //    PlayMusic();
                //    PlayVocals();

                //    dspTime = time;
                //    lastDspTime = AudioSettings.dspTime;

                //    this.FireAction(1, () =>
                //    {
                //        state = State.Playing;
                //    });
                //}
            }

            PickupBar pickupBar = new PickupBar();
            void UpdatePickuBar()
            {
                if (_state != MidiState.PickUpBar) return;
                if (pickupBar.counter <= 0) return;

                if (pickupBar.time > 0)
                {
                    var dspTime = AudioSettings.dspTime;
                    pickupBar.time -= (dspTime - pickupBar.lastDspTime);
                    pickupBar.lastDspTime = dspTime;
                    if(pickupBar.time <= 0)
                    {
                        pickupBar.counter--;
                        pickupBar.time = pickupBar.beatTime - pickupBar.time;
                        //TimeKeeper.BeatEvent(midi.timeSignatureNumerator - pickupBar.counter, midi.timeSignatureNumerator, midi.timeSignatureDenominator);
                    }
                }
               
                if(pickupBar.counter <= 0)
                {
                        //start//
                }                
            }
        }
    }       
}
