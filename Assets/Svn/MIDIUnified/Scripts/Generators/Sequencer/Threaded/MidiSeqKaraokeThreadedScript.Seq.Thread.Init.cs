/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour, IMidiSender
{
    partial class ThreadedSequencer
    {
        partial class SequencerThread
        {          
            public byte[] bytes;
                        
            bool Init()
            {
                state = MidiState.None;
                midi.ResetForLoading();
                time.ResetForLoading();
                lyrics.ResetForLoading();

                if (bytes == null || bytes.Length == 0)
                {
                    Debug.LogError("Midi Bytes null or empty!");
                    onFinished?.Invoke(false);
                    return false;
                }
                                                              
                midi.midiFile = new MidiFile(new MemoryStream(bytes), false);
                for (int i = 0; i < midi.midiFile.Tracks; i++)
                {
                    midi.tracks.Add(midi.midiFile.Events.GetTrackEvents(i));
                }

                midi.eventPos = new int[midi.midiFile.Tracks];
                midi.endOfTrack = new bool[midi.midiFile.Tracks];
                midi.muteTrack = new bool[midi.midiFile.Tracks];
                midi.PPQN = midi.midiFile.DeltaTicksPerQuarterNote;

                #region Words, Sentences, Verses			

                IList<MidiEvent> events = new List<MidiEvent>();

                if (lyrics.lyricTrack >= 0 && lyrics.lyricTrack < midi.tracks.Count)
                {
                    events = midi.tracks[lyrics.lyricTrack].Where(e => (e is TextEvent) && e.AbsoluteTime > 0).ToList();
                }

                // bool sentence = true;
                // bool verse = true;
                bool lastWordHadSentenceSign = false;
                //string test = "";
                foreach (TextEvent e in events)
                {

                    string cmdNewVerse = "[NV]";
                    string cmdNewSentence = "[NS]";
                    string cmdNewLine = "[NL]";
                    // string cmdPhonemNone = "[P]";
                    // string cmdPhonemA = "[PA]";
                    // string cmdPhonemE = "[PE]";
                    // string cmdPhonemI = "[PI]";
                    // string cmdPhonemO = "[PO]";
                    // string cmdPhonemU = "[PU]";
                    // string cmdBridgeBegin = "[BB]";
                    // string cmdBridgeEnd = "[BE]";

                    //test += e.Text;
                    //Debug.Log (test);

                    // tohle je standardne pouzivano v karaoke midi filech, tak pro compatibilitu s nima to prevadim do naseho systemu //
                    string t = e.Text.Replace("\\", cmdNewVerse).Replace("/", cmdNewSentence);

                    if (lyrics.forceSentenceNewLine && !t.Contains("[NL]"))
                    {
                        t = t.Replace(".", ".[NL]").Replace("!", "![NL]").Replace("?", "?[NL]");
                    }

                    if (lyrics.forceCommaNewLine && !t.Contains("NL"))
                    {
                        t = t.Replace(",", ".[NL]");
                    }

                    Regex regexCommands = new Regex("\\[(.*?)\\]");

                    MatchCollection matches = regexCommands.Matches(t);

                    List<string> commands = new List<string>();

                    foreach (Match m in matches)
                    {
                        commands.Add(m.Value);
                    }

                    lyrics.words.Add(new WordText()
                    {
                        text = regexCommands.Replace(t, ""),
                        absoluteStartTime = e.AbsoluteTime,
                        deltaTime = e.DeltaTime,
                        newSentence = commands.Contains(cmdNewSentence),
                        newVerse = commands.Contains(cmdNewVerse),
                        commands = commands
                    });

                    if (commands.Contains(cmdNewVerse) || commands.Contains(cmdNewSentence) || (lyrics.forceSentences && lastWordHadSentenceSign))
                    {
                        lastWordHadSentenceSign = false;
                        lyrics.sentences.Add(new SentenceText()
                        {
                            text = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), ""),
                            absoluteStartTime = e.AbsoluteTime,
                            newVerse = commands.Contains(cmdNewVerse),
                            commands = commands
                        });
                    }
                    else
                    {
                        if (lyrics.sentences.Count > 0)
                        {
                            string sentenceHelper = "";
                            if (lyrics.sentences.Last().text.EndsWith("\n", StringComparison.Ordinal))
                            {
                                sentenceHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "").TrimStart(' ');
                            }
                            else if (lyrics.sentences.Last().text.EndsWith("\n" + " ", StringComparison.Ordinal))
                            {
                                lyrics.sentences.Last().text = lyrics.sentences.Last().text.TrimEnd(' ');
                                sentenceHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                            }
                            else
                            {
                                sentenceHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                            }
                            lyrics.sentences.Last().text += sentenceHelper;
                        }
                    }

                    if (commands.Contains(cmdNewVerse))
                    {
                        lyrics.verses.Add(new MidiText()
                        {
                            text = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), ""),
                            absoluteStartTime = e.AbsoluteTime,
                            commands = commands
                        });
                    }
                    else
                    {
                        if (lyrics.verses.Count > 0)
                        {
                            string verseHelper = "";
                            if (lyrics.verses.Last().text.EndsWith("\n", StringComparison.Ordinal))
                            {
                                verseHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "").TrimStart(' ');
                            }
                            else if (lyrics.verses.Last().text.EndsWith("\n" + " ", StringComparison.Ordinal))
                            {
                                lyrics.verses.Last().text = lyrics.verses.Last().text.TrimEnd(' ');
                                verseHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                            }
                            else
                            {
                                verseHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                            }

                            lyrics.verses.Last().text += verseHelper;
                        }
                    }

                    lastWordHadSentenceSign = t.Contains(".") || t.Contains("!") || t.Contains("?");
                }
                #endregion

                while (state != MidiState.Finished)
                {
                    UpdateBars();
                }

                state = MidiState.None;
                midi.ResetForPlaying();
                time.ResetForPlaying();

                onFinished?.Invoke(true);
                return true;                
            }
        }
    }
}
