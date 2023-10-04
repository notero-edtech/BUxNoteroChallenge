/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using UnityEngine;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.MIDIUnified;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public class MusicXMLSequencer : MonoBehaviour, IMidiSender
    {

        public static MusicXMLSequencer singleton;

        public string id;
        public string Id => id;
        public event ShortMessageEventHandler ShortMessageEvent;

        public TextAsset xmlFile;

        public string version = "";
        public string workNumber = "";
        public string workTitle = "";
        public string composer = "";
        public string arragedBy = "Foriero";
        public int partCount;

        public bool playOnStart = false;
        public float delay = 0f;
        public bool midiOut = false;
        public bool loaded = false;

        public MidiSequencer sequencer;

        // Use this for initialization
        void Awake()
        {
            singleton = this;
            sequencer = new MidiSequencer(44100);
            sequencer.LoadMusicXML(xmlFile.bytes);
            sequencer.ShortMessageEvent += ShortMessageEventProc;
        }

        void Start()
        {
            if (xmlFile != null)
            {
                StartCoroutine(StartWait(delay));
            }
        }

        public IEnumerator StartWait(float aDelay)
        {
            yield return new WaitForSeconds(aDelay);
            sequencer.Play();
        }

        // Update is called once per frame
        void Update()
        {
            if (sequencer != null && sequencer.isPlaying)
            {
                sequencer.ProcessFrame(Time.deltaTime);
            }
        }

        public void Play(float aDelay = 0f)
        {
            if (sequencer != null)
            {
                StartCoroutine(singleton.StartWait(aDelay));
            }
        }

        public void Stop()
        {
            if (sequencer != null)
            {
                sequencer.Stop(true);
            }
        }

        public void Pause()
        {
            if (sequencer != null)
                sequencer.Pause();
        }

        public void Continue()
        {
            if (sequencer != null)
                sequencer.Continue();
        }

        public void Load(byte[] bytes, string aWorkTitle, string aComposer)
        {
            if (sequencer != null)
            {
                sequencer.LoadMusicXML(bytes);
                loaded = true;
            }
        }

        void ShortMessageEventProc(int aCommand, int aData1, int aData2, note aNote)
        {
            ShortMessageEvent?.Invoke(aCommand, aData1, aData2, -1);
            if (midiOut) MidiOut.SendShortMessage(aCommand, aData1, aData2, -1);
        }
    }
}
