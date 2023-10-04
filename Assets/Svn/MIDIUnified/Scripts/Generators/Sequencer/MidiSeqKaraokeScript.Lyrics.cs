using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeScript : MonoBehaviour, IMidiSender
{
    public int lyricTrack = 1;
    int _lyricTrack = 1;
    public int wordPos => Midi.wordPos;
    public int wordOffsetPos => Midi.wordOffsetPos;
    public List<MidiSeqKaraoke.WordText> words => Midi.words;
    public float wordTimeOffset = 0f;
    float _wordTimeOffset = 0f;
    public float wordTimeFinishedOffset = 0f;
    float _wordTimeFinishedOffset = 0f;
    [Tooltip("Will force each sentence ending to be really a sentence. Sometimes there is no [NS] tag in midi file.")]
    public bool forceSentences = false;
    bool _forceSentences = false;
    public bool forceSentenceNewLine = false;
    bool _forceSentenceNewLine = false;
    public bool forceCommaNewLine = false;
    bool _forceCommaNewLine = false;
    public int sentencePos => Midi.sentencePos;
    public List<MidiSeqKaraoke.SentenceText> sentences => Midi.sentences;
    public float sentenceTimeOffset = 0f;
    float _sentenceTimeOffset = 0f;
    public int versePos => Midi.versePos;
    public List<MidiSeqKaraoke.MidiText> verses => Midi.verses;
    public float versetTimeOffset = 0f;
    float _versetTimeOffset = 0f;
}
