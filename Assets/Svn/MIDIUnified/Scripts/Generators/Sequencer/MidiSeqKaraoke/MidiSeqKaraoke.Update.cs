/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;
#if FMOD
using FMOD.Studio;
#endif

public partial class MidiSeqKaraoke : IMidiSender
{
    float lastSpeed = -1f;
    int lastSemitone = 0;

    SynchronizationContext syncContext = SynchronizationContext.Midi;
    void SetSyncContext()
    {
        switch (synchronizationContext)
        {
            case SynchronizationContext.Music:
                syncContext = MusicInterface.IsValid ? SynchronizationContext.Music : SynchronizationContext.Midi;
                break;
            case SynchronizationContext.Vocal:
                syncContext = VocalsInterface.IsValid ? SynchronizationContext.Vocal : SynchronizationContext.Midi;
                break;
            case SynchronizationContext.Midi:
                syncContext = SynchronizationContext.Midi;
                break;
            case SynchronizationContext.Manual:
                syncContext = SynchronizationContext.Manual;
                break;
        }
    }

    double DSPTime()
    {
#if FMOD
        // until I find how to get reasonably FMOD dspclock //
        return Time.timeAsDouble;
#else
        return AudioSettings.dspTime;
#endif
    }

    private double _time = 0;
    public void Update(double manualTime = 0)
    {
        if (!initialized) return;
        
        if (Math.Abs(lastSpeed - speed) > 0.0001f) SetSpeed(speed);
        
        if (lastSemitone != semitone) SetSemitone(semitone);

        if (_playingDelay > 0)
        {
            _playingDelay -= Time.deltaTime;
            if (_playingDelay > 0) return;
        }

        if (State == MidiSeqStates.Playing)
        {
            cancelUpdate = false;
            midiFinished = _time >= duration;
            musicFinished = false;
            vocalsFinished = false;

            SetSyncContext();

            bool CheckMarkerTreshold(double currentAudioTime)
            {
                if (currentAudioTime < 0 || currentAudioTime > duration) return false;
                
                if (Math.Abs(deltaTime) > markerTimeThreshold)
                {
                    lastTime = time = currentAudioTime;
                    ticks = TimeToTicks(time);
                    for (int i = 0; i < eventPos.Length; i++) eventPos[i] = GetTrackEventPosFromAbsoluteTicks(i, ticks - 100);
                    bar = GetCurrentBarFromAbsoluteTicks(ticks - 100);
                    beatCount = GetBeatCount(ticks) - 1;
                    beat = GetBeat(beatCount);
                    //deltaTime = 0.01;
                    return true;
                }

                return false;
            }
            
            switch (syncContext)
            {
                case SynchronizationContext.Music:
                    _time = MusicInterface.GetTime() - timelineStartTime;
                    deltaTime = _time - lastTime;
                    musicFinished = MusicInterface.GetState() != AudioSourceState.Playing;
                    if (CheckMarkerTreshold(_time)) return;
                    break;
                case SynchronizationContext.Vocal:
                    _time = VocalsInterface.GetTime() - timelineStartTime;
                    deltaTime = _time - lastTime;
                    vocalsFinished = VocalsInterface.GetState() != AudioSourceState.Playing;
                    if (CheckMarkerTreshold(_time)) return;
                    break;
                case SynchronizationContext.Midi:
                    dspTime += (DSPTime() - lastDspTime) * speed;
                    _time = dspTime - timelineStartTime;
                    deltaTime = (dspTime - lastTime);
                    lastDspTime = DSPTime();
                    break;
                case SynchronizationContext.Manual:
                    _time = manualTime - timelineStartTime;
                    deltaTime = _time - lastTime;
                    if (CheckMarkerTreshold(_time)) return;
                    break;
            }
            
            // checking if the time is in the midi time range //
            if (_time < 0) return;
            
            // need to add last round of deltaTime //
            if(_time >= duration)
            {
                _time = duration;
                deltaTime = _time - lastTime;
            }
            
            if (deltaTime <= 0) return;

            deltaTimeNumerator = deltaTime + deltaTimeRest;

            deltaTimeIterator = (int)Math.Floor(deltaTimeNumerator / deltaTimeResolution);

            if (deltaTimeIterator <= 0) return;

            for (int k = 0; k < deltaTimeIterator; k++)
            {
                if (cancelUpdate) break;
                if (CallEvents()) UpdateTime();
                if (midiFinished) break;
            }
            
            switch (syncContext)
            {
                case SynchronizationContext.Midi: if (midiFinished || time >= duration) { State = MidiSeqStates.Finished; return; } break;
                case SynchronizationContext.Music: if (musicFinished) { State = MidiSeqStates.Finished; return; } break;
                case SynchronizationContext.Vocal: if (vocalsFinished) { State = MidiSeqStates.Finished; return; } break;
                case SynchronizationContext.Manual: if (midiFinished) { State = MidiSeqStates.Finished; return; } break;
            }
            
            deltaTimeRest = deltaTimeNumerator % deltaTimeResolution;
        }
    }

