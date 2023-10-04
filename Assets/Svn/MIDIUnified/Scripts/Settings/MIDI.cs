using System.Collections;
using UnityEngine;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;

namespace ForieroEngine.MIDIUnified
{
    [RequireComponent(typeof(AudioSource))]
    public partial class MIDI : MonoBehaviour
    {
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        private void OnEnable()
        {
            if (instance)
            {
                Debug.LogError("Something is trying to add MIDIUnified into scene but it already exists!");
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                StartCoroutine(Init());
            }
        }

        private void Update()
        {
            MidiINPlugin.through = MIDISettings.instance.inputSettings.through;
            MidiINPlugin.synth = MIDISettings.instance.inputSettings.synth;
            #if UNITY_WEBGL && !UNITY_EDITOR
            MidiINPlugin.threaded = false;
            MidiOUTPlugin.threaded = false;
            Synth.threaded = false;
            #else
            MidiINPlugin.threaded = MIDISettings.instance.inputSettings.threaded;
            MidiINPlugin.sleep = MIDISettings.instance.inputSettings.sleep;
            MidiOUTPlugin.threaded = MIDISettings.instance.outputSettings.threaded;
            MidiOUTPlugin.sleep = MIDISettings.instance.outputSettings.sleep;
            
            Synth.threaded = MIDISynthSettings.instance.threaded;
            Synth.sleep = MIDISynthSettings.instance.sleep;
#endif

        }
                        
        private void OnDisable()
        {
            CleanUp();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus) MidiOut.AllSoundOff();
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}