/* Copyright © Marek Ledvina, Foriero s.r.o. */

using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeScript : MonoBehaviour, IMidiSender
{
    private void Update()
    {
        if(_synchronizationContext != synchronizationContext) _synchronizationContext = Midi.synchronizationContext = synchronizationContext;
        if (!Mathf.Approximately(_playingDelay, playingDelay)) { _playingDelay = Midi.playingDelay = playingDelay; }
        
        if (!Mathf.Approximately(_musicVolume, musicVolume))
        {
            _musicVolume = Midi.musicVolume = musicVolume;
            Midi.MusicInterface.SetVolume(musicVolume);
        }
        
        if(_music != music) _music = Midi.music = music;

        if (!Mathf.Approximately(_vocalsVolume, vocalsVolume))
        {
            _vocalsVolume = Midi.vocalsVolume = vocalsVolume;
            Midi.VocalsInterface.SetVolume(vocalsVolume);
        }
        
        if(_vocals != vocals) _vocals = Midi.vocals = vocals;
        if (!Mathf.Approximately(_eventsOffset, eventsOffset)) { _eventsOffset = Midi.eventsOffset = eventsOffset; }
        if(!Mathf.Approximately(_speed, speed)) { _speed = speed;Midi.SetSpeed(speed); }
        if (_semitone != semitone) { _semitone = semitone; Midi.SetSemitone(semitone); }
        if (_midiOut != midiOut) Midi.midiOut = _midiOut = midiOut;
        if (_midiThrough != midiThrough) Midi.midiThrough = _midiThrough = midiThrough;
        if (_metronome != metronome) Midi.metronome = _metronome = metronome;
        if (_repeatBarSelection != repeatBarSelection) Midi.repeatBarSelection = _repeatBarSelection = repeatBarSelection;
        if (_startBar != startBar) Midi.startBar = _startBar = startBar;
        if (_endBar != endBar) Midi.endBar = _endBar = endBar;
        if (_pickUpBar != pickUpBar) Midi.pickUpBar = _pickUpBar = pickUpBar;
        if (_pickUpBarOnRepeat != pickUpBarOnRepeat) Midi.pickUpBarOnRepeat = _pickUpBarOnRepeat = pickUpBarOnRepeat;
        if (_forceTrackAsChannel != forceTrackAsChannel) Midi.forceTrackAsChannel = _forceTrackAsChannel = forceTrackAsChannel;
        if (!Mathf.Approximately((float) _markerTimeThreshold, (float) markerTimeThreshold)) Midi.markerTimeThreshold = _markerTimeThreshold = markerTimeThreshold;
        
        if(_lyricTrack != lyricTrack) _lyricTrack = Midi.lyricTrack = lyricTrack;
        if(!Mathf.Approximately(_wordTimeOffset, wordTimeOffset)) _wordTimeOffset = Midi.wordTimeOffset = wordTimeOffset;
        if(!Mathf.Approximately(_wordTimeFinishedOffset, wordTimeFinishedOffset)) _wordTimeFinishedOffset = Midi.wordTimeFinishedOffset = wordTimeFinishedOffset;
        if(_forceSentences != forceSentences) _forceSentences = Midi.forceSentences = forceSentences;
        if(_forceSentenceNewLine != forceSentenceNewLine) _forceSentenceNewLine = Midi.forceSentenceNewLine = forceSentenceNewLine;
        if(_forceCommaNewLine != forceCommaNewLine) _forceCommaNewLine = Midi.forceCommaNewLine = forceCommaNewLine;
        if(!Mathf.Approximately(_sentenceTimeOffset, sentenceTimeOffset)) _sentenceTimeOffset = Midi.senteceTimeOffset = sentenceTimeOffset;
        if(!Mathf.Approximately(_versetTimeOffset, versetTimeOffset)) _versetTimeOffset = Midi.versetTimeOffset = versetTimeOffset;

        if (_PPQN != PPQN) _PPQN = Midi.PPQN = PPQN;
        
        if(update == UpdateEnum.Update) Midi.Update();
    }

    private void LateUpdate()
    {
        if(update == UpdateEnum.LateUpdate) Midi.Update();
    }

    private void FixedUpdate()
    {
        if(update == UpdateEnum.FixedUpdate) Midi.Update();
    }
}
