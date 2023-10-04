/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{    
    ThreadedSequencer seq;

    partial class ThreadedSequencer
    {
        public MidiState state { get { return thread.state; } }
        public bool midiOut { get { return thread.midiOut; } set { thread.midiOut = value; } }
        public bool synth { get { return thread.synth; } set { thread.synth = value; } }
        public double speed { get { return thread.speed; } set { thread.speed = value; } }
        public int semitone { get { return thread.semitone; } set { thread.semitone = value; } }

        public bool threaded { get { return thread.threaded; } set { thread.threaded = value; } }
                                
        SequencerThread thread = new SequencerThread();

        public void Run()
        {
            thread.Run();
        }

        public void Initialize(byte[] midiBytes, Action<bool> onFinished)
        {
            if (thread.loading)
            {
                Debug.LogError("Sequencer is currently loading other file!");
                return;
            }
            
            thread.state = MidiState.None;
            thread.onFinished = onFinished;
            thread.bytes = midiBytes;

            thread.SendSignal(ThreadSignal.Init);
                        
            //singleton.SetMusicClip();
            //singleton.SetMusicVolume();

            //SetVocalsClip(vocalsClip);
            //SetVocalsVolume(vocalsVolume);
                        
            //initialized = Initialize();

            //ResetSequencer();

            //SetSpeed(speed);

            //if (initialized) OnMidiLoaded?.Invoke();
            //OnInitialized?.Invoke();
            //onFinished?.Invoke(false);
        }

        public void Play(bool aPickupBar)
        {
            //Debug.Log("SEQUENCER : Play(" + aPickUpBar.ToString() + ")");

            //CancelPickUpBarCounting();

            //if (midiFile == null)
            //{
            //    return;
            //}

            //if (state == State.Finished)
            //{
            //    ResetSequencer();
            //}

            //pickUpBar = aPickUpBar;

            //timeSignatureNumerator = bars[bar].timeSignatureNumerator;
            //timeSignatureDenominator = bars[bar].timeSignatureDenominator;

            //tempo = bars[bar].tempo;
            //OnTempoChange?.Invoke(tempo);

            //if (pickUpBar)
            //{
            //    Debug.Log("SEQUENCER : Calling Pickup");
            //    //pickUpBar = false;
            //    pickUpBarCounter = timeSignatureNumerator;
            //    state = State.PickUpBar;
            //    onPickupBarBeginFired = false;

            //    PickUpBar();
            //}
            //else
            //{
            //    Debug.Log("SEQUENCER : Calling Play");
            //    CancelPickUpBarCounting();

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

        public void Stop()
        {
            //thread.Stop();

            //if (midiOut)
            //{
            //    MidiOut.AllPedalsOff();
            //    MidiOut.AllSoundOff();
            //}

            //StopMusic();
            //StopVocals();

            //if (repeatBarSelection)
            //{
            //    SetBar(startBar, false);
            //}

            //if (OnStop != null)
            //{
            //    OnStop();
            //}
        }

        public void Pause()
        {
            //CancelPickUpBarCounting();
            //if (midiFile == null) return;

            //PauseMusic();
            //PauseVocals();

            //if (midiOut)
            //{
            //    MidiOut.AllSoundOff();
            //}

            //state = State.Pausing;
        }

        public void Continue()
        {
            //Debug.Log("KARAOKE CONTINUE");
            //PlayMusic();
            //PlayVocals();
            //state = State.Playing;
        }

        public void Reset()
        {
            //ticks = 0;
            //time = 0f;
            //fractionalTicks = 0;
            //beat = 0;
            //beatCount = 0;
            //bar = 0;
            //lastTime = 0f;
            //wordPos = 0;
            //wordOffsetPos = 0;
            //sentencePos = 0;
            //versePos = 0;
            //deltaTimeNumerator = 0f;
            //deltaTimeRest = 0f;
            //lastDeltaTime = 0f;
            //lastDeltaTicks = 0f;
            ////pickUpBar = true;

            //SetMusicTime(0);
            //SetVocalsTime(0);

            //if (midiFile == null)
            //{
            //    eventPos = new int[0];
            //    endOfTrack = new bool[0];
            //    muteTrack = new bool[0];
            //}
            //else
            //{
            //    eventPos = new int[midiFile.Tracks];
            //    endOfTrack = new bool[midiFile.Tracks];
            //    muteTrack = new bool[midiFile.Tracks];
            //}
            //foreach (WordText wt in words)
            //{
            //    wt.finishFired = false;
            //    wt.finishOffsetFired = false;
            //}
        }

        public void Terminate()
        {
            thread.Terminate();
        }
                       
        void PickUpBar()
        {
            //state = State.PickUpBar;
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
    }
}
