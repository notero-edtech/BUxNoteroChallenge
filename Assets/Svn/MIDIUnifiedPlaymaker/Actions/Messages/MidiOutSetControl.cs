/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Set CONTROL value. For example : Pan, Virato etc.")]
	public class MidiOutSetControl : FsmStateAction
	{
		[Tooltip ("Control command. If you use controlEnum command instead, control has to be set to 'None'.")]
		public FsmInt control;
		[Tooltip ("Control command. If you use controlEnum command instead, control has to be set to 'None'.")]
		public ControllerEnum controlEnum;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;
		[Tooltip ("Set control value.")]
		public FsmInt controlValue;

		public override void Reset ()
		{
			control = new FsmInt{ UseVariable = true };
			controlEnum = ControllerEnum.None;
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
			controlValue = 0;
		}

		public override void OnEnter ()
		{
			if (MidiPlayMakerInput.singleton != null) {
				MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? MIDISettings.instance.playmakerSettings.midiChannel : channelEnum) : (ChannelEnum)channel.Value;
				if (!control.IsNone) {
					MidiPlayMakerInput.singleton.SendControl ((byte)control.Value, controlValue.IsNone ? (byte)0 : (byte)controlValue.Value);
				} else {
					if (controlEnum != ControllerEnum.None) {
						MidiPlayMakerInput.singleton.SendControl (controlEnum, controlValue.IsNone ? (byte)0 : (byte)controlValue.Value);		
					}	
				}
				Finish ();
			} else {
				Finish ();	
			}
		}
	}
}