using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

public static partial class Lang
{

    public static LangSettings settings
    {
        get
        {
            return LangSettings.instance;
        }
    }

    public static bool debug
    {
        get { return ForieroDebug.Languages; }
        set { ForieroDebug.Languages = value; }
    }

    private static LanguageCode? _selectedLanguage = null;
    private static readonly string SELECTED_LANGUAGE = "SELECTED_LANGUAGE";

    public static LanguageCode selectedLanguage
    {
        set
        {
            if (ForieroDebug.Languages) Debug.Log($"Lang | SELECTED : {value}");
            
            _selectedLanguage = value;

            PlayerPrefs.SetInt(SELECTED_LANGUAGE, (int)_selectedLanguage);

            if (Application.isPlaying) OnLanguageChange?.Invoke();             
        }
        get
        {
            if (_selectedLanguage == null)
            {
                _selectedLanguage = (LanguageCode)PlayerPrefs.GetInt(SELECTED_LANGUAGE, (int)LanguageCode.Unassigned);
                if (ForieroDebug.Languages) Debug.Log($"Lang | RESTORED : {_selectedLanguage}");
            }
            return (Lang.LanguageCode)(_selectedLanguage == LanguageCode.Unassigned ? LangSettings.instance.defaultLanguage : _selectedLanguage);
        }
    }

    public delegate void LanguageChangeEvent();

    public static event LanguageChangeEvent OnLanguageChange;

    public static List<LangDictionary> dictionaries = new List<LangDictionary>();

    public static bool DictionaryExists(string anAliasName)
    {
        foreach (LangDictionary d in dictionaries)
        {
            if (d.aliasName.Equals(anAliasName))
            {
                return true;
            }
        }
        return false;
    }

    public static LangDictionary GetDictionary(string anAliasName)
    {
        foreach (LangDictionary d in dictionaries)
        {
            if (d.aliasName.Equals(anAliasName))           
                return d;            
        }

        if (ForieroDebug.Languages)    
            Debug.LogError($"Lang | DICTIONARY NOT FOUND : {anAliasName}");        

        return null;
    }

    public static LangDictionary AddDictionary(string anAliasName, TextAsset textAsset)
    {
        if (DictionaryExists(anAliasName))        
            dictionaries.Remove(GetDictionary(anAliasName));
        

        LangDictionary result = new LangDictionary(anAliasName, textAsset);
        dictionaries.Add(result);

        if (ForieroDebug.Languages)        
            Debug.Log($"Lang | DICTIONARY ADDED : {anAliasName} : RESOURCE PATH : {textAsset.name}");
        
        return result;
    }
       
    /// <summary>
    /// Get translated text.
    /// </summary>
    /// <returns>The text.</returns>
    /// <param name="aDictionaryAliasName">A dictionary alias name. You can find alias names in Resources/Settings/LangSettings.asset in inspector tab Dictionaries.</param>
    /// <param name="anId">An record identifier. This is the ID column in google doc and then lately when parsed id column in csv.</param>
    /// <param name="aDefaultValue">A default value in case record was not found.</param>
    public static string GetText(string aDictionaryAliasName, string anId, string aDefaultValue = "", bool removeActorTag = true, bool removeInterjections = false)
    {
        if (DictionaryExists(aDictionaryAliasName))
        {
            LangDictionary dictionary = GetDictionary(aDictionaryAliasName);
            return dictionary.GetText(anId, aDefaultValue, removeActorTag, removeInterjections);
        }
        else
        {
            if (ForieroDebug.Languages)
            {
                Debug.LogError(
                    "Lang | DICTIONARY NOT FOUND - GET TEXT : " + (string.IsNullOrEmpty(anId) ? "EMPTY ID" : anId)
                    + "\n" + "IN DICTIONARY : " + (string.IsNullOrEmpty(aDictionaryAliasName) ? "EMPTY ALIASNAME" : aDictionaryAliasName)
                    + "\n" + "DEFAULT VALUE : " + (string.IsNullOrEmpty(aDefaultValue) ? "EMPTY DEFAULT VALUE" : aDefaultValue)
                );
            }
            return aDefaultValue;
        }
    }

