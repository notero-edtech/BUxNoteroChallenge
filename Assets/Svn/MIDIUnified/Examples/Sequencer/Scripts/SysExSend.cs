using UnityEngine;
using ForieroEngine.MIDIUnified.Plugins;

public class SysExSend : MonoBehaviour
{
	void Start ()
	{
		MidiInput.MidiBytesEvent += SysExMessage;
	}

	void OnDestroy ()
	{
        MidiInput.MidiBytesEvent -= SysExMessage;
    }

	#pragma warning disable 436
	void SysExMessage (byte[] bytes, int deviceId)
	{
		
		dataStr = "";
		for (int i = 0; i < bytes.Length; i++) {
			dataStr += bytes[i].ToString () + " ";	
		}
	}
	#pragma warning restore 436

	string dataStr = "";

	void OnGUI ()
	{
		if (GUILayout.Button ("Send SYS EX")) {
			byte[] data = new byte[6] { 0xF0, 1, 2, 3, 4, 0xF7 };
			MidiOUTPlugin.SendData (data);
		}
		GUILayout.Label ("DATA :" + dataStr);
		
	}
}
