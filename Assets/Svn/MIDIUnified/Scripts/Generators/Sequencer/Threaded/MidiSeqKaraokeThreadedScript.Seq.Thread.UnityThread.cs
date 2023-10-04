/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Threading.Unity;
using UnityEngine;

partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {
            // calling events and functin on unity's main thread //

            #region init
            void OnInitialized(bool b) { Call(() => singleton.OnInitialized(b)); }
            void OnMidiLoaded() { Call(() => singleton.OnMidiLoaded()); }
            #endregion

            #region control
            void OnFinished() { Call(() => singleton.OnFinished()); }
            void OnPlay() { Call(() => singleton.OnPlay()); }
            void OnContinue() { Call(() => singleton.OnContinue()); }
            void OnStop() { Call(() => singleton.OnStop()); }
            void OnPause() { Call(() => singleton.OnPause()); }
            #endregion

            #region lyrics
            void OnWord(WordText w) { Call(() => singleton.OnWord(w)); }
            void OnWordFinished(){ Call(() => singleton.OnWordFinished()); }
            void OnWordOffset(WordText w) { Call(() => singleton.OnWordOffset(w)); }
            void OnWordOffsetFinished() { Call(() => singleton.OnWordFinished()); }
            void OnSentence(SentenceText s) { Call(() => singleton.OnSentence(s)); }
            // void   OnSentenceFinished;
            void OnVerse(MidiText t) { Call(() => singleton.OnVerse(t)); }
            // void   OnVerseFinished;
            #endregion

            #region bars
            void OnRepeat(int b) { Call(() => singleton.OnRepeat(b)); }
            void OnPickUpBar(int b) { Call(() => singleton.OnPickUpBar(b)); }
            void OnPickupBarBegin(int b) { Call(() => singleton.OnPickupBarBegin(b)); }
            void OnPickupBarEnd() { Call(() => singleton.OnPickupBarEnd()); }
            #endregion

            #region changes
            void OnTempoChange(double t) { Call(() => singleton.OnTempoChange((float)t)); }
            #endregion

            #region functions
            void SetMusicTime(double t) { Call(() => singleton.SetMusicTime(t)); }
            void SetVocalsTime(double t) { Call(() => singleton.SetVocalsTime(t)); }

            void PlayMusic() { Call(() => singleton.PlayMusic()); }
            void PlayVocals() { Call(() => singleton.PlayVocals()); }

            void PauseMusic() { Call(() => singleton.PauseMusic()); }
            void PauseVocals() { Call(() => singleton.PauseVocals()); }
            #endregion

            void Call(Action f) { if (threaded) MainThreadDispatcher.Run(f); else f?.Invoke(); }
        }
    }
}
