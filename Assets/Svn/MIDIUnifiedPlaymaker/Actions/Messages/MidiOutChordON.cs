/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Play chord notes.")]
	public class MidiOutChordON : FsmStateAction
	{
		[Tooltip ("Semicolon separated notes. For example the input can look like : 'C2;E3#;A4bb'")]
		public FsmString chord;
		[Tooltip ("You can specify how long will the notes be played before Finished event is called.")]
		public FsmFloat duration;
		[Tooltip ("Sets midi out messages volume.")]
		[HasFloatSlider (0, 1)]
		public FsmFloat volume;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;
		
		float countDown = 0f;
		
		string[] parsedString;

		public override void Reset ()
		{
			duration = new FsmFloat{ UseVariable = true };
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
			volume = new FsmFloat{ UseVariable = true };
			chord = "";
		}

		public override void OnEnter ()
		{
			parsedString = chord.IsNone ? new string[0] : chord.Value.Split (new char[] { ';' });
			
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
				for (int i = 0; i < parsedString.Length; i++) {
					MidiPlayMakerInput.singleton.NoteOn (MidiConversion.NoteToMidiIndex (parsedString [i]), (byte)(Mathf.RoundToInt ((volume.IsNone ? 1f : volume.Value) * 127)));
				} 
			
				if (duration.IsNone)
					Finish ();
				else
					countDown = duration.Value;
			} else {
				Finish ();	
			}
		}

		public override void OnUpdate ()
		{
			if (countDown > 0f) {
				countDown -= Time.deltaTime;	
			} else {
				if (MidiPlayMakerInput.singleton != null) {
					MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
					for (int i = 0; i < parsedString.Length; i++) {
						MidiPlayMakerInput.singleton.NoteOff (MidiConversion.NoteToMidiIndex (parsedString [i]));
					} 
					Finish ();	
				} else {
					Finish ();	
				}
			}
		}
	}
}