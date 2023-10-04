/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using System;
using ForieroEngine.MIDIUnified;
using System.Collections.Generic;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Connection")]
	[Tooltip ("MIDIUnified Init prefab has to initialize MIDI OUT and MIDI INT settings. So this action is waiting for this to happen.")]
	public class WairForMidiInit : FsmStateAction
	{
		public FsmEvent onStart;

		public override void Reset ()
		{
			onStart = null;
		}

		public override void OnUpdate ()
		{
			if (MIDI.initialized) {
				if (onStart != null)
					Fsm.Event (onStart);
				Finish ();	
			}
		}
		
	}
}