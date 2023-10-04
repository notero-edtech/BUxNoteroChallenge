using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using ForieroEngine.MIDIUnified.Midi;
using UnityEngine;
#if FMOD
using FMODUnity;
#endif

public partial class MidiSeqKaraoke : IMidiSender
{
    private bool InitializeMidiFile()
    {
        tracks = new List<IList<MidiEvent>>();

        eventPos = new int[0];
        endOfTrack = new bool[0];
        muteTrack = new bool[0];

        words = new List<WordText>();
        wordPos = 0;
        wordOffsetPos = 0;
        sentences = new List<SentenceText>();
        sentencePos = 0;
        verses = new List<MidiText>();
        versePos = 0;
        MidiFile = null;
        bar = 0;
        barCount = 0;
        bars = new List<Bar>();
        State = MidiSeqStates.None;
        fractionalTicks = 0;
        deltaTimeNumerator = 0f;
        deltaTimeRest = 0f;
        time = 0f;
        ticks = 0f;
        duration = 0f;
        lastDeltaTime = 0f;
        lastDeltaTicks = 0f;

        if (MidiBytes == null) return false;

        SetSyncContext();

        if (MidiBytes.Length > 0)
        {
            MidiFile = new MidiFile(new MemoryStream(MidiBytes), false);
            for (int i = 0; i < MidiFile.Tracks; i++)
            {
                tracks.Add(MidiFile.Events.GetTrackEvents(i));
            }

            eventPos = new int[MidiFile.Tracks];
            endOfTrack = new bool[MidiFile.Tracks];
            muteTrack = new bool[MidiFile.Tracks];
            PPQN = MidiFile.DeltaTicksPerQuarterNote;

            #region Words, Sentences, Verses			

            IList<MidiEvent> events = new List<MidiEvent>();

            if(lyricTrack >= 0 && lyricTrack < tracks.Count){
                events = tracks[lyricTrack].Where(e => (e is TextEvent) && e.AbsoluteTime > 0).ToList();
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

                if (forceSentenceNewLine && !t.Contains("[NL]"))
                {
                    t = t.Replace(".", ".[NL]").Replace("!", "![NL]").Replace("?", "?[NL]");
                }

                if (forceCommaNewLine && !t.Contains("NL"))
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

                words.Add(new WordText()
                {
                    text = regexCommands.Replace(t, ""),
                    absoluteStartTime = e.AbsoluteTime,
                    deltaTime = e.DeltaTime,
                    newSentence = commands.Contains(cmdNewSentence),
                    newVerse = commands.Contains(cmdNewVerse),
                    commands = commands
                });

                if (commands.Contains(cmdNewVerse) || commands.Contains(cmdNewSentence) || (forceSentences && lastWordHadSentenceSign))
                {
                    lastWordHadSentenceSign = false;
                    sentences.Add(new SentenceText()
                    {
                        text = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), ""),
                        absoluteStartTime = e.AbsoluteTime,
                        newVerse = commands.Contains(cmdNewVerse),
                        commands = commands
                    });
                }
                else
                {
                    if (sentences.Count > 0)
                    {
                        string sentenceHelper = "";
                        if (sentences.Last().text.EndsWith("\n"))
                        {
                            sentenceHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "").TrimStart(' ');
                        }
                        else if (sentences.Last().text.EndsWith("\n" + " "))
                        {
                            sentences.Last().text = sentences.Last().text.TrimEnd(' ');
                            sentenceHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                        }
                        else
                        {
                            sentenceHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                        }
                        sentences.Last().text += sentenceHelper;
                    }
                }

                if (commands.Contains(cmdNewVerse))
                {
                    verses.Add(new MidiText()
                    {
                        text = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), ""),
                        absoluteStartTime = e.AbsoluteTime,
                        commands = commands
                    });
                }
                else
                {
                    if (verses.Count > 0)
                    {
                        string verseHelper = "";
						if (verses.Last().text.EndsWith("\n"))
                        {
                            verseHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "").TrimStart(' ');
                        }
                        else if (verses.Last().text.EndsWith("\n" + " "))
                        {
                            verses.Last().text = verses.Last().text.TrimEnd(' ');
                            verseHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                        }
                        else
                        {
                            verseHelper = regexCommands.Replace(t.Replace(cmdNewLine, "\n"), "");
                        }

                        verses.Last().text += verseHelper;
                    }
                }

                lastWordHadSentenceSign = t.Contains(".") || t.Contains("!") || t.Contains("?");
            }
            #endregion

            while (State != MidiSeqStates.Finished)
            {
                UpdateBars();
            }

            barCount = bars.Count;
            time = 0;
            ticks = 0;
            bar = 0;
            fractionalTicks = 0;
            deltaTimeNumerator = 0f;
            deltaTimeRest = 0f;
            lastDeltaTime = 0f;
            lastDeltaTicks = 0f;

            eventPos = new int[MidiFile.Tracks];
            endOfTrack = new bool[MidiFile.Tracks];
            muteTrack = new bool[MidiFile.Tracks];

            State = MidiSeqStates.None;

            return true;
        }
        return false;
    }

    public void SetMusicSource(IAudioSource s) => MusicInterface.AudioInterface = s;
    public void SetVocalsSource(IAudioSource s) => VocalsInterface.AudioInterface = s;

    public void Initialize(TextAsset midiTextAsset)
    {
        if (midiTextAsset) Initialize(midiTextAsset.bytes);
    }

    public void Initialize(byte[] midiBytes)
    {
        initialized = false;
        Stop();
        InitializeCommon(midiBytes);
    }
    
    public void Initialize(TextAsset midiTextAsset, AudioClip vClip, AudioClip mClip)
    {
        initialized = false;
        Stop();
        if (midiTextAsset) { Initialize(midiTextAsset.bytes, vClip, mClip); }
    }

    #if FMOD
    public void Initialize(byte[] midiRawBytes, EventReference vClipEventReference, EventReference mEventReference)
    {
        initialized = false;
        Stop();

        MusicInterface.SetClip(mEventReference);
        MusicInterface.SetVolume(musicVolume);
        
        VocalsInterface.SetClip(vClipEventReference);
        VocalsInterface.SetVolume(vocalsVolume);
        
        InitializeCommon(midiRawBytes);
    }
    #endif
    
    public void Initialize(byte[] midiRawBytes, string vClipId, string mClipId)
    {
        initialized = false;
        Stop();
#if WWISE
        MusicInterface.SetClip(mClipId);
        MusicInterface.SetVolume(musicVolume);
        
        VocalsInterface.SetClip(vClipId);
        VocalsInterface.SetVolume(vocalsVolume);
        
        InitializeCommon(midiRawBytes);
#endif
    }
    
    public void Initialize(byte[] midiRawBytes, AudioClip vClip, AudioClip mClip)
    {
        initialized = false;
        Stop();

        this.musicClip = musicClip;
        MusicInterface.SetClip(mClip);
        MusicInterface.SetVolume(musicVolume);

        this.vocalsClip = vocalsClip;
        VocalsInterface.SetClip(vClip);
        VocalsInterface.SetVolume(vocalsVolume);
        
        InitializeCommon(midiRawBytes);
    }

    void InitializeCommon(byte[] midiRawBytes)
    {
        this.MidiBytes = midiRawBytes;
        initialized = InitializeMidiFile();
        ResetSequencer();
        SetSpeed(speed);
        if (initialized) OnMidiLoaded?.Invoke();
        OnInitialized?.Invoke();
    }
}
