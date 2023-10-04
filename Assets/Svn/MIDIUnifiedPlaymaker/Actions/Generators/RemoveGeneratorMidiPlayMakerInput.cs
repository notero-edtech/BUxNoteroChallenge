/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Generators")]
	[Tooltip("Remove Midi Input Generator.")]
	public class RemoveGeneratorMidiPlayMakerInput : FsmStateAction
	{							
		public override void Reset()
		{

		}

		public override void OnEnter()
		{
			MidiPlayMakerInput input = (MidiPlayMakerInput)this.Owner.GetComponent<MidiPlayMakerInput>();
			if(input != null) GameObject.Destroy(input);
			Finish();
		}
	}
}