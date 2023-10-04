/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Mute chord notes.")]
	public class MidiOutChordOFF : FsmStateAction
	{
		[Tooltip ("Semicolon separated notes. For example the input can look like : 'C2;E3#;A4bb'")]
		public FsmString chord;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;
		
		string[] parsedString;

		public override void Reset ()
		{
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
			chord = "";
		}

		public override void OnEnter ()
		{
			parsedString = chord.IsNone ? new string[0] : chord.Value.Split (new char[] { ';' });
			
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
				for (int i = 0; i < parsedString.Length; i++) {
					MidiPlayMakerInput.singleton.NoteOff (MidiConversion.NoteToMidiIndex (parsedString [i]));
				} 
			}
			Finish ();
		}
	}
}