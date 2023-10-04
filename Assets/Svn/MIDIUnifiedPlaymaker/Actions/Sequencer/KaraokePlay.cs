/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Karaoke Sequencer")]
	[Tooltip("Play Midi Sequencer.")]
	public class KaraokePlay : FsmStateAction
	{						
		public bool pickupBar;
		
		GameObject go;
		
		public override void Reset()
		{
			pickupBar = true;
		}

		public override void OnEnter()
		{
			if(MidiSeqKaraokeScript.singleton){
				MidiSeqKaraokeScript.singleton.Play(pickupBar);	
			}
			Finish();
		}
	}
}