    /// <summary>
    /// Get translated text.
    /// </summary>
    /// <returns>The text.</returns>
    /// <param name="aDictionaryAliasName">A dictionary alias name. You can find alias names in Resources/Settings/LangSettings.asset in inspector tab Dictionaries.</param>
    /// <param name="langCode">LanguageCode to retrieve language you need.</param>
    /// <param name="anId">An record identifier. This is the ID column in google doc and then lately when parsed id column in csv.</param>
    /// <param name="aDefaultValue">A default value in case record was not found.</param>
    public static string GetText(string aDictionaryAliasName, LanguageCode aLangCode, string anId, string aDefaultValue = "", bool removeActor = false)
    {
        if (DictionaryExists(aDictionaryAliasName))
        {
            LangDictionary dictionary = GetDictionary(aDictionaryAliasName);
            return dictionary.GetText(aLangCode, anId, aDefaultValue, removeActor);
        }
        else
        {
            if (ForieroDebug.Languages)
            {
                Debug.LogError(
                    "Lang | DICTIONARY NOT FOUND - GET TEXT : " + (string.IsNullOrEmpty(anId) ? "EMPTY ID" : anId)
                    + "\n" + "IN DICTIONARY : " + (string.IsNullOrEmpty(aDictionaryAliasName) ? "EMPTY ALIASNAME" : aDictionaryAliasName)
                    + "\n" + "DEFAULT VALUE : " + (string.IsNullOrEmpty(aDefaultValue) ? "EMPTY DEFAULT VALUE" : aDefaultValue)
                );
            }
            return aDefaultValue;
        }
    }

    private static readonly Regex ActorTagRegex = new Regex(@"\[(.*?)\]", RegexOptions.Compiled);
    public static string RemoveActorTag(this string t) => ActorTagRegex.Replace(t, "");
    
