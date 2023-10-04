/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Generators")]
	[Tooltip ("Remove Midi Sequencer Generator.")]
	public class RemoveGeneratorMidiSequencer : FsmStateAction
	{
		public override void Reset ()
		{
		
		}

		public override void OnEnter ()
		{
			MidiSeqKaraokeScript input = (MidiSeqKaraokeScript)this.Owner.GetComponent<MidiSeqKaraokeScript> ();
			if (input != null)
				Object.Destroy (input);			
			Finish ();
		}
	}
}