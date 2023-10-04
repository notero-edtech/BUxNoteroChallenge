/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {
            void Update()
            {
                if (_state != MidiState.Playing) return;
                
                cancelUpdate = false;
                midiFinished = false;
                musicFinished = false;
                vocalsFinished = false;

                switch (synchronizationContext)
                {
                    case SynchronizationContext.Music:
//                        time.deltaTime = GetMusicTime() - time.lastTime;
//                        musicFinished = GetMusicState() != StateAudioSource.Playing;
                        break;
                    case SynchronizationContext.Vocal:
//                        time.deltaTime = GetVocalsTime() - time.lastTime;
//                        vocalsFinished = GetMusicState() != StateAudioSource.Playing;
                        break;
                    case SynchronizationContext.Midi:
                        time.dspTime += (AudioSettings.dspTime - time.lastDspTime) * speed;
                        time.deltaTime = (time.dspTime - time.lastTime);
                        time.lastDspTime = AudioSettings.dspTime;
                        break;
                }

                if (time.deltaTime <= 0) return;

                time.deltaTimeNumerator = time.deltaTime + time.deltaTimeRest;

                time.deltaTimeIterator = (int)Math.Floor(time.deltaTimeNumerator / time.deltaTimeResolution);

                if (time.deltaTimeIterator <= 0) return;

                for (int k = 0; k < time.deltaTimeIterator; k++)
                {
                    if (cancelUpdate) break;

                    if (CallEvents()) UpdateTime();

                    if (midiFinished) break;
                }

                if ((midiFinished && musicFinished && vocalsFinished) || time.totalTime <= time.time)
                {
                    state = MidiState.Finished;
                    return;
                }

                time.deltaTimeRest = time.deltaTimeNumerator % time.deltaTimeResolution;
            }
        }
    }
}
