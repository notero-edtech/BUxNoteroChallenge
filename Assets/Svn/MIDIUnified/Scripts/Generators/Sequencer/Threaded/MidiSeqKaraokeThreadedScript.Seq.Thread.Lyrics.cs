using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
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

    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {
            Lyrics lyrics = new Lyrics();

            class Lyrics
            {
                public int lyricTrack = 1;
                public int wordPos = 0;
                public int wordOffsetPos = 0;
                public List<WordText> words = new List<WordText>();
                public float wordTimeOffset = 0f;
                public float wordTimeFinishedOffset = 0f;

                [Tooltip("Will force each sentence ending to be really a sentence. Sometimes there is no [NS] tag in midi file.")]
                public bool forceSentences = false;
                public bool forceSentenceNewLine = false;
                public bool forceCommaNewLine = false;
                public int sentencePos = 0;
                public List<SentenceText> sentences = new List<SentenceText>();
                public float senteceTimeOffset = 0f;

                public int versePos = 0;
                public List<MidiText> verses = new List<MidiText>();
                public float versetTimeOffset = 0f;

                public void ResetForPlaying()
                {
                    wordPos = 0;
                    wordOffsetPos = 0;
                    sentencePos = 0;
                    versePos = 0;

                    foreach (WordText wt in words)
                    {
                        wt.finishFired = false;
                        wt.finishOffsetFired = false;
                    }
                }

                public void ResetForLoading()
                {
                    ResetForPlaying();
                    words = new List<WordText>();                                     
                    sentences = new List<SentenceText>();                                        
                    verses = new List<MidiText>();                    
                }
            }
        }
    }
}
