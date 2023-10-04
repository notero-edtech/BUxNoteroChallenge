/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Conditions")]
	[Tooltip("Play note.")]
	public class MidiOutNoteCondition : FsmStateAction
	{
		[ActionSection("Condition")]
		[Tooltip("Specify NoteON or NoteOFF hook.")]
		public Command commandEnum;
		public FsmBool ignoreOpposite;
		[Tooltip("Midi Id from 0 to 127. Middle C or C4 has midi index 60.")]
		public FsmInt id;
		[Tooltip("You can specify a note by name. ID has to be None.")]
		public NoteEnum noteEnum;
		 [Tooltip("You can specify a note accidental.")]
		public AccidentalEnum accidentalEnum;
		[Tooltip("If you use 'noteEnum' insteead of 'ID' an octaveEnum has to be specified.")]
		public OctaveEnum octaveEnum;
		[Tooltip("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;
		[ActionSection("Events")]
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		
		public enum Command{
			NoteON,
			NoteOFF,
			NoteONOFF
		}
			
		public override void Reset()
		{
			id = new FsmInt{UseVariable = true };
			noteEnum = NoteEnum.None;
			accidentalEnum = AccidentalEnum.None;
			channelEnum = ChannelEnum.None;
			octaveEnum = OctaveEnum.None;
			commandEnum = MidiOutNoteCondition.Command.NoteONOFF;
			trueEvent = null;
			falseEvent = null;
			ignoreOpposite = true;
		}
		
		
		ShortMessageEventHandler shortMessageDelegate;
		public override void OnEnter()
		{
			shortMessageDelegate = new ShortMessageEventHandler(OnShortMessage);
			MidiOut.ShortMessageEvent += shortMessageDelegate;
		}
		
		public override void OnExit(){
			MidiOut.ShortMessageEvent -= shortMessageDelegate;
			shortMessageDelegate = null;
		}
				
		void OnShortMessage(int aCommand, int aData1, int aData2, int deviceId){
			int command = aCommand.ToMidiCommand();
			int channel = aCommand.ToMidiChannel();

			// ignore all other commands //
			if(!(command == (int)CommandEnum.MIDI_NOTE_OFF || command == (int)CommandEnum.MIDI_NOTE_ON)) return;

			// check if channel matches //
			if(channelEnum != ChannelEnum.None && channel != (int)channelEnum) {
				ReturnResult(false);
				return;
			}

			if(id.IsNone && noteEnum == NoteEnum.None) {
				Debug.LogError("MidiOutNoteCondition: midi id not set!");
			}

			int midiIndex = 0;

			if(id.IsNone){
				midiIndex = (int)noteEnum + (int)accidentalEnum + (int)octaveEnum*12;
			} else {
				midiIndex = id.Value;
			}
			
			if(midiIndex != aData1){
				ReturnResult(false);
				return;
			}

			// check if command matches //
			switch(commandEnum){
			case Command.NoteON:
				if(command != (int)CommandEnum.MIDI_NOTE_ON) {
					if(ignoreOpposite.IsNone || ignoreOpposite.Value == false){
						ReturnResult(false);
						
					} else {
						if(command == (int)CommandEnum.MIDI_NOTE_OFF) {
							
						} else {
							ReturnResult(false);
							
						}
					}
				} else {
					ReturnResult(true);	
					
				}
		   	break;
		  	 case Command.NoteOFF:
				if(command != (int)CommandEnum.MIDI_NOTE_OFF) {
					if(ignoreOpposite.IsNone || ignoreOpposite.Value == false){
						ReturnResult(false);
						
					} else {
						if(command == (int)CommandEnum.MIDI_NOTE_ON) {
							
						} else {
							ReturnResult(false);
							
						}
					}
				} else {
					ReturnResult(true);	
					
				}
		   	break;
		   	case Command.NoteONOFF:
				if(!(command == (int)CommandEnum.MIDI_NOTE_OFF || command == (int)CommandEnum.MIDI_NOTE_ON)){
					ReturnResult(false);
					
				} else {
					ReturnResult(true);	
					
				}
		   	break;
		   	}
		}

		void ReturnResult(bool result){
			if(result) {
				if(trueEvent != null) Fsm.Event(trueEvent);	
			} else {
				if(falseEvent != null) Fsm.Event(falseEvent);		
			}
		}
	}
}