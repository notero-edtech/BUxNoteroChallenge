/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Generators")]
	[Tooltip("Remove Midi Input Generator.")]
	public class RemoveGeneratorMidiInput : FsmStateAction
	{							
		public override void Reset()
		{

		}

		public override void OnEnter()
		{
			MidiInput input = (MidiInput)this.Owner.GetComponent<MidiInput>();
			if(input != null) Object.Destroy(input);
			Finish();
		}
	}
}