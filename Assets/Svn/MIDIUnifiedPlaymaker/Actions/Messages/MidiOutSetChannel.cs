/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Set global channel.")]
	public class MidiOutSetChannel : FsmStateAction
	{
		
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;

		public override void Reset ()
		{
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
		}

		public override void OnEnter ()
		{
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? ChannelEnum.C0 : channelEnum) : (ChannelEnum)channel.Value;
				Finish ();
			} else {
				Finish ();	
			}
		}
	}
}