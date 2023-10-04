using ForieroEditor;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using MoreLinq.Extensions;
using UnityEditor;
using UnityEngine;

public partial class MIDIConnectionEditor : EditorWindow
{
	[MenuItem("Foriero/Tools/MIDI/Connections &m", false, -2000)]
	private static void MIDIConnectionEditorMenu()
	{
		window = EditorWindow.GetWindow(typeof(MIDIConnectionEditor)) as MIDIConnectionEditor;
		window.titleContent = new GUIContent("MIDI IN/OUT");
	}

	public static MIDIConnectionEditor window;
				
	private static string _messagesIn = "";
	private static string _messagesOut = "";
	private static MidiEvents _me = new MidiEvents();

	[InitializeOnLoadMethod]
	private static void AutoInit()
	{
		_me.NoteOnEvent += (id, value, channel) => MIDIPianoEditor.NoteOn(id);
		_me.NoteOffEvent += (id, value, channel) => MIDIPianoEditor.NoteOff(id);
		
		if (!MidiINPlugin.initialized)
		{
			EditorDispatcher.StartThread((a) =>
			{
				MidiINPlugin.Init();
				MidiINPlugin.Refresh();
				EditorDispatcher.Dispatch((b) =>
				{
					MidiINPlugin.RestoreEditorConnections();
					_midiInInitialized = true;
				}, new { a = "" });
			}, new { a = "" });
		}
		else
		{
			_midiInInitialized = true;
		}

		if (!MidiOUTPlugin.initialized)
		{
			EditorDispatcher.StartThread((a) =>
			{
				MidiOUTPlugin.Init();
				MidiOUTPlugin.Refresh();
				EditorDispatcher.Dispatch((b) =>
				{
					MidiOUTPlugin.RestoreEditorConnections();
					_midiOutInitialized = true;
				}, new { a = "" });
			}, new { a = "" });
		}
		else
		{
			_midiOutInitialized = true;
		}

		EditorApplication.update -= EditorUpdate;
		EditorApplication.update += EditorUpdate;

        EditorApplication.wantsToQuit += () =>
        {
            EditorApplication.update -= EditorUpdate;

            MidiINPlugin.DisconnectDevices(true);
            MidiINPlugin.CloseVirtualPorts(true);

            MidiOUTPlugin.DisconnectDevices(true);
            MidiOUTPlugin.CloseVirtualPorts(true);

            return true;
        };
	}

	~MIDIConnectionEditor() { EditorApplication.update -= EditorUpdate; }

	private static bool _midiOutInitialized = false;
	private static bool _midiInInitialized = false;
    private static bool _midiReceived = false;

	private static void EditorUpdate()
	{
        _midiReceived = false;
		while (MidiINPlugin.PopMessage(out var m, true) == 1)
		{
			_messagesIn = m.ToString() + System.Environment.NewLine + _messagesIn;
			_me.AddShortMessage(m.CommandAndChannel, m.Data1, m.Data2);
            _midiReceived = true;
		}
		if (window != null && _midiReceived && _oneFrameDelay) { window.Repaint(); }
	}

	private Vector2 _scrollMessagesIn = Vector2.zero;
	private Vector2 _scrollMessagesOut = Vector2.zero;
	private bool _debug = false;
	private static bool _oneFrameDelay = false;

