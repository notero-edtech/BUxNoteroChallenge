using System;
using System.IO;
using System.Text.RegularExpressions;
using ForieroEditor.CommandLine;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;
using static ForieroEditor.Extensions.ForieroEditorExtensions;
using Debug = UnityEngine.Debug;

public partial class LangVoiceGenerator : EditorWindow
{
    static string _cmdRhubarb = null;
    static string cmdRhubarb
    {
        get
        {
#if UNITY_EDITOR_OSX
            if (_cmdRhubarb == null) _cmdRhubarb = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "rhubarb", SearchOption.AllDirectories)[0];
#elif UNITY_EDITOR_WIN
            if (_cmdRhubarb == null) _cmdRhubarb = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "rhubarb.exe", SearchOption.AllDirectories)[0];
#endif
            return _cmdRhubarb;
        }
    }

    public static Texture _rhubarb_a;
    public static Texture _rhubarb_b;
    public static Texture _rhubarb_c;
    public static Texture _rhubarb_d;
    public static Texture _rhubarb_e;
    public static Texture _rhubarb_f;
    public static Texture _rhubarb_g;
    public static Texture _rhubarb_h;
    public static Texture _rhubarb_x;

    public static Texture rhubarb_a { get { LoadRhubarbTextures(); return _rhubarb_a; } }
    public static Texture rhubarb_b { get { LoadRhubarbTextures(); return _rhubarb_b; } }
    public static Texture rhubarb_c { get { LoadRhubarbTextures(); return _rhubarb_c; } }
    public static Texture rhubarb_d { get { LoadRhubarbTextures(); return _rhubarb_d; } }
    public static Texture rhubarb_e { get { LoadRhubarbTextures(); return _rhubarb_e; } }
    public static Texture rhubarb_f { get { LoadRhubarbTextures(); return _rhubarb_f; } }
    public static Texture rhubarb_g { get { LoadRhubarbTextures(); return _rhubarb_g; } }
    public static Texture rhubarb_h { get { LoadRhubarbTextures(); return _rhubarb_h; } }
    public static Texture rhubarb_x { get { LoadRhubarbTextures(); return _rhubarb_x; } }

    static void LoadRhubarbTextures()
    {
        if (_rhubarb_a) return;

        var mouthPath = Path.GetDirectoryName(cmdRhubarb).FixOSPath().GetAssetPathFromFullPath() + "/mouth/";
        Debug.Log(mouthPath);
        _rhubarb_a = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_a_p_b_m.png");
        _rhubarb_b = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_b_k_s_t_ee.png");
        _rhubarb_c = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_c_eh_ae.png");
        _rhubarb_d = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_c_eh_ae.png");
        _rhubarb_e = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_e_ao_er.png");
        _rhubarb_f = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_f_uw_ow.png");
        _rhubarb_g = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_g_f_v.png");
        _rhubarb_h = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_h_l.png");
        _rhubarb_x = AssetDatabase.LoadAssetAtPath<Texture>(mouthPath + "rhubarb_x.png");
    }

    public static void GeneratePhonemes(string audioFullPath, string text)
    {
        if (!File.Exists(audioFullPath)) {
            Debug.Log("Audiofile does not exists : " + audioFullPath);
        }

        string spineTempDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/Rhubarb").FixOSPath();

        if (!Directory.Exists(spineTempDirectory)) Directory.CreateDirectory(spineTempDirectory);

        string fileName = Path.GetFileNameWithoutExtension(audioFullPath);
        string fileDirectory = Path.GetDirectoryName(audioFullPath);

        string guid = Guid.NewGuid().ToString();

        string textFullPath = Path.Combine(spineTempDirectory, fileName + "_" + guid + ".txt").FixOSPath();

        text = Regex.Replace(text, @" ?\[.*?\]", string.Empty);

        File.WriteAllText(textFullPath, text ?? "");
                
        string ouputFullPath = Path.Combine(fileDirectory, fileName + ".txt");

        var cmd = cmdRhubarb.DoubleQuotes();
        var args = "-f tsv -d " + textFullPath.DoubleQuotes() + " --extendedShapes GHX -o " + ouputFullPath.DoubleQuotes() + " " + audioFullPath.DoubleQuotes();

        Debug.Log(cmd + " " + args);

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        CMD.Bash("chmod +x " + cmdRhubarb.DoubleQuotes());
        CMD.Bash(cmd + " " + args);
#endif
    }
        
    public static string GetClipAssetPathEx(string guid, string dictionary, Lang.LanguageCode langCode, string recordId)
    {
        return "Assets/Resources Localization/" + guid + "/LanguageAudios/" + dictionary + "/" + langCode + "/" + recordId + ".ogg";
    }

    public static string GetPhonemesAssetPathEx(string guid, string dictionary, Lang.LanguageCode langCode, string recordId)
    {
        return GetClipAssetPathEx(guid, dictionary, langCode, recordId).Replace(".ogg", ".txt");
    }

    public static string GetWordsAssetPathEx(string guid, string dictionary, Lang.LanguageCode langCode, string recordId)
    {
        return GetClipAssetPathEx(guid, dictionary, langCode, recordId).Replace(".ogg", ".xml");
    }
}