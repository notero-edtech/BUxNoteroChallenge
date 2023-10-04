/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Listeners")]
	[Tooltip("Update your object in real time.")]
	public class MidiOutShortMessageListener : FsmStateAction
	{
		[ActionSection("Condition")]
		public FsmInt channel;
		public FsmInt command;
		public FsmInt data1;
		public FsmInt data2;
		[ActionSection("Data")]
		public FsmInt channelData;
		public FsmInt commandData;
		public FsmInt data1Data;
		public FsmInt data2Data;
		[ActionSection("Events")]
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
					
		public override void Reset()
		{
			channelData = new FsmInt{UseVariable = true};
			commandData = new FsmInt{UseVariable = true};
			data1Data = new FsmInt{UseVariable = true};
			data2Data = new FsmInt{UseVariable = true};
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
				if(channel.Value != (aCommand.ToMidiChannel())) result = false;
			}
			
			if(!command.IsNone){
				if(command.Value != aCommand.ToMidiCommand()) result = false;
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
			if(result){
				if(!channelData.IsNone) channelData.Value = aCommand.ToMidiChannel();
				if(!commandData.IsNone) commandData.Value = aCommand.ToMidiCommand();
				if(!data1Data.IsNone) data1Data.Value = aData1;
				if(!data1Data.IsNone) data2Data.Value = aData2;
					
				if(trueEvent != null) {
					Fsm.Event(trueEvent);	
					Finish();
				}
			} else {
				if(falseEvent != null) {
					Fsm.Event(falseEvent);
					Finish();
				}
			}
		}
	}
}