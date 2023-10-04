/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Set INSTRUMENT. In midi specification you can find it under PROGRAM commands.")]
	public class MidiOutSetInstrument : FsmStateAction
	{
		[Tooltip ("Set instrument'")]
		public ProgramEnum instrument;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;

		public override void Reset ()
		{
			channel = new FsmInt{ UseVariable = true };
			instrument = ProgramEnum.AcousticGrandPiano;
			channelEnum = ChannelEnum.None;
		}

		public override void OnEnter ()
		{
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
				MidiPlayMakerInput.singleton.SetInstrument (instrument.ToInt ());
				Finish ();
			} else {
				Finish ();	
			}
		}
	}
}