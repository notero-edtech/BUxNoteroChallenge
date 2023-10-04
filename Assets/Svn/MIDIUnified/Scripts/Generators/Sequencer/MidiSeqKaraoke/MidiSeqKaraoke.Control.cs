/* Copyright © Marek Ledvina, Foriero s.r.o. */

using System.Timers;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;
#if FMOD

#endif

public partial class MidiSeqKaraoke : IMidiSender
{
    private Timer pickupBarTimer = new Timer(); 
    void PickUpBar()
    {
        State = MidiSeqStates.PickUpBar;
        if (pickUpBarCounter > 0)
        {
            if (!onPickupBarBeginFired && OnPickupBarBegin != null)
            {
                OnPickupBarBegin?.Invoke(bar);
                onPickupBarBeginFired = true;
            }
            pickUpBarCounter--;
            TimeKeeper.BeatEvent(timeSignatureNumerator - pickUpBarCounter, timeSignatureNumerator, timeSignatureDenominator);
            
            //pickUpCoroutine =  FireAction(60 / bars[bar].tempo / (timeSignatureDenominator / 4) / speed, PickUpBar);
            if(pickupBarTimer.Enabled) pickupBarTimer.Stop();
            pickupBarTimer.Interval = 60 / bars[bar].tempo / (timeSignatureDenominator / 4) / speed;
            pickupBarTimer.Enabled = true;
            pickupBarTimer.Start();
        }
        else
        {
            CancelPickUpBarCounting();
            OnPickupBarEnd?.Invoke();

            MusicInterface.Play();
            VocalsInterface.Play();

            dspTime = time;
            lastDspTime = DSPTime();

            // wait 1 frame //
            Update();
            
            State = MidiSeqStates.Playing;
        }
    }

    public void Play() => Play(false);
    
    public void Play(bool aPickUpBar)
    {
        if(debug) Debug.Log("SEQUENCER : Play(" + aPickUpBar.ToString() + ")");

        CancelPickUpBarCounting();
        
        if (MidiFile == null) return;
        
        if (State == MidiSeqStates.Finished) ResetSequencer();
        
        pickUpBar = aPickUpBar;

        if (bars.IndexInRange(bar))
        {
            timeSignatureNumerator = bars[bar].timeSignatureNumerator;
            timeSignatureDenominator = bars[bar].timeSignatureDenominator;
        }

        _playingDelay = playingDelay;
        
        tempo = bars[bar].tempo;
        OnTempoChange?.Invoke(tempo);

        if (pickUpBar)
        {
            if(debug) Debug.Log("SEQUENCER : Calling Pickup");
            //pickUpBar = false;
            pickUpBarCounter = timeSignatureNumerator;
            State = MidiSeqStates.PickUpBar;
            onPickupBarBeginFired = false;

            PickUpBar();
        }
        else
        {
            if(debug)  Debug.Log("SEQUENCER : Calling Play");
            CancelPickUpBarCounting();

            MusicInterface.Play();
            VocalsInterface.Play();

            dspTime = time;
            lastDspTime = DSPTime();

            Update();
            
            State = MidiSeqStates.Playing;
        }
    }

    public void Continue()
    {
        if(debug)  Debug.Log("KARAOKE CONTINUE");
        MusicInterface.Play();
        VocalsInterface.Play();
        State = MidiSeqStates.Playing;
    }

    void CancelPickUpBarCounting()
    {
        pickupBarTimer.Enabled = false;
        pickupBarTimer.Stop();
    }

    public void Pause()
    {
        CancelPickUpBarCounting();
        if (MidiFile == null) return;

        MusicInterface.Pause();
        VocalsInterface.Pause();

        if (midiOut) MidiOut.AllSoundOff();
        
        State = MidiSeqStates.Pausing;
    }

    public bool SetVocals(bool v)
    {
        vocals = v;
        
        if (vocals)
        {
            if (MusicInterface.GetState() == AudioSourceState.Playing)
            {
                VocalsInterface.SetTime(MusicInterface.GetTime());
                VocalsInterface.Play();
            }
            else
            {
                if (State == MidiSeqStates.Playing)
                {
                    VocalsInterface.SetTime(time);
                    VocalsInterface.Play();
                }
            }
        }
        else
        {
            VocalsInterface.Pause();
        }

        return this.vocals;
    }

    public bool SetMusic(bool m)
    {
        music = m;
        
        if (music)
        {
            if (VocalsInterface.GetState() == AudioSourceState.Playing)
            {
                MusicInterface.SetTime(VocalsInterface.GetTime());
                MusicInterface.Play();
            }
            else
            {
                if (State == MidiSeqStates.Playing)
                {
                    MusicInterface.SetTime(time);
                    MusicInterface.Play();
                }
            }
        }
        else
        {
            MusicInterface.Pause();
        }

        return this.music;
    }

    void ResetSequencer()
    {
        ticks = 0;
        time = 0f;
        fractionalTicks = 0;
        beat = 0;
        beatCount = 0;
        bar = 0;
        lastTime = 0f;
        wordPos = 0;
        wordOffsetPos = 0;
        sentencePos = 0;
        versePos = 0;
        deltaTimeNumerator = 0f;
        deltaTimeRest = 0f;
        lastDeltaTime = 0f;
        lastDeltaTicks = 0f;
        //pickUpBar = true;

        MusicInterface.SetTime(0);
        VocalsInterface.SetTime(0);

        if (MidiFile == null)
        {
            eventPos = new int[0];
            endOfTrack = new bool[0];
            muteTrack = new bool[0];
        }
        else
        {
            eventPos = new int[MidiFile.Tracks];
            endOfTrack = new bool[MidiFile.Tracks];
            muteTrack = new bool[MidiFile.Tracks];
        }
        foreach (WordText wt in words)
        {
            wt.finishFired = false;
            wt.finishOffsetFired = false;
        }
    }

    public void Stop()
    {
        CancelPickUpBarCounting();

        if (MidiFile == null) return;
        
        State = MidiSeqStates.None;
        ResetSequencer();

        if (midiOut)
        {
            MidiOut.AllPedalsOff();
            MidiOut.AllSoundOff();
        }

        MusicInterface.Stop();
        VocalsInterface.Stop();

        if (repeatBarSelection)
        {
            SetBar(startBar, false);
        }
        
        OnStop?.Invoke();
    }

    public float SetSpeed(float speed)
    {
        MusicInterface.SetSpeed(speed);
        VocalsInterface.SetSpeed(speed);

        this.speed = this.lastSpeed = Mathf.Clamp(speed, 0f, 10f);

        return this.speed;
    }

    public int SetSemitone(int semitone)
    {
        MusicInterface.SetSemitone(semitone);
        VocalsInterface.SetSemitone(semitone);

        this.semitone = this.lastSemitone = semitone;

        if (midiOut) MidiOut.AllSoundOff();

        return this.semitone;
    }
}
