using AudioSynthesis.Synthesis;
using ForieroEngine.Collections.NonBlocking;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;

public partial class CSharpSynth : MonoBehaviour
{
    private static Synth.Settings settings;

    private static volatile bool initialized = false;
	private static volatile bool active = true;
    private static volatile bool isEnabled = false;
	private static volatile bool allSoundsOff = false;
	private static NonBlockingQueue<MidiMessage> queue = new NonBlockingQueue<MidiMessage>();
    private static volatile float _volume = 1f;

    public float volume
    {
        get => _volume;
        set
        {
            if (!Mathf.Approximately(_volume, value)) { if (_synth != null) _synth.MixGain = value; }
            _volume = value;
        }
    }

    public static void StartSynthesizer(Synth.Settings settings)
	{
		if (Instance) { Debug.LogError("C# Synth already in scene!"); return; }
        CSharpSynth.settings = settings;
		var go = new GameObject("CSharpSynth");
		Instance = go.AddComponent<CSharpSynth>();						
	}

    public static void StopSynthesizer() { if (Instance) Destroy(Instance.gameObject); }

	public static void SendShortMessage(byte Command, byte Data1, byte Data2)
	{
        if (!Instance) return;
		queue.Enqueue(new MidiMessage() { channel = Command.ToMidiChannel(), command = Command.ToMidiCommand(), data1 = Data1, data2 = Data2 });		
	}

	public static void AllSoundOff() { allSoundsOff = true; }
}
