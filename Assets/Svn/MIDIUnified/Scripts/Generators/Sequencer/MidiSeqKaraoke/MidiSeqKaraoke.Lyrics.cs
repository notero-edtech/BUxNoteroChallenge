using System.Collections.Generic;
using ForieroEngine.MIDIUnified;

public partial class MidiSeqKaraoke : IMidiSender
{
    public class MidiText
    {
        public string text;
        public long absoluteStartTime;
        public long absoluteEndTime;
        public int deltaTime;
        public bool finishFired;
        public bool finishOffsetFired;
        public List<string> commands;
    }

    public class WordText : MidiText
    {
        public bool newSentence;
        public bool newVerse;
    }

    public class SentenceText : MidiText
    {
        public bool newVerse;
    }

    public int lyricTrack { get; set; } = 1;
    public int wordPos { get; private set; } = 0;
    public int wordOffsetPos { get; private set; } = 0;
    public List<WordText> words { get; private set; } = new List<WordText>();
    public float wordTimeOffset { get; set; } = 0f;
    public float wordTimeFinishedOffset { get; set; } = 0f;

     // Will force each sentence ending to be really a sentence. Sometimes there is no [NS] tag in midi file. //
    public bool forceSentences { get; set; } = false;

    public bool forceSentenceNewLine { get; set; } = false;
    public bool forceCommaNewLine { get; set; } = false;
    public int sentencePos { get; set; } = 0;
    public List<SentenceText> sentences { get; private set; }= new List<SentenceText>();
    public float senteceTimeOffset { get; set; } = 0f;

    public int versePos { get; private set; } = 0;
    public List<MidiText> verses { get; private set; } = new List<MidiText>();
    public float versetTimeOffset { get; set; } = 0f;
}
