/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Messages")]
	[Tooltip ("Send a raw shortmessage to opened out device(port).")]
	public class MidiOutSendMessage : FsmStateAction
	{
		[Tooltip ("Channel to be played on(0-15). If 'None' the default channel is used.")]
		public FsmInt channel;

		[RequiredField]
		[Tooltip ("Midi Command from 0 to 127")]
		public FsmInt command;

		[RequiredField]
		[Tooltip ("Midi Data 1 from 0  to 127")]
		public FsmInt data1;

		[Tooltip ("Midi Data 2 from 0 to 127")]
		public FsmInt data2;

		int
			_channel, 
			_command, 
			_data1,
			_data2;

		public override void Reset ()
		{
			channel = 0;
			command = 0;
			data1 = 0;
			data2 = 0;
		}

		public override void OnEnter ()
		{
			_channel = channel.IsNone ? 0 : channel.Value;
			_command = command.IsNone ? 0 : command.Value;
			_data1 = data1.IsNone ? 0 : data1.Value;
			_data2 = data2.IsNone ? 0 : data2.Value;

			if (MidiPlayMakerInput.singleton != null) {
				MidiPlayMakerInput.singleton.SendShortMessage ((byte)(_command << 4 | _channel), (byte)_data1, (byte)_data2, -1);
				Finish ();
			} else {
				Finish ();	
			}
		}
	}
}