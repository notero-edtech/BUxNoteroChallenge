/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Conditions")]
	[Tooltip("Play note.")]
	public class MidiOutChordCondition : FsmStateAction
	{
		[ActionSection("Condition")]
		[Tooltip("Specify NoteON or NoteOFF hook.")]
		public Command commandEnum;
		[Tooltip("If harmonic all tones needs to be pressed at once otherwise tones can be played melodically.")]
		public bool harmonic;
		[Tooltip("Midi Id from 0 to 127. Middle C or C4 has midi index 60.")]
		public int[] ids;
		[Tooltip("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;
		[ActionSection("Events")]
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;

		
		public enum Command{
			NoteON 	= 0x9,
			NoteOFF	= 0x8
		}
				
		bool[] idsDown;
		
		public override void Reset()
		{
			ids = null;
			channelEnum = ChannelEnum.None;
			harmonic = false;
			trueEvent = null;
			falseEvent = null;
		}
				
		ShortMessageEventHandler shortMessageDelegate;
		public override void OnEnter(){
			idsDown = new bool[ids.Length];
			shortMessageDelegate = new ShortMessageEventHandler(OnShortMessage);
			MidiOut.ShortMessageEvent += shortMessageDelegate;
		}
		
		public override void OnExit(){
			MidiOut.ShortMessageEvent -= shortMessageDelegate;
			shortMessageDelegate = null;
		}
				
		void OnShortMessage(int aCommand, int aData1, int aData2, int deviceId){
			bool procesed = false;
			bool allDown = true; 
			if(((aCommand & 240) >> 4) == 0x08){
				//NOTE OFF//
				for(int i = 0; i<ids.Length; i++){
					if(ids[i] == aData1){
						if(harmonic) idsDown[i] = false;
						procesed = true; 
					}
				}
			} else if(((aCommand & 240) >> 4) == 0x09){
				//NOTE ON//
				for(int i = 0; i<ids.Length; i++){
					if(ids[i] == aData1){
						idsDown[i] = true;
						procesed = true;
					}
				}
			}

			foreach(bool down in idsDown){
				if(!down) allDown = false;
			}

			if(procesed){
				if(allDown){
					if(trueEvent != null) Fsm.Event(trueEvent);		
				}
			} else {
				if(falseEvent != null) Fsm.Event(falseEvent);		
			}
		}
	}
}