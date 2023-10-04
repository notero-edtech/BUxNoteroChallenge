/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Play note.")]
	public class MidiOutNoteON : FsmStateAction
	{
		[Tooltip ("Midi Id from 0 to 127. Middle C or C4 has midi index 60.")]
		public FsmInt id;
		[Tooltip ("You can specify how long will the notes be played before Finished event is called.")]
		public FsmFloat duration;
		[Tooltip ("Sets midi out messages volume.")]
		[HasFloatSlider (0, 1)]
		public FsmFloat volume;
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
						
		float countDown = 0f;

		public override void Reset ()
		{
			id = new FsmInt{ UseVariable = true };
			noteEnum = NoteEnum.None;
			duration = new FsmFloat{ UseVariable = true };
			accidentalEnum = AccidentalEnum.None;
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
			octaveEnum = OctaveEnum.Octave4;
			volume = new FsmFloat{ UseVariable = true };
		}

		public override void OnEnter ()
		{
			
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
				if (!id.IsNone) {
					MidiPlayMakerInput.singleton.NoteOn ((byte)id.Value, (byte)(Mathf.RoundToInt ((volume.IsNone ? 1f : volume.Value) * 127)));
				} else {
					if (noteEnum != NoteEnum.None) {
						MidiPlayMakerInput.singleton.NoteOn (noteEnum, accidentalEnum, octaveEnum, (byte)(Mathf.RoundToInt ((volume.IsNone ? 1f : volume.Value) * 127)));	
					}
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
}