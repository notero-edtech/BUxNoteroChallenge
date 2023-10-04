/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Messages")]
	[Tooltip("Play note.")]
	public class MidiOutDispatcherAction : FsmStateAction
	{
		[Tooltip("Midi Id from 0 to 127. Middle C or C4 has midi index 60.")]
		public FsmInt id;
		[RequiredField]
		[Tooltip("You can specify how long will the notes be played before Finished event is called.")]
		public FsmFloat duration;
		[Tooltip("You can specify how long will the notes be played before Finished event is called.")]
		public FsmFloat delay;
		[Tooltip("Sets midi out messages volume.")]
		[HasFloatSlider(0,1)]
		public FsmFloat volume;
		 [Tooltip("You can specify a note by name. ID has to be None.")]
		public NoteEnum note;
		 [Tooltip("You can specify a note accidental.")]
		public AccidentalEnum accidental;
		 [Tooltip("If you use 'noteEnum' insteead of 'ID' an octaveEnum has to be specified.")]
		public OctaveEnum octave;
		[Tooltip("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public FsmInt channel;
		 [Tooltip("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
		public ChannelEnum channelEnum;
		public bool waitForFinish;
						
		float ellapsedTime = 0f;
														
		public override void Reset()
		{
			id = new FsmInt{UseVariable = true };
			note = NoteEnum.None;
			duration = new FsmFloat{UseVariable = true};
			delay = new FsmFloat{UseVariable = true};
			accidental = AccidentalEnum.None;
			channel = new FsmInt{ UseVariable = true };
			channelEnum = ChannelEnum.None;
			octave = OctaveEnum.Octave4;
			volume = new FsmFloat{ UseVariable = true };
			waitForFinish = false;
		}

		public override void OnEnter(){
			int noteIndex = (note.ToInt() + accidental.ToInt() + ((int)(octave == OctaveEnum.None ? OctaveEnum.Octave4 : octave) * 12) + 24);
			int v = Mathf.RoundToInt((volume.IsNone ? 1f : volume.Value) * 127);
			int ch =  channel.IsNone ? (channelEnum == ChannelEnum.None ? 0 : channelEnum.ToInt()) : channel.Value;
			MidiDispatcher.DispatchNote(id.IsNone ? noteIndex : id.Value, v, ch, -1, duration.Value, delay.IsNone ? 0f : delay.Value);
			if(!waitForFinish) Finish (); else ellapsedTime = duration.Value + (delay.IsNone ? 0f : delay.Value);
		}

		public override void OnUpdate(){
			if(waitForFinish){
				if(ellapsedTime > 0f){
					ellapsedTime -= Time.deltaTime;
				} else {
					Finish ();
				}
			}
		}
	}
}