    private static readonly Regex InterjectionsRegex = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);
    public static string RemoveInterjections(this string t) => InterjectionsRegex.Replace(t, "");
    
    public class LangDictionary
    {
        public TextAsset textAsset = null;
        public string aliasName = "";

        public Dictionary<LanguageCode, Dictionary<string, string>> dictionary = new Dictionary<LanguageCode, Dictionary<string, string>>();

        public List<LanguageCode> languages = new List<LanguageCode>();

        public LangDictionary(string anAliasName, TextAsset textAsset)
        {
            this.aliasName = anAliasName;
            this.textAsset = textAsset;
        }

        public int ItemCount()
        {
            InitDictionary();
            return dictionary.Count;
        }

        // FOR Lang.selectedLanguage //
        public string GetText(string anId, string aDefaultValue = "", bool removeActorTag = true, bool removeInterjections = false)
        {            
            if (selectedLanguage == LanguageCode.Unassigned)
            {
                if (ForieroDebug.Languages)
                    Debug.LogWarning($"Returning default value for Lang.selectedLanguage : {Lang.selectedLanguage}");                
                return aDefaultValue;
            }
            else
            {
                if (!dictionary.ContainsKey(selectedLanguage)) InitDictionary();

                if (dictionary[selectedLanguage].ContainsKey(anId))
                {
                    var r = 
                        dictionary[selectedLanguage][anId]
                        .Replace(@"\n", "\n")
                        .Replace(@"\r", "\r");
                    
                    r = removeActorTag ? r.RemoveActorTag() : r;
                    r = removeInterjections ? r.RemoveInterjections() : r.Replace("{", "").Replace("}", "");
                    return r;
                }
                else
                {
                    if (ForieroDebug.Languages)
                    {
                        Debug.LogError("Lang | RECORD NOT FOUND - GET TEXT : " + (string.IsNullOrEmpty(anId) ? "EMPTY ID" : anId)
                        + "\n" + "DEFAULT VALUE : " + (string.IsNullOrEmpty(aDefaultValue) ? "EMPTY DEFAULT VALUE" : aDefaultValue)
                        + "\n" + "DICTIONARY ALIAS : " + aliasName
                        + "\n" + "LANG CODE : " + selectedLanguage.ToString()
                        );
                    }
                    return aDefaultValue;
                }
            }            
        }

        public bool Initialized()
        {
            return dictionary.ContainsKey(selectedLanguage);
        }

        public bool Initialized(LanguageCode aLangCode)
        {
            return dictionary.ContainsKey(aLangCode);
        }

        public string GetText(LanguageCode aLangCode, string anId, string aDefaultValue = "", bool removeActorTag = true, bool removeInterjections = false)
        {
            var r = "";
            if (aLangCode == LanguageCode.Unassigned)
            {
                if (ForieroDebug.Languages)
                    Debug.LogWarning($"Lang | Returning default value for language : {aLangCode}");                
                return aDefaultValue;
            }
            else
            {
                if (!dictionary.ContainsKey(aLangCode)) InitDictionary(aLangCode);

                if (dictionary[aLangCode].ContainsKey(anId))
                {
                    r = 
                        dictionary[aLangCode][anId]
                        .Replace(@"\n", "\n")
                        .Replace(@"\r", "\r");
                    
                    r = removeActorTag ? r.RemoveActorTag() : r;
                    r = removeInterjections ? r.RemoveInterjections() : r.Replace("{", "").Replace("}", "");
                    return r;
                }
                else
                {
                    if (ForieroDebug.Languages)
                    {
                        Debug.LogError("Lang | RECORD NOT FOUND - GET TEXT : " + (string.IsNullOrEmpty(anId) ? "EMPTY ID" : anId)
                        + "\n" + "DEFAULT VALUE : " + (string.IsNullOrEmpty(aDefaultValue) ? "EMPTY DEFAULT VALUE" : aDefaultValue)
                        + "\n" + "DICTIONARY ALIAS : " + aliasName
                        + "\n" + "LANG CODE : " + aLangCode.ToString()
                        );
                    }
                    return aDefaultValue;
                }
            }
        }

        public string FindID(LanguageCode aLangCode, string recordValue, bool caseSensitive = false)
        {
            if (aLangCode == LanguageCode.Unassigned)
            {
                if (ForieroDebug.Languages)                
                    Debug.LogWarning($"Lang | Returning default value for language : {aLangCode}");                
                return "";
            }
            else
            {
                if (!dictionary.ContainsKey(aLangCode))
                    InitDictionary(aLangCode);

                foreach (KeyValuePair<string, string> kvp in dictionary[aLangCode])
                {
                    if (caseSensitive)
                    {
                        if (kvp.Value == recordValue)
                            return kvp.Key;
                    }
                    else
                    {
                        if (kvp.Value.ToLower() == recordValue.ToLower())
                            return kvp.Key;
                    }
                }
            }

            return "";
        }

        public void InitDictionary(bool force = false) => InitDictionary(selectedLanguage, force);
        public void InitDictionary(LanguageCode aLangCode, bool force = false)
        {
            if (!force && Initialized(selectedLanguage)) return;
            
            if (aLangCode == LanguageCode.Unassigned)
                if (ForieroDebug.Languages) { Debug.LogWarning($"Lang | Can not initialize Unassigned language!"); }
            
            if (ForieroDebug.Languages)
                Debug.Log($"Lang | DICTIONARY INITIALIZED : {aliasName} : ASSET PATH : {textAsset.name} LANGUAGE : {aLangCode}");
            
            if (dictionary.ContainsKey(aLangCode))
                dictionary[aLangCode] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);            
            else
                dictionary.Add(aLangCode, new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase));
            
            languages.Clear();

            string id = "";
            string line = "";
            string[] defs = new string[0];
            string[] values = new string[0];

            using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            {
                using (StreamReader sr = new StreamReader(stream))
                {

                    line = sr.ReadLine();
                    defs = line.ToUpper().Split(";".ToCharArray()[0]);
                    for (int i = 1; i < defs.Length; i++)
                    {
                        languages.Add(GetLanguageCodeFromTwoLetterISOLanguageName(defs[i]));
                    }

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        line = line.Replace(@"\;", "_semicolon");
                        values = line.Split(";".ToCharArray()[0]);
                        for (int k = 0; k < languages.Count + 1; k++)
                        {
                            if (k == 0) { id = values[k]; }
                            else
                            {
                                if (aLangCode == languages[k - 1])
                                {
                                    if (dictionary[aLangCode].ContainsKey(id))
                                    {
                                        if (ForieroDebug.Languages)                                    
                                            Debug.LogError($"Lang | DICTIONARY : {aliasName} ALREADY CONTAINS KEY : {id}");
                                    }
                                    else
                                    {
                                        try
                                        {
                                            dictionary[aLangCode].Add(id, values[k]
                                                .Replace("_semicolon", ";")
                                                .Replace(@"\n", "\n")
                                                .Replace(@"\r", "\r")
                                            );
                                        }
                                        catch (Exception e)
                                        {
                                            if (ForieroDebug.Languages)                                            
                                                Debug.LogError($"Lang | DICTIONARY : {aliasName} KEY : {id} {e.Message}");                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static LanguageCode GetOSLanguageCode()
    {
        LanguageCode r = Application.systemLanguage.SystemLanguageToLangCode();

        if (r == LanguageCode.Unassigned) r = GetLanguageCodeFromTwoLetterISOLanguageName(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        
        return r;
    }

    public static string GetLanguageString(this LanguageCode aLangCode)
    {
        if (aLangCode == LanguageCode.Unassigned) return "Unassigned";
        var r = "Unsupported";
        try
        {
            r = new CultureInfo(aLangCode == LanguageCode.ZH ? "zh-CN" : aLangCode.ToString()).NativeName;
        }
        catch { r = "Unsupported"; }
        return r;
    }

    public static LanguageCode GetLangCodeFromLanguageString(this string langString)
    {
        var r = LanguageCode.Unassigned;
        foreach (LanguageCode item in System.Enum.GetValues(typeof(LanguageCode)))
        {
            if (item == LanguageCode.Unassigned) continue;
            if (string.Equals(langString, item.GetLanguageString())) { r = item; break; }
        }
        return r;
    }

    public static LanguageCode GetLanguageCodeFromTwoLetterISOLanguageName(this string langCode)
    {
        langCode = langCode.ToUpper().Trim().Substring(0, 2);
        foreach (LanguageCode item in System.Enum.GetValues(typeof(LanguageCode))) { if (item.ToString() == langCode) return item; }
        if (ForieroDebug.Languages) Debug.LogWarning($"Lang | There is no language: [{langCode}]");
        return LanguageCode.Unassigned;
    }
}
