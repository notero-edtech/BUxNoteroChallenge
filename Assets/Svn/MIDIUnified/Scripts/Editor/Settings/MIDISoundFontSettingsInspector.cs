using UnityEngine;
using UnityEditor;
using System.IO;
using ForieroEditor.Extensions;
using ForieroEngine.MIDIUnified.Synthesizer;

[CustomEditor(typeof(MIDISoundFontSettings))]
[CanEditMultipleObjects()]
public class MIDISoundFontSettingsInspector : Editor
{
    MIDISoundFontSettings o;

    static readonly string soundfontResourcesPath = "Assets/Resources/soundfont.sf2.bytes";
    static readonly string soundfontPersistentPath = "soundfont.sf2";

    static string lastGUID
    {
        get { return EditorPrefs.GetString("SOUNDFONT_RESOURCES", ""); }
        set { EditorPrefs.SetString("SOUNDFONT_RESOURCES", value); }
    }

    public void OnEnable()
    {
        o = target as MIDISoundFontSettings;
    }

    public override void OnInspectorGUI()
    {
        var color = GUI.backgroundColor;

        serializedObject.Update();

        MIDISettingsInspector.DrawTabs(MIDISettingsInspector.Tab.SoundFont);

        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Initialize Resources", EditorStyles.toolbarButton))
        {
            CheckSoundFontInResources();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(soundfontResourcesPath));
        }

        if (GUILayout.Button("P", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(soundfontResourcesPath));
        }

        if (GUILayout.Button("O", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            EditorUtility.OpenWithDefaultApp(Path.GetDirectoryName(soundfontResourcesPath.GetFullPathFromAssetPath()));
        }


        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("D", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            AssetDatabase.DeleteAsset(soundfontResourcesPath);
        }
        GUI.backgroundColor = color;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Initialize Persistent Path", EditorStyles.toolbarButton))
        {
            var sf = MIDISoundFontSettings.instance.GetPlatformSoundFont();
            if (sf)
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, soundfontPersistentPath), sf.bytes);
            }
        }

        GUILayout.Box("", EditorStyles.toolbarButton, GUILayout.Width(25));

        if (GUILayout.Button("O", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            EditorUtility.OpenWithDefaultApp(Path.GetDirectoryName(Application.persistentDataPath));
        }


        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("D", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            string f = Path.Combine(Application.persistentDataPath, soundfontPersistentPath);
            if (File.Exists(f)) File.Delete(f);
        }
        GUI.backgroundColor = color;

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }

    public static void CheckSoundFontInResources()
    {
        if (!MIDISettings.instance.initialize) return;

        switch (MIDISynthSettings.GetPlatformSettings().GetSynthEnum())
        {
            case Synth.SynthEnum.NONE:
                DeleteSoundfontFromResources();
                break;
            case Synth.SynthEnum.NATIVE:
                DeleteSoundfontFromResources();
                break;
            case Synth.SynthEnum.BASS24:
                CopySoundfontToResources();
                break;
            case Synth.SynthEnum.CSHARP:
                CopySoundfontToResources();
                break;
#if MIDIUNIFIED_BETA
            case Synth.SynthEnum.FLUID:
                CopySoundfontToResources();
                break;
#endif
        }
    }

    static void CopySoundfontToResources()
    {
        if (MIDISettings.IsDebug)
        {
            Debug.Log("MU | SoundFontAssetPath | " + MIDISoundFontSettings.instance.GetPlatformSoundFontAssetPath());
            Debug.Log("MU | SoundFontFullPath | " + MIDISoundFontSettings.instance.GetPlatformSoundFontFullPath());
        }

        DuplicateAndCopyToResources(MIDISoundFontSettings.instance.GetPlatformSoundFont());
    }

    static void DeleteSoundfontFromResources()
    {
        AssetDatabase.DeleteAsset(soundfontResourcesPath);
    }

    static void DuplicateAndCopyToResources(TextAsset soundFont)
    {
        if (soundFont)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(soundFont));

            if (!File.Exists(soundfontResourcesPath.GetFullPathFromAssetPath()) || guid != lastGUID)
            {
                lastGUID = guid;
                AssetDatabase.DeleteAsset(soundfontResourcesPath);
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(soundFont), soundfontResourcesPath);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            Debug.LogError("MU | DuplicateAndCopyToResources | Soundfont TextAsset is null!!!");
        }
    }
}