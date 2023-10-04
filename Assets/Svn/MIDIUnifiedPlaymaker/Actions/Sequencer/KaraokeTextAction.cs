/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Sequencer")]
	[Tooltip ("Play Midi Sequencer Karaoke Text.")]
	public class KaraokeTextAction : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault owner;
		
		[UIHint (UIHint.Variable)]
		public FsmString word;
		public FsmEvent wordEvent;
		public FsmEvent wordFinishedEvent;
		[UIHint (UIHint.Variable)]
		public FsmString sentence;
		public FsmEvent sentenceEvent;
		[UIHint (UIHint.Variable)]
		public FsmString verse;
		public FsmEvent verseEvent;
		
		public FsmEvent finishedEvent;


		
		GameObject go;
		MidiSeqKaraokeScript karaoke;

		public override void Reset ()
		{
			word = null;
			wordEvent = null;
			wordFinishedEvent = null;
			sentence = null;
			sentenceEvent = null;
			verse = null;
			verseEvent = null;
			finishedEvent = null;

		}

		public override void OnEnter ()
		{
			go = Fsm.GetOwnerDefaultTarget (owner);
			if (go) {
				karaoke = go.GetComponent<MidiSeqKaraokeScript> ();
				karaoke.OnWord += OnWord;
				karaoke.OnWordFinished += OnWordFinished;
				karaoke.OnSentence += OnSentence;
				karaoke.OnVerse += OnVerse;
				karaoke.OnFinished += OnFinished;
				//Debug.Log ("KaraokeTextAction - Events Hooked");
			}
		}

		public override void OnExit ()
		{
			if (karaoke) {
				karaoke.OnWord -= OnWord;
				karaoke.OnWordFinished -= OnWordFinished;
				karaoke.OnSentence -= OnSentence;
				karaoke.OnVerse -= OnVerse;
				karaoke.OnFinished -= OnFinished;
				//Debug.Log ("KaraokeTextAction - Events Released");
			}
		}

		void OnFinished ()
		{
			if (finishedEvent != null)
				Fsm.Event (finishedEvent);
			Finish ();
		}

		void OnWord (MidiSeqKaraoke.WordText w)
		{
			if (!word.IsNone)
				word.Value = w.text;
			if (wordEvent != null)
				Fsm.Event (wordEvent);
			Finish ();
		}

		void OnWordFinished ()
		{
			if (wordFinishedEvent != null)
				Fsm.Event (wordFinishedEvent);
			Finish ();
		}

		void OnSentence (MidiSeqKaraoke.SentenceText s)
		{
			if (!sentence.IsNone) {
				sentence.Value = s.text;
			}
			if (sentenceEvent != null)
				Fsm.Event (sentenceEvent);
			Finish ();
		}

		void OnVerse (MidiSeqKaraoke.MidiText w)
		{
			if (!verse.IsNone)
				verse.Value = w.text;
			if (verseEvent != null)
				Fsm.Event (verseEvent);
			Finish ();
		}
	}
}