// (c) Copyright Marek Ledvina, Foriero Studo

using UnityEngine;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MIDI Unified - Connection")]
	[Tooltip ("Disconnects a MIDI OUT device(port).")]
	public class MidiOutDisconnect : FsmStateAction
	{
		[Tooltip ("Id that you get from connecting a Device(Port).")]
		public FsmInt ConnectionId;
		[Tooltip ("If you specify a PortName the action will try to find a Port with such a string")]
		public FsmString PortName;
		[Tooltip ("Called when a Midi In port is successfuly connected.")]
		public FsmEvent connectedEvent;
		[Tooltip ("Called when a Midi In port is not successfuly connected.")]
		public FsmEvent errorEvent;

		public override void Reset ()
		{
			ConnectionId = new FsmInt{ UseVariable = true };
			PortName = new FsmString{ UseVariable = true };
			connectedEvent = null;
			errorEvent = null;
		}

		public override void OnEnter ()
		{
			int deviceId = ConnectionId.IsNone ? -1 : ConnectionId.Value;
			string deviceName = PortName.IsNone ? "" : PortName.Value;
			foreach (MidiDevice device in MidiOUTPlugin.connectedDevices) {
				if (device.deviceId == deviceId || device.name == deviceName) {
					MidiOUTPlugin.DisconnectDevice (device);
					if (connectedEvent != null) {
						Fsm.Event (connectedEvent);
					}
					Finish ();
					return;
				}
			}

			if (errorEvent != null) {
				Fsm.Event (errorEvent);
			}
			Finish ();
		}
	}
}