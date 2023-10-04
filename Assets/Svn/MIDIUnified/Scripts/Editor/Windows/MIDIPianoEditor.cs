using System.IO;
using ForieroEditor.Extensions;
using ForieroEngine.MIDIUnified;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MIDIPianoEditor : EditorWindow
{
	[MenuItem("Foriero/Tools/MIDI/Piano &p", false, -2000)]
	private static void MIDIPianoEditorMenu()
	{
		window = EditorWindow.GetWindow(typeof(MIDIPianoEditor)) as MIDIPianoEditor;
		window.titleContent = new GUIContent("MIDI Piano");
		window.ShowAuxWindow();
	}

	public static MIDIPianoEditor window;
	private static string _assetsFolder = null;
	private static VisualElement[] _keys = new VisualElement[128];

	private static string PianoAssetFolder
	{
		get
		{
			if (_assetsFolder != null) return _assetsFolder;
			_assetsFolder = EditorPrefs.GetString("PIANO_UI_ELEMENTS", null);
			if (!string.IsNullOrEmpty(_assetsFolder)) return _assetsFolder;
				
			var path = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
			var files = Directory.GetFiles(path, "piano_ui_elements*.*", SearchOption.AllDirectories);
			_assetsFolder = Path.GetDirectoryName(files[0]);
			_assetsFolder = _assetsFolder.GetAssetPathFromFullPath();
			EditorPrefs.SetString("PIANO_UI_ELEMENTS", _assetsFolder);

			return _assetsFolder;
		}
	}

	[InitializeOnLoadMethod]
	private static void AutoInit()
	{
		
	}

	public static void NoteOn(int index)
	{
		if (_keys[index] == null) return;
		_keys[index].style.unityBackgroundImageTintColor = index.ToMidiColor();
	}

	public static void NoteOff(int index)
	{
		if (_keys[index] == null) return;
		_keys[index].style.unityBackgroundImageTintColor = Color.white;
	}

	private void CreateGUI()
	{
		// Each editor window contains a root VisualElement object.
 		var root = rootVisualElement;
        var pianoVT = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PianoAssetFolder + "/piano_ui_elements.uxml");
        if(pianoVT == null) Debug.LogError("Piano UIElements VisualTreeAsset is NULL!");
        var pianoSS = AssetDatabase.LoadAssetAtPath<StyleSheet>(PianoAssetFolder + "/piano_ui_elements.uss");
        if(pianoSS == null) Debug.LogError("Piano UIElements StyleSheet is NULL!");
        root.styleSheets.Add(pianoSS);
        pianoVT.CloneTree(root);

        var whiteKeyContainers = root.Query(className: "white_key_container");
        whiteKeyContainers.ForEach((v) => {
	        var n = int.Parse(v.name) + 12;
	        _keys[n] = v.Query(className:"white_key").First();
	        _keys[n].RegisterCallback<PointerDownEvent, int>((evt, index) =>
	        {
		        if (_keys[index] == null) return;
		        _keys[index].style.unityBackgroundImageTintColor = index.ToMidiColor();
				MidiOut.NoteOn(index);
	        }, n);
	        
	        _keys[n].RegisterCallback<PointerUpEvent, int>((evt, index) =>
	        {
		        if (_keys[index] == null) return;
		        _keys[index].style.unityBackgroundImageTintColor = Color.white;
		        MidiOut.NoteOff(index);
	        }, n);
        });

        var blackKeyContainers = root.Query(className: "black_key_container");
        blackKeyContainers.ForEach((v) =>
        {
	        var n = int.Parse(v.name) + 12;
	        _keys[n] = v.Query(className:"black_key").First();
	        _keys[n].RegisterCallback<PointerDownEvent, int>((evt, index) =>
	        {
		        if (_keys[index] == null) return;
		        _keys[index].style.unityBackgroundImageTintColor = index.ToMidiColor();
		        MidiOut.NoteOn(index);
	        }, n);
	        
	        _keys[n].RegisterCallback<PointerUpEvent, int>((evt, index) =>
	        {
		        if (_keys[index] == null) return;
		        _keys[index].style.unityBackgroundImageTintColor = Color.white;
		        MidiOut.NoteOff(index);
	        }, n);
        });

        root.Bind(new SerializedObject(this));
	}
}