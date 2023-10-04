using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using UnityEngine;
using Object = UnityEngine.Object;

public class MidiSenderMonitor : MonoBehaviour
{
	[RestrictInterface (typeof(IMidiSender))]
	public Object midiGenerator;

	public bool log = false;
	public string logName = "";
	public Color logColor = Color.white; 
	private IMidiSender MidiSender => midiGenerator as IMidiSender;
	private MidiEvents _midiEvents = new MidiEvents();
	
	private void Awake()
	{
		_midiEvents.AddSender(MidiSender);
		_midiEvents.log = log;
		_midiEvents.OnLog += s =>
		{
			if (log) Debug.Log($"<color=#{logColor.ToHex().Substring(0, 6)}>{logName} | {s}</color>");
		};
	}

	private void OnDestroy() { _midiEvents.RemoveAllSenders(); }

	private void Update() { _midiEvents.log = log; }
}
