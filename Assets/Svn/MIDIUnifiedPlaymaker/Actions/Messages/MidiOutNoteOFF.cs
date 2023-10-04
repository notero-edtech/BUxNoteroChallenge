/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Mute note.")]
	public class MidiOutNoteOFF : FsmStateAction
	{
		[Tooltip ("Midi Id from 0 to 127. Middle C or C4 has midi index 60.")]
		public FsmInt id;
		[Tooltip ("You can specify a note by name. ID has to be None.")]
		public NoteEnum noteEnum;
		[Tooltip ("You can specify a note accidental.")]
		public AccidentalEnum accidentalEnum;
		[Tooltip ("If you use 'noteEnum' insteead of 'ID' an octaveEnum has to be specified.")]
		public OctaveEnum octaveEnum;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;

		public override void Reset ()
		{
			id = new FsmInt{ UseVariable = true };
			noteEnum = NoteEnum.None;
			accidentalEnum = AccidentalEnum.None;
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
			octaveEnum = OctaveEnum.Octave4;
		}

		public override void OnEnter ()
		{
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
				if (!id.IsNone) {
					MidiPlayMakerInput.singleton.NoteOff ((byte)id.Value);
				} else {
					if (noteEnum != NoteEnum.None) {
						MidiPlayMakerInput.singleton.NoteOff (noteEnum, accidentalEnum, octaveEnum);	
					}
				}
				Finish ();
			} else {
				Finish ();	
			}
		}
	}
}