/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("MIDI Unified - Karaoke Sequencer")]
	[Tooltip("Play Midi Sequencer.")]
	public class KaraokeStop : FsmStateAction
	{						
		public override void Reset()
		{
			
		}

		public override void OnEnter()
		{
			if(MidiSeqKaraokeScript.singleton){
				MidiSeqKaraokeScript.singleton.Stop();
			}
			Finish();
		}
	}
}