    void UpdateTime()
    {
        deltaTime = deltaTimeResolution;

        periodResolution = PPQN * 1000f * deltaTime * MicrosecondsPerMillisecond;
        //ticksPerClock = PPQN / PPQNMinValue;
        deltaTicks = (fractionalTicks + periodResolution) / tempoTicks;
        fractionalTicks += periodResolution - deltaTicks * tempoTicks;

        if (repeatBarSelection)
        {
            barTmp = bar;

            if (beatCount != (int)((ticks + deltaTicks) / PPQN / (4f / timeSignatureDenominator)) + 1)
            {
                if (beatCount % (int)timeSignatureNumerator + 1 == 1) barTmp++;
            }

            if (barTmp > endBar + 1)
            {
                OnRepeat?.Invoke(startBar);
                cancelUpdate = true;
                SetBar(startBar, true, pickUpBarOnRepeat);
                return;
            }
        }

        if (beatCount != GetBeatCount(ticks))
        {
            beat = GetBeat(beatCount);
            if (beat == 1) bar++;
            beatCount = GetBeatCount(ticks);
            if (metronome) TimeKeeper.BeatEvent(beat, timeSignatureNumerator, timeSignatureDenominator);
        }

        time += deltaTime;
        
        // if (time < 0) time = 0;
        // if (time > duration) time = duration;
        
        ticks += deltaTicks;
        
        // if (ticks < 0) ticks = 0;
        // if (ticks > TimeToTicks(time)) ticks = TimeToTicks(time);
        
        lastTime = time;
        lastDeltaTime = deltaTime;
        lastDeltaTicks = deltaTicks;
    }

    int GetTrackEventPosFromAbsoluteTicks(int aTrackIndex, double aAbsoluteTicks)
    {
        for (int i = 0; i < tracks[aTrackIndex].Count; i++)
        {
            if (tracks[aTrackIndex][i].AbsoluteTime > aAbsoluteTicks)
            {
                return i - 1;
            }
        }
        return tracks[aTrackIndex].Count - 1;
    }

    int GetNextBarFromAbsoluteTicks(double aAbsoluteTicks)
    {
        for (int i = 0; i < bars.Count; i++) 
        { if (bars[i].ticks > aAbsoluteTicks) return i; }
        return 0;
    }
    
    int GetPrevBarFromAbsoluteTicks(double aAbsoluteTicks)
    {
        for (int i = bars.Count - 1; i >= 0; i--) 
        { if (aAbsoluteTicks < bars[i].ticks) return i; }
        return bars.Count - 1;
    }

    int GetCurrentBarFromAbsoluteTicks(double aAbsoluteTicks)
    {
        for (int i = 0; i < bars.Count; i++) 
        { if (Mathf.Approximately((float)bars[i].ticks, (float)aAbsoluteTicks) || bars[i].ticks > aAbsoluteTicks) return i; }
        return bars.Count - 1;
    }
}
