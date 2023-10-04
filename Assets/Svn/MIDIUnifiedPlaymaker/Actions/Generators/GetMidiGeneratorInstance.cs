/* Copyright © Marek Ledvina, Foriero s.r.o. */

using ForieroEngine.MIDIUnified;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Generators")]
	[Tooltip ("Get MidiGenerator Instance")]
	public class GetMidiGeneratorInstance : FsmStateAction
	{
		[RequiredField] public FsmString id;
		
		[RequiredField]
		[ObjectType(typeof(FsmMidiGeneratorInstance))]
		[UIHint(UIHint.Variable)]
		public FsmObject instance;

		private FsmMidiGeneratorInstance _instance = null;
		
		public override void Reset()
		{
			id = null;
			instance = null;
		}
		
		public override void OnEnter()
		{
			var s = MidiSenders.GetById(id.Value);
			if (s != null)
			{
				instance.Value = _instance = ScriptableObject.CreateInstance<FsmMidiGeneratorInstance>();
				_instance.instance = s as IMidiSender;
			}
			Finish();
		}
	}
}