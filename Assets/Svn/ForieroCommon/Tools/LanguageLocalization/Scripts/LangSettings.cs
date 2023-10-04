using UnityEngine;
using System.Collections.Generic;
using ForieroEngine.Settings;
using PlayerPrefs = ForieroEngine.PlayerPrefs;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class LangSettings : Settings<LangSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Language", false, -1000)] public static void LangSettingsMenu() => Select();
#endif

#if UNITY_EDITOR
    [Serializable]
    public class FTPAuth
    {
        public string user;
        [NonSerialized] public string paswd;
        public int port;
        public string host;
        public string hostpath;       
    }

    public FTPAuth ftpAuth;
#endif

    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if (initialized) return;

        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
        if (PlayerPrefs.HasKey<bool>("FORIERO_DEBUG_LANGUAGES"))
        {
            ForieroDebug.Languages = PlayerPrefs.GetInt("FORIERO_DEBUG_LANGUAGES", ForieroDebug.Languages ? 1 : 0) == 1 ? true : false;
        }
          
        Debug.Log($"LANG | {Lang.selectedLanguage}");
        
        if (instance.forceLanguage) { Lang.selectedLanguage = instance.forcedLanguage; }
        else
        {
            if (Lang.selectedLanguage == Lang.LanguageCode.Unassigned)
            {
                Lang.LanguageCode lc = Lang.GetOSLanguageCode();

                if (instance.IsSupportedLanguage(lc))
                {
                    Lang.selectedLanguage = lc;
                }
                else
                {
                    Lang.selectedLanguage = instance.defaultLanguage;
                }

                if (ForieroDebug.Languages) Debug.Log("Setting up OS or DEFAULT Languages : " + Lang.selectedLanguage.ToString());
            }
            else
            {
                if (!instance.IsSupportedLanguage(Lang.selectedLanguage))
                {
                    Lang.selectedLanguage = instance.defaultLanguage;
                }
            }
        }

        foreach (LangSettings.LangDictionary ld in instance.dictionaries)
        {
            if (ld.textAsset == null)
            {
                if (ForieroDebug.Languages)
                {
                    Debug.LogError("Dictionary TextAsset in NULL : " + ld.aliasName);
                }
            }
            else
            {
                Lang.AddDictionary(ld.aliasName, ld.textAsset);
            }
        }
        
        if(ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (LanguageSettings - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        
        initialized = true;
    }

    public void SetDebug(bool value)
    {
        UnityEngine.PlayerPrefs.SetInt("FORIERO_DEBUG_LANGUAGES", value ? 1 : 0);
        ForieroDebug.Languages  = value;
    }

   
        
    [System.Serializable]
    public class LangItem
    {
        public Lang.LanguageCode langCode = Lang.LanguageCode.Unassigned;
        public bool included = true;
    }

    [System.Serializable]
    public class LangDictionary
    {
        public enum Storage
        {
            InBuild,
            OnServer
        }

        public string aliasName = "";
        public TextAsset textAsset;
        public Storage stored = Storage.InBuild;        
    }

    [System.Serializable]
    public class LangActor
    {
        public string name = "";
        public LangActorVoice[] langActorVoices = new LangActorVoice[0];
    }
       
    [System.Serializable]
    public class GoogleDoc
    {
        public string fileName = "";
        public string publicKey = "";
        public bool prependFileName = true;

        public string googleURL
        {
            get
            {
                return "https://docs.google.com/spreadsheets/d/" + publicKey + "/edit";
            }
        }

        public string odsURL
        {
            get
            {
                return "https://docs.google.com/spreadsheets/d/" + publicKey + "/pub?output=ods";
            }
        }

        public string xlsxURL
        {
            get
            {
                return "https://docs.google.com/spreadsheets/d/" + publicKey + "/pub?output=xlsx";
            }
        }

        public string csvURL
        {
            get
            {
                return "https://docs.google.com/spreadsheets/d/" + publicKey + "/pub?output=csv";
            }
        }

        public string odsAssetsPath
        {
            get
            {
                return "Assets/Resources Localization/" + instance.guid + "/" + fileName + ".ods";
            }
        }

        public string xlsxAssetsPath
        {
            get
            {
                return "Assets/Resources Localization/" + instance.guid + "/" + fileName + ".xlsx";
            }
        }

        public string csvAssetsPath
        {
            get
            {
                return "Assets/Resources Localization/" + instance.guid + "/" + fileName + ".csv";
            }
        }
    }
   
    public bool test = false;

    public string guid = "";

    public Lang.LanguageCode defaultLanguage;
    public bool forceLanguage = false;
    public Lang.LanguageCode forcedLanguage = Lang.LanguageCode.EN;

    public string voURLBase = "https://backend.foriero.com/unity/voice_over/";
    public string voProjectName = "";
    public string voVersion = "";

    public List<LangItem> supportedLanguages = new List<LangItem>();
    public List<GoogleDoc> googleDocs = new List<GoogleDoc>();
    public List<LangDictionary> dictionaries = new List<LangDictionary>();
    public List<LangActor> actors = new List<LangActor>();

    public bool IsSupportedLanguage(Lang.LanguageCode langCode)
    {
        foreach (LangItem item in supportedLanguages)
        {
            if (item.included && item.langCode == langCode)
                return true;
        }
        return false;
    }

    public string GetVoiceOverURL()
    {
        return voURLBase + (voURLBase.EndsWith("/") ? "" : "/") + voProjectName + "/" + voVersion + "/";
    }
}
