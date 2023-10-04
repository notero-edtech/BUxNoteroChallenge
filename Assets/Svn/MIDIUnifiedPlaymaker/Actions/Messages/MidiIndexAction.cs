/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Messages")]
	[Tooltip("Play note.")]
	public class MidiIndexAction : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeIndex;

		[Tooltip("You can specify a note by name. ID has to be None.")]
		public NoteEnum note;
		 [Tooltip("You can specify a note accidental.")]
		public AccidentalEnum accidental;
		 [Tooltip("If you use 'noteEnum' insteead of 'ID' an octaveEnum has to be specified.")]
		public OctaveEnum octave;
							
																
		public override void Reset()
		{
			note = NoteEnum.None;
			accidental = AccidentalEnum.None;
			octave = OctaveEnum.None;
			storeIndex = null;
		}

		public override void OnEnter(){
			storeIndex.Value = note.ToInt() + accidental.ToInt() + (octave.ToInt() * 12 + 12);
			Finish();	
		}
	}
}