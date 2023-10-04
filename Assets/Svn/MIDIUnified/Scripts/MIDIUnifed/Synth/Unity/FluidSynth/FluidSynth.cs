using System.Collections;
using System.IO;
using ForieroEngine.Audio.Recording;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;

#if MIDIUNIFIED_BETA
using NFluidsynth;
using Synth = NFluidsynth.Synth;

[RequireComponent(typeof(AudioSource))]
public partial class FluidSynth : MonoBehaviour, ISynthRecorder
{
    public static FluidSynth singleton { get; private set; }

    Synth synth;
    Settings synthSettings = new Settings();
        
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
        
        synthSettings[ConfigurationKeys.SynthSampleRate].IntValue = AudioSettings.outputSampleRate / MIDISynthSettings.GetPlatformSettings().sampleRate;
        synthSettings[ConfigurationKeys.SynthCpuCores].IntValue = Mathf.Clamp(System.Environment.ProcessorCount / 2, 1, 10);
        synthSettings[ConfigurationKeys.SynthPolyphony].IntValue = 64;
        synthSettings[ConfigurationKeys.SynthGain].DoubleValue = volume;

        //synthSettings[ConfigurationKeys.SynthChorusActive].IntValue = 1;
        //synthSettings[ConfigurationKeys.SynthThreadSafeApi].IntValue = 1;

        synth = new Synth(synthSettings);

        if (!File.Exists(MIDISettings.soundFontPersistentPath))
        {
            Debug.LogError("MU | InitSynth | File does not exits : " + MIDISettings.soundFontPersistentPath);
            return;
        }
                            
        synth.LoadSoundFont(MIDISettings.soundFontPersistentPath, true);
        
        initialized = true;
    }


    void OnApplicationQuit() => CleanUp();
    void OnDestroy() => CleanUp();
    
    void CleanUp()
    {
        initialized = false;        
        synth?.Dispose(); synth = null;
        synthSettings?.Dispose(); synthSettings = null;
        singleton = null;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!initialized || !active || !isEnabled) return;

        if (allSoundsOff)
        {
            synth?.SystemReset();
            synth?.ProgramReset();

            allSoundsOff = false;
        }

        while (queue.Dequeue(ref m)) ProcessMidiMessage(m);


        if (synth == null) return;

        float[] l = new float[data.Length];
        float[] r = new float[data.Length];

        if (allSoundsOff) synth?.WriteSampleFloat(data.Length, out l, out r);
        else
        {
            synth?.WriteSampleFloat(data.Length, out l, out r);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = l[i] + r[i];
            }
        }

        if (record) fileStream?.ConverAndWrite(data);
    }

    void ProcessMidiMessage(MidiMessage message)
    {        
        switch (message.Command)
        {
            case 0x80:
                synth.NoteOff(message.Channel, message.data1);
                break;
            case 0x90:
                if (message.data2 == 0) synth.NoteOff(message.Channel, message.data1);
                else synth.NoteOn(message.Channel, message.data1, message.data2);
                break;
            case 0xA0:
                synth.KeyPressure(message.Channel, message.data1, message.data2);
                break;
            case 0xB0:
                synth.CC(message.Channel, message.data1, message.data2);
                break;
            case 0xC0:
                synth.ProgramChange(message.Channel, message.data1);
                break;
            case 0xD0:
                synth.ChannelPressure(message.Channel, message.data1);
                break;
            case 0xE0:
                synth.PitchBend(message.Channel, message.data1 + message.data2 * 0x80);
                break;
            case 0xF0:
                //synth.Sysex(new ArraySegment<byte>(msg, offset, length).ToArray(), null);
                break;
        }
    }
}
#else
[RequireComponent(typeof(AudioSource))]
public partial class FluidSynth : MonoBehaviour
{
    public static FluidSynth singleton { get; private set; }
}
#endif
