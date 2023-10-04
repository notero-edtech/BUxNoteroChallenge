/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Karaoke Sequencer")]
	[Tooltip ("Play Midi Sequencer.")]
	public class KaraokeAction : FsmStateAction
	{

		public enum CommandEnum
		{
			none,
			play,
			pause,
			cont,
			stop,
			set_file,
			set_volume,
			events,
			set_speed,
			settings
		}

		public enum EventEnum
		{
			OnContinue,
			OnFinished,
			OnInitialized,
			OnMidiLoaded,
			OnPause,
			OnPickUpBar,
			OnPlay,
			OnRepeat,
			OnSentence,
			OnSentenceFinished,
			OnStop,
			OnTempoChange,
			OnVerse,
			OnVerseFinished,
			OnWord,
			OnWordFinished,
			OnWordOffset,
			OnWordOffsetFinished
		}

		[RequiredField]
		[Tooltip ("GameObject")]
		public FsmOwnerDefault gameObject;

		public CommandEnum command;
		public EventEnum karaokeEvent;
		public FsmFloat delay;
		public FsmBool pickupBar;
		public TextAsset midiFile;
		public AudioClip music;
		public FsmFloat musicVolume;
		public AudioClip vocals;
		public FsmFloat vocalsVolume;
		public FsmFloat floatValue;

		[UIHint (UIHint.Variable)]
		public FsmString commands;

		public FsmEvent OnContinue;
		public FsmEvent OnFinished;
		public FsmEvent OnInitialized;
		public FsmEvent OnMidiLoaded;
		public FsmEvent OnPause;
		public FsmEvent OnPickUpBar;
		public FsmEvent OnPlay;
		public FsmEvent OnRepeat;
		public FsmEvent OnSentence;
		public FsmEvent OnSentenceFinished;
		public FsmEvent OnStop;
		public FsmEvent OnTempoChange;
		public FsmEvent OnVerse;
		public FsmEvent OnVerseFinished;
		public FsmEvent OnWord;
		public FsmEvent OnWordFinished;
		public FsmEvent OnWordOffset;
		public FsmEvent OnWordOffsetFinished;

		public FsmBool playMusic;
		public FsmBool playVocals;
		
		//public FsmEvent

		GameObject go;

		bool commandCalled;
		float elapsedDelay;
		MidiSeqKaraokeScript karaoke;

		public override void Reset ()
		{
			command = CommandEnum.none;
			pickupBar = true;
			commandCalled = false;
			elapsedDelay = 0f;
			delay = new FsmFloat{ UseVariable = true };
			musicVolume = new FsmFloat{ UseVariable = true };
			music = null;
			vocalsVolume = new FsmFloat{ UseVariable = true };
			vocals = null;

			floatValue = new FsmFloat{ UseVariable = true };
			commands = new FsmString{ UseVariable = true };

			OnContinue = null;
			OnFinished = null;
			OnInitialized = null;
			OnMidiLoaded = null;
			OnPause = null;
			OnPickUpBar = null;
			OnPlay = null;
			OnRepeat = null;
			OnSentence = null;
			OnSentenceFinished = null;
			OnStop = null;
			OnTempoChange = null;
			OnVerse = null;
			OnVerseFinished = null;
			OnWord = null;
			OnWordFinished = null;
			OnWordOffset = null;
			OnWordOffsetFinished = null;

			playMusic = true;
			playVocals = true;
		}

		public override void OnEnter ()
		{
			go = Fsm.GetOwnerDefaultTarget (gameObject);

			karaoke = go.GetComponent<MidiSeqKaraokeScript> ();

			if (!karaoke)
				karaoke = MidiSeqKaraokeScript.singleton;

			if (!karaoke) {
				Debug.LogError ("No MidiSeqKaraokeScript found!");
				Finish ();
				return;
			}

			commandCalled = false;
			if (delay.IsNone) {
				CallCommand ();
			} else {
				elapsedDelay = delay.Value;
			}
		}

		public override void OnUpdate ()
		{
			if (!delay.IsNone && !commandCalled) {
				elapsedDelay -= Time.deltaTime;
				if (elapsedDelay <= 0) {
					CallCommand ();
				}
			}
		}

		public override void OnExit ()
		{
			karaoke.OnResume -= HandleOnContinue;
			karaoke.OnFinished -= HandleOnFinished;
			karaoke.OnInitialized -= HandleOnInitialized;
			karaoke.OnMidiLoaded -= HandleOnMidiLoaded;
			karaoke.OnPause -= HandleOnPause;
			karaoke.OnPickUpBar -= HandleOnPickUpBar;
			karaoke.OnPlay -= HandleOnPlay;
			karaoke.OnRepeat -= HandleOnRepeat;
			karaoke.OnSentence -= HandleOnSentence;
//			karaoke.OnSentenceFinished -= HandleOnSentenceFinished;
			karaoke.OnStop -= HandleOnStop;
			karaoke.OnTempoChange -= HandleOnTempoChange;
			karaoke.OnVerse -= HandleOnVerse;
//			karaoke.OnVerseFinished -= HandleOnVerseFinished;
			karaoke.OnWord -= HandleOnWord;
			karaoke.OnWordFinished -= HandleOnWordFinished;
			karaoke.OnWordOffset -= HandleOnWordOffset;
			karaoke.OnWordOffsetFinished -= HandleOnWordOffsetFinished;
		}

		void CallCommand ()
		{
			commandCalled = true;
			switch (command) {
			case CommandEnum.play:
				Debug.Log ("KaraokeAction - PLAY");
				karaoke.Play (pickupBar.IsNone ? false : pickupBar.Value);
				Finish ();
				break;
			case CommandEnum.stop:
				karaoke.Stop ();
				Finish ();
				break;
			case CommandEnum.cont:
				karaoke.Continue ();
				Finish ();
				break;
			case CommandEnum.set_file:
				if(midiFile) karaoke.Initialize (midiFile.bytes, vocals, music);
				Finish ();
				break;
			case CommandEnum.set_volume:
				karaoke.musicVolume = musicVolume.IsNone ? karaoke.musicVolume : musicVolume.Value;
				karaoke.vocalsVolume = vocalsVolume.IsNone ? karaoke.vocalsVolume : vocalsVolume.Value;
				Finish ();
				break;
			case CommandEnum.set_speed:
				karaoke.SetSpeed (floatValue.IsNone ? 1f : floatValue.Value);
				Finish ();
				break;
			case CommandEnum.events:
				switch (karaokeEvent) {
				case EventEnum.OnContinue:
					karaoke.OnResume += HandleOnContinue;
					break;
				case EventEnum.OnFinished:
					karaoke.OnFinished += HandleOnFinished;
					break;
				case EventEnum.OnInitialized:
					karaoke.OnInitialized += HandleOnInitialized;
					break;
				case EventEnum.OnMidiLoaded:
					karaoke.OnMidiLoaded += HandleOnMidiLoaded;
					break;
				case EventEnum.OnPause:
					karaoke.OnPause += HandleOnPause;
					break;
				case EventEnum.OnPickUpBar:
					karaoke.OnPickUpBar += HandleOnPickUpBar;
					break;
				case EventEnum.OnPlay:
					karaoke.OnPlay += HandleOnPlay;
					break;
				case EventEnum.OnRepeat:
					karaoke.OnRepeat += HandleOnRepeat;
					break;
				case EventEnum.OnSentence:
					karaoke.OnSentence += HandleOnSentence;
					break;
				case EventEnum.OnSentenceFinished:
//					karaoke.OnSentenceFinished += HandleOnSentenceFinished;
					break;
				case EventEnum.OnStop:
					karaoke.OnStop += HandleOnStop;
					break;
				case EventEnum.OnTempoChange:
					karaoke.OnTempoChange += HandleOnTempoChange;
					break;
				case EventEnum.OnVerse:
					karaoke.OnVerse += HandleOnVerse;
					break;
				case EventEnum.OnVerseFinished:
//					karaoke.OnVerseFinished += HandleOnVerseFinished;
					break;
				case EventEnum.OnWord:
					karaoke.OnWord += HandleOnWord;
					break;
				case EventEnum.OnWordFinished:
					karaoke.OnWordFinished += HandleOnWordFinished;
					break;
				case EventEnum.OnWordOffset:
					karaoke.OnWordOffset += HandleOnWordOffset;
					break;
				case EventEnum.OnWordOffsetFinished:
					karaoke.OnWordOffsetFinished += HandleOnWordOffsetFinished;
					break;
				}
				break;
			case CommandEnum.settings:
				if (!playMusic.IsNone)
					karaoke.music = playMusic.Value;
				if (!playVocals.IsNone)
					karaoke.vocals = playVocals.Value;
				Finish ();
				break;
			}

		}

		void HandleOnWordOffsetFinished ()
		{
			if (OnWordOffsetFinished != null) {
				Fsm.Event (OnWordOffsetFinished);
				Finish ();
			}

		}

		void HandleOnWordOffset (MidiSeqKaraoke.WordText obj)
		{
			if (OnWordOffset != null) {
				Fsm.Event (OnWordOffset);
				Finish ();
			}
		}

		void HandleOnWordFinished ()
		{
			if (OnWordFinished != null) {
				Fsm.Event (OnWordFinished);
				Finish ();
			}
		}

		void HandleOnWord (MidiSeqKaraoke.WordText obj)
		{
			if (OnWord != null) {
				commands.Value = string.Join (";", obj.commands.ToArray ());
				Fsm.Event (OnWord);
				Finish ();
			}
		}

		void HandleOnVerseFinished ()
		{
			if (OnVerseFinished != null) {
				Fsm.Event (OnVerseFinished);
				Finish ();
			}
		}

		void HandleOnVerse (MidiSeqKaraoke.MidiText obj)
		{
			if (OnVerse != null) {
				Fsm.Event (OnVerse);
				Finish ();
			}
		}

		void HandleOnTempoChange (float obj)
		{
			if (OnTempoChange != null) {
				Fsm.Event (OnTempoChange);
				Finish ();
			}
		}

		void HandleOnStop ()
		{
			if (OnStop != null) {
				Fsm.Event (OnStop);
				Finish ();
			}
		}

		void HandleOnSentenceFinished ()
		{
			if (OnSentenceFinished != null) {
				Fsm.Event (OnSentenceFinished);
				Finish ();
			}
		}

		void HandleOnSentence (MidiSeqKaraoke.SentenceText obj)
		{
			if (OnSentence != null) {
				Fsm.Event (OnSentence);
				Finish ();
			}

		}

		void HandleOnRepeat (int obj)
		{
			if (OnRepeat != null) {
				Fsm.Event (OnRepeat);
				Finish ();
			}
		}

		void HandleOnPlay ()
		{
			if (OnPlay != null) {
				Fsm.Event (OnPlay);
				Finish ();
			}
		}

		void HandleOnPickUpBar (int obj)
		{
			if (OnPickUpBar != null) {
				Fsm.Event (OnPickUpBar);
				Finish ();
			}
		}

		void HandleOnPause ()
		{
			if (OnPause != null) {
				Fsm.Event (OnPause);
				Finish ();
			}
		}

		void HandleOnMidiLoaded ()
		{
			if (OnMidiLoaded != null) {
				Fsm.Event (OnMidiLoaded);
				Finish ();
			}
		}

		void HandleOnInitialized ()
		{
			if (OnInitialized != null) {
				Fsm.Event (OnInitialized);
				Finish ();
			}
		}

		void HandleOnFinished ()
		{
			if (OnFinished != null) {
				Fsm.Event (OnFinished);
				Finish ();
			}
		}

		void HandleOnContinue ()
		{
			if (OnContinue != null) {
				Fsm.Event (OnContinue);
				Finish ();
			}
		}
	}
}