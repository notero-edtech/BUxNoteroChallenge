/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Sequencer")]
	[Tooltip ("MidiSeqControl")]
	public class MidiSeqControl : FsmStateAction
	{
		public enum Commands
		{
			Play,
			Stop,
			Pause,
			Resume,
			Unknown = int.MaxValue
		}
		
		[RequiredField]
		[ObjectType(typeof(IMidiSeqControl))]
		public FsmOwnerDefault owner;

		public Commands command;
		
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(MidiSeqStates))]
		public FsmEnum state;
		
		[Tooltip ("Delay playing.")]
		public FsmBool pickupBar;
		
		private GameObject go;
		private IMidiSeqControl c;

		public override void Reset ()
		{
			pickupBar = new FsmBool() {UseVariable = true};
			command = Commands.Unknown;
			go = null;
			c = null;
			state = MidiSeqStates.None;
		}

		public override void OnEnter ()
		{
			go = Fsm.GetOwnerDefaultTarget (owner);
			if (go) c = go.GetComponent<IMidiSeqControl>();
			if (c != null)
			{
				switch (command)
				{
					case Commands.Play: c.Play(); break;
					case Commands.Stop: c.Stop(); break;
					case Commands.Pause: c.Pause(); break;
					case Commands.Resume: c.Continue(); break;
					case Commands.Unknown: break;
				}

				state.Value = c.State;
			}
			Finish ();
		}
	}
}