	private void OnGUI()
	{
		if (window == null)
		{
			window = EditorWindow.GetWindow(typeof(MIDIConnectionEditor)) as MIDIConnectionEditor;
			_oneFrameDelay = false;
			return;
		}

		if (!_midiInInitialized || !_midiOutInitialized)
		{
			EditorGUILayout.HelpBox("Initializing MIDI. Please wait....", MessageType.Info);
			_oneFrameDelay = false;
			return;
		}

		if (!_oneFrameDelay)
		{
			_oneFrameDelay = true;
			return;
		}

		if (!_oneFrameDelay)
		{
			_oneFrameDelay = true;
			EditorGUILayout.HelpBox("Waiting one frame...", MessageType.Info);
			return;
		}

		DrawINOUT();
		EditorGUILayout.BeginHorizontal();
		_scrollMessagesIn = GUILayout.BeginScrollView(_scrollMessagesIn);
		_messagesIn = GUILayout.TextArea(_messagesIn, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.EndScrollView();
		
		_scrollMessagesOut = GUILayout.BeginScrollView(_scrollMessagesOut);
		_messagesOut = GUILayout.TextArea(_messagesOut, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.EndScrollView();
		
		EditorGUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Clear")) { _messagesIn = ""; }
			GUILayout.FlexibleSpace();
			if (_debug)
			{
				GUILayout.Label("Connected IN Devices : " + MidiINPlugin.GetConnectedDeviceCount().ToString());
				GUILayout.Label("Connected OUT Devices : " + MidiOUTPlugin.GetConnectedDeviceCount().ToString());
			}
			_debug = GUILayout.Toggle(_debug, "Debug");
		}
		GUILayout.EndHorizontal();
	}
	
	private readonly float _lineHeight = 25f;
	private Color _backgroundColor;
	private bool _refresh = false;
	
	private void DrawINOUT()
	{
		_backgroundColor = GUI.backgroundColor;

		int count = MidiINPlugin.deviceNames.Count > MidiOUTPlugin.deviceNames.Count ? MidiINPlugin.deviceNames.Count : MidiOUTPlugin.deviceNames.Count;

		if (count == 0) { EditorGUILayout.HelpBox("No MIDI connection found!!!", MessageType.Info); }

		var selectionHeight = count * (_lineHeight + 5);
		var dialogHeight = selectionHeight + 65;

		var defaultRect = new Rect(0, 0, window.position.width, dialogHeight);

		GUI.BeginGroup(defaultRect);

		var width = defaultRect.width;
		var halfWidth = width / 2f;

		GUI.Box(new Rect(-5f, -5f, defaultRect.width + 10, defaultRect.height + 10), "");

		GUILayout.BeginHorizontal();
		GUILayout.Box("Midi IN", GUILayout.Width(halfWidth));
		GUILayout.Box("Midi OUT", GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));

		var exists = false;

		for (var i = 0; i < count; i++)
		{
			GUILayout.BeginHorizontal();
			if (i < MidiINPlugin.deviceNames.Count)
			{
				exists = false;
				foreach (var inDevice in MidiINPlugin.connectedEditorDevices)
				{
					if (MidiINPlugin.deviceNames[i] != inDevice.name) continue;
					exists = true; break;
				}
				GUI.backgroundColor = exists ? Color.green : _backgroundColor;
				if (GUILayout.Button(MidiINPlugin.deviceNames[i], GUILayout.Width(halfWidth), GUILayout.Height(_lineHeight)))
				{
					if (exists) { MidiINPlugin.DisconnectDeviceByName(MidiINPlugin.deviceNames[i], true); }
					else { MidiINPlugin.ConnectDevice(i, true); }
					MidiINPlugin.StoreEditorConnections();
				}
				GUI.backgroundColor = _backgroundColor;
			}
			else { GUILayout.Label("", GUILayout.Width(halfWidth), GUILayout.Height(_lineHeight)); }

			if (i < MidiOUTPlugin.deviceNames.Count)
			{
				exists = false;
				foreach (var outDevice in MidiOUTPlugin.connectedEditorDevices)
				{
					if (MidiOUTPlugin.deviceNames[i] != outDevice.name) continue;
					exists = true; break;
				}
				GUI.backgroundColor = exists ? Color.green : _backgroundColor;
				if (GUILayout.Button(MidiOUTPlugin.deviceNames[i], GUILayout.Width(halfWidth), GUILayout.Height(_lineHeight)))
				{
					if (exists) { MidiOUTPlugin.DisconnectDeviceByName(MidiOUTPlugin.deviceNames[i], true); }
					else { MidiOUTPlugin.ConnectDevice(i, true); }
					MidiOUTPlugin.StoreEditorConnections();
				}
				GUI.backgroundColor = _backgroundColor;
			}
			else { GUILayout.Label("", GUILayout.Width(halfWidth), GUILayout.Height(_lineHeight)); }
			GUILayout.EndHorizontal();
		}

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Refresh", GUILayout.Width(halfWidth))) _refresh = true;
		if (GUILayout.Button("Reset", GUILayout.Width(halfWidth))) { MidiOut.AllSoundOff(); MidiOut.ResetAllControllers(); }
		GUILayout.EndHorizontal();
		GUI.EndGroup();

		if (_refresh) { _refresh = false; MidiINPlugin.Refresh(); MidiOUTPlugin.Refresh(); }
	}
}