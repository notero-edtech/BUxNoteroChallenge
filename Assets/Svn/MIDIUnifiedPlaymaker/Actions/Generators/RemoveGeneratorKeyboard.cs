/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Generators")]
	[Tooltip("Remove Midi Keyboard Generator.")]
	public class RemoveGeneratorKeyboard : FsmStateAction
	{						
		public override void Reset()
		{
		
		}

		public override void OnEnter()
		{
			MidiKeyboardInput input = (MidiKeyboardInput)this.Owner.GetComponent<MidiKeyboardInput>();
			if(input != null) Object.Destroy(input);			
			Finish();
		}
	}
}