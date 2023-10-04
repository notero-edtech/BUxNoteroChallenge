/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Conditions")]
	[Tooltip("Wait for a midi conditions.")]
	public class MidiOutNonRepeater : FsmStateAction
	{
		public enum CmdEnum{
			Reset,
			Listen
		}

		public CmdEnum command;

		public ChannelEnum channelEnum;
		public bool registerOnEvent;
		public bool registerOffEvent;

		[ActionSection("Events")]
		public FsmEvent repeatedEvent;
		public FsmEvent newEvent;

		public static bool[] offEvents = new bool[128];
		public static bool[] onEvents = new bool[128];
				
		public override void Reset(){
			command = CmdEnum.Listen;
			channelEnum = ChannelEnum.None;
			registerOnEvent = true;
			registerOffEvent = false;
			repeatedEvent = null;
			newEvent = null;
		}

		public override void OnEnter()
		{
			switch(command){
			case CmdEnum.Reset: 
				offEvents = new bool[128];
				onEvents = new bool[128];
				Finish ();
				break;
			case CmdEnum.Listen: 
				shortMessageDelegate = new ShortMessageEventHandler(OnShortMessage);
				MidiOut.ShortMessageEvent += shortMessageDelegate;
				break;
			}

		}
		
		public override void OnExit(){
			switch(command){
			case CmdEnum.Reset: 
				;
				break;
			case CmdEnum.Listen: 
				MidiOut.ShortMessageEvent -= shortMessageDelegate;
				shortMessageDelegate = null;
				break;
			}
		}
		
		ShortMessageEventHandler shortMessageDelegate;
		
		void OnShortMessage(int aCommand, int aData1, int aData2, int deviceId){
			int channel = aCommand.ToMidiChannel();
			int command = aCommand.ToMidiCommand();

			if(!(command == (int)CommandEnum.MIDI_NOTE_OFF || command == (int)CommandEnum.MIDI_NOTE_ON)) return;

			if(command == (int)CommandEnum.MIDI_NOTE_OFF && !registerOffEvent) return;

			if(command == (int)CommandEnum.MIDI_NOTE_ON && !registerOnEvent) return;

			// check if channel matches //
			if(channelEnum != ChannelEnum.None && channel != (int)channelEnum) return;

			if(command == (int)CommandEnum.MIDI_NOTE_OFF){
				if(offEvents[aData1]){
					ReturnResult(true);
				} else {
					offEvents[aData1] = true;
					ReturnResult(false);
				}
			}

			if(command == (int)CommandEnum.MIDI_NOTE_ON){
				if(onEvents[aData1]){
					ReturnResult(true);
				} else {
					onEvents[aData1] = true;
					ReturnResult(false);
				}
			}
		}

		void ReturnResult(bool repeated){
			if(repeated) {
				if(repeatedEvent != null) Fsm.Event(repeatedEvent);	
			} else {
				if(newEvent != null) Fsm.Event(newEvent);		
			}
		}
	}
}