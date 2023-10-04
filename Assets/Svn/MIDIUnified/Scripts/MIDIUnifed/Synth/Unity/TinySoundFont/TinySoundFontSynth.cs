using System.Collections;
using System.IO;
using ForieroEngine.Audio.Recording;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;

#if MIDIUNIFIED_BETA
using Synth = NTinySoundFont.TinySoundFont;

[RequireComponent(typeof(AudioSource))]
public partial class TinySoundFontSynth : MonoBehaviour, ISynthRecorder
{
    public static TinySoundFontSynth singleton { get; private set; }

    Synth synth;
            
    AudioSource audioSource = null;
    int outputSampleRate = 44100;
    MidiMessage m;
        
    void OnDisable()
    {
        isEnabled = false;
    }

    void OnEnable()
    {
        isEnabled = true;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(this);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(this.gameObject);
        audioSource = this.GetComponent<AudioSource>();
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => MIDI.initialized);
        InitSynth();
    }

    void InitSynth()
    {
        if (initialized) return;
        if (!audioSource) return;
        if (synth != null) return;
               
        // Load bank data //
        if (!MIDI.soundFontAsset)
        {
            Debug.LogError("MU | TinySoundFont | MIDI.soundFontAsset is empty!!!");
            return;
        }

        byte[] bankData = MIDI.soundFontAsset ? MIDI.soundFontAsset.bytes : null;
        if (bankData == null)
        {
            Debug.LogError("MU | TinySoundFont | Soundfont data empty!");
            return;
        }

        synth = Synth.Load(new MemoryStream(bankData));

        //synth.OutSampleRate = AudioSettings.outputSampleRate / MIDISynthSettings.GetPlatformSettings().outputSampleRateDivider;
        switch (AudioSettings.speakerMode)
        {           
            case AudioSpeakerMode.Mono:
                //synth.OutputMode = NTinySoundFont.OutputMode.Mono;
                break;
            case AudioSpeakerMode.Stereo:
                //synth.OutputMode = NTinySoundFont.OutputMode.StereoInterleaved;
                break;
            case AudioSpeakerMode.Quad:                
            case AudioSpeakerMode.Surround:               
            case AudioSpeakerMode.Mode5point1:               
            case AudioSpeakerMode.Mode7point1:                
            case AudioSpeakerMode.Prologic:
                Debug.LogError("MU | TinySoundFont | Speaker mode not supported : " + AudioSettings.speakerMode.ToString());
                synth = null;
                return;                
        }

        if (synth == null)
        {
            Debug.LogError("MU | TinySoundFont | Could not be initialised!!!");
            return;
        }

        initialized = true;
    }


    void OnApplicationQuit() => CleanUp();
    void OnDestroy() => CleanUp();
    
    void CleanUp()
    {
        initialized = false;        
        synth = null;        
        singleton = null;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!initialized || !active || !isEnabled) return;

        if (allSoundsOff)
        {
            synth?.Reset();            
            allSoundsOff = false;
        }

        while (queue.Dequeue(ref m)) ProcessMidiMessage(m);

        if (synth == null) return;
                
        synth?.RenderFloat(data, 0, data.Length);
        
        if (record) fileStream?.ConverAndWrite(data);
    }

    void ProcessMidiMessage(MidiMessage message)
    {
        synth?.ProcessMidiMessage(message.Command, message.Channel, message.data1, message.data2);
    }
}
#else
[RequireComponent(typeof(AudioSource))]
public partial class TinySoundFontSynth : MonoBehaviour
{

}
#endif
