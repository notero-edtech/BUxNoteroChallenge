// (c) Copyright Marek Ledvina, Foriero Studo

using UnityEngine;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Connection")]
	[Tooltip ("Connect to a MIDI IN device(port).")]
	public class MidiInConnect : FsmStateAction
	{
		[Tooltip ("Midi In Device(Port).")]
		public FsmInt Port;
		[Tooltip ("If you specify a PortName the action will try to find a Port with such a string")]
		public FsmString PortName;
		[Tooltip ("Called when a Midi In port is successfuly connected.")]
		public FsmEvent connectedEvent;
		[Tooltip ("Called when a Midi In port is not successfuly connected.")]
		public FsmEvent errorEvent;

		[UIHint (UIHint.Variable)]
		public FsmInt connectionId;

		public override void Reset ()
		{
			Port = new FsmInt{ UseVariable = true };
			PortName = new FsmString{ UseVariable = true };
			connectedEvent = null;
			errorEvent = null;

			connectionId = null;
		}

		public override void OnEnter ()
		{
			if (!PortName.IsNone) {
				for (int i = 0; i < MidiINPlugin.GetDeviceCount (); i++) {
					string name = MidiINPlugin.GetDeviceName (i);
					if (name.Contains (PortName.Value)) {
						if (ConnectPort (i)) {
							Finish ();
							if (connectedEvent != null)
								Fsm.Event (connectedEvent);
							return;
						} else {
							Finish ();
							if (errorEvent != null)
								Fsm.Event (errorEvent);
							return;
						}
					}
				}
				if (!Port.IsNone) {
					if (ConnectPort (Port.Value)) {
						Finish ();
						if (connectedEvent != null)
							Fsm.Event (connectedEvent);
						return;
					} else {
						Finish ();
						if (errorEvent != null)
							Fsm.Event (errorEvent);
						return;
					}
				}
			} else {
				if (!Port.IsNone) {
					if (ConnectPort (Port.Value)) {
						Finish ();
						if (connectedEvent != null)
							Fsm.Event (connectedEvent);
						return;
					} else {
						Finish ();
						if (errorEvent != null)
							Fsm.Event (errorEvent);
						return;
					}
				}
			}
			
			Finish ();
			if (errorEvent != null)
				Fsm.Event (errorEvent);
		}

		bool ConnectPort (int aPort)
		{
			MidiDevice device = MidiINPlugin.ConnectDevice (aPort);
			if (!connectionId.IsNone) {
				connectionId.Value = device.deviceId;
			}
			return device.deviceId >= 0;
		}
	}
}