/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Generators")]
	[Tooltip ("Adds Midi Sequencer Generator. This generator allows you to play a midi file.")]
	public class AddGeneratorSequencer : FsmStateAction
	{
		[Tooltip ("Send all midi input midimessages to midi output device(port).")]
		public TextAsset midiFile;
		[Tooltip ("Send all midi input midimessages to midi output device(port).")]
		public bool midiOut;
		[Tooltip ("Sets midi out messages volume.")]
		[HasFloatSlider (0, 1)]
		public FsmFloat volume;

		public override void Reset ()
		{
			midiOut = false;
			volume = new FsmFloat{ Value = 1 };
		}

		public override void OnEnter ()
		{
			MidiSeqKaraokeScript input = (MidiSeqKaraokeScript)this.Owner.AddComponent<MidiSeqKaraokeScript> ();
			input.midiOut = midiOut;
//			input.volume = volume.IsNone ? 1f : volume.Value;
//			if(midiFile != null) {
//				input.midiFile = midiFile;
//				input.LoadMidiTextAsset(midiFile);
//			}
			Finish ();
		}
	}
}