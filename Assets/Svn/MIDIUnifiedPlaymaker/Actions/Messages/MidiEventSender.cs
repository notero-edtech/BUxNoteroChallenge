using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	
	[ActionCategory("MIDI Unified - Messages")]
	[Tooltip("Send a GUI Event.")]
	public class MidiEventSender : FsmStateAction
	{
		
		public FsmInt channel;
		public FsmInt command;
		public FsmInt data1;
		public FsmInt data2;
		
		public delegate void GUIEventDelegate(int channel, int command, int data1, int data2);
		static public event GUIEventDelegate OnGUIEventSent;
				
		public override void Reset()
		{
			channel = new FsmInt{UseVariable = true};
			command = new FsmInt{UseVariable = true};
			data1 = new FsmInt{UseVariable = true};
			data2 = new FsmInt{UseVariable = true};
		}

		public override void OnEnter()
		{
			
			CallEventSent();
			Finish();
		}
				
		void CallEventSent(){

            OnGUIEventSent?.Invoke(
                (channel.IsNone ? channel.Value : -1),
                (command.IsNone ? command.Value : -1),
                (data1.IsNone ? data1.Value : -1),
                (data2.IsNone ? data2.Value : -1)
           );
        }
	}
}