using ForieroEditor;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;

public partial class MIDISoundFontSettings : ScriptableObject
{
	[MenuItem("Foriero/Settings/Midi/SoundFont")]
	public static void MIDISoundFontSettingsMenu()
	{
		MIDISoundFontSettings s = MIDISoundFontSettings.instance;
		EditorGUIUtility.PingObject(s);
		Selection.objects = new Object[1] { s };
	}

	static MIDISoundFontSettings _instance;

	public static MIDISoundFontSettings instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Instances.EditorInstance<MIDISoundFontSettings>(typeof(MIDISoundFontSettings).Name, "Assets/Editor");
			}

			if (_instance.defaultSoundFont == null)
			{
				string[] found = AssetDatabase.FindAssets("t:TextAsset DefaultSoundFont.sf2");
				if (found.Length > 0)
				{
					_instance.defaultSoundFont = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(found[0]));
				}
			}

			return _instance;
		}
	}

	[Header("Default SoundFont")]
	public TextAsset defaultSoundFont;

	[Header("Platform SoundFonts")]
	public TextAsset android;
	public TextAsset ios;
	public TextAsset wsa;
	public TextAsset windows;
	public TextAsset osx;
	public TextAsset linux;

	public TextAsset GetPlatformSoundFont()
	{
		TextAsset result = null;

		result = defaultSoundFont;

		if (result == null)
		{
			string[] found = AssetDatabase.FindAssets("t:TextAsset DefaultSoundFont.sf2");
			if (found.Length > 0)
			{
				result = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(found[0]));
			}
		}

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		if (osx) result = osx;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if(windows) result = windows;
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
        if(linux) result = linux;
#elif UNITY_WSA
        if(wsa) result = wsa;
#elif UNITY_ANDROID
        if(android) result = android;
#elif UNITY_IOS
        if(ios) result = ios;
#endif
		return result;
	}

	public string GetPlatformSoundFontAssetPath()
	{
		return AssetDatabase.GetAssetPath(GetPlatformSoundFont());
	}

	public string GetPlatformSoundFontFullPath()
	{
		return GetPlatformSoundFontAssetPath().GetFullPathFromAssetPath();
	}
}