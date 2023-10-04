#if MIDIUNIFIED_BETA
using ForieroEngine.Collections.NonBlocking;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using ForieroEngine.Extensions;
using UnityEngine;

public partial class TinySoundFontSynth : MonoBehaviour
{
    static Synth.Settings settings;

    static volatile bool initialized = false;
	static volatile bool active = true;
    static volatile bool isEnabled = false;
	static volatile bool allSoundsOff = false;

    static NonBlockingQueue<MidiMessage> queue = new NonBlockingQueue<MidiMessage>();

    static volatile float _volume = 0.8f;
    public float volume
    {
        get { return _volume; }
        set {
            if (!Mathf.Approximately(_volume, value))
            {
                //if(synth != null ) synth.GlobalGainDb = value.ToDecibel();
            }

            _volume = value;
        }
    }

	public static void StartSynthesizer(Synth.Settings settings)
	{
		if (singleton)
		{
			Debug.LogError("C# Synth already in scene!");
			return;
		}
               
		GameObject go = new GameObject("TinySoundFontSynth");
		singleton = go.AddComponent<TinySoundFontSynth>();
	}

    public static void StopSynthesizer()
    {
        if (singleton) Destroy(singleton.gameObject);       
    }

	public static void SendShortMessage(byte Command, byte Data1, byte Data2)
	{
        if (!singleton) return;
		queue.Enqueue(new MidiMessage() { command = Command, data1 = Data1, data2 = Data2 });		
	}

	public static void AllSoundOff()
	{
    	allSoundsOff = true;		
	}
}
#endif
