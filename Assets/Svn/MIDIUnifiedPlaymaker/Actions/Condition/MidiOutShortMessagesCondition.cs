/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Conditions")]
	[Tooltip("Wait for a midi conditions.")]
	public class MidiOutShortMessageCondition : FsmStateAction
	{
		[ActionSection("Condition")]
		public FsmInt channel;
		public FsmInt command;
		public FsmInt data1;
		public FsmInt data2;
		[ActionSection("Events")]
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
				
		public override void Reset()
		{
			channel = new FsmInt{UseVariable = true};
			command = new FsmInt{UseVariable = true};
			data1 = new FsmInt{UseVariable = true};
			data2 = new FsmInt{UseVariable = true};
			trueEvent = null;
			falseEvent = null;
		}

		public override void OnEnter()
		{
			shortMessageDelegate = new ShortMessageEventHandler(OnShortMessage);
			MidiOut.ShortMessageEvent += shortMessageDelegate;
		}
		
		public override void OnExit(){
			MidiOut.ShortMessageEvent -= shortMessageDelegate;
			shortMessageDelegate = null;
		}
		
		ShortMessageEventHandler shortMessageDelegate;
		
		void OnShortMessage(int aCommand, int aData1, int aData2, int deviceId){
			bool result = true;
			if(!channel.IsNone){
				if(channel.Value != (aCommand & 15)) result = false;
			}
			
			if(!command.IsNone){
				if(command.Value != (aCommand >> 4)) result = false;
				else {
					if(!data1.IsNone){
						if(data1.Value != aData1) result = false;
						else {
							if(!data2.IsNone){
								if(data2.Value != aData2) result = false;
							}
						}
					}
				}
			}
								
			if(result) {
				if(trueEvent != null) Fsm.Event(trueEvent);	
			} else {
				if(falseEvent != null) Fsm.Event(falseEvent);		
			}
		}
	}
}