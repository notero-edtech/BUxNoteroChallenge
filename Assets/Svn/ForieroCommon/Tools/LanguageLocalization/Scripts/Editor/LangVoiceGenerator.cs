using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ForieroEditor;
using ForieroEditor.Coroutines;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;

//Get List Of Voices : say -v '?'
//Command to say someting : say -v "voice name" -o "~/Desktop/hi.wav" --data-format=LEF32@22100 "hello"

public partial class LangVoiceGenerator : EditorWindow
{
    static LangVoiceGenerator _window;
    public static LangVoiceGenerator window
    {
        get
        {
            if (_window == null) Init();
            return _window;
        }
    }

    [MenuItem("Foriero/Lang/Language Voice Generator")]
    static void Init()
    {
        _window = EditorWindow.GetWindow(typeof(LangVoiceGenerator)) as LangVoiceGenerator;
        _window.titleContent = new GUIContent("Voice Generator");
    }

    static int dictionaryIndex = -1;
    static int dictionaryIndexTmp = -1;
    static string[] dictionaries = new string[0];

    static int fileLanguageIndex = -1;
    static int fileLanguageIndexTmp = -1;
    static string[] fileLanguageNames = new string[0];

    static Lang.LangDictionary selectedDictionary = null;

    static string idFilter = "";
    static string idFilterTmp = "";

    static string recordFilter = "";
    static string recordFilterTmp = "";

    static string[] records = new string[0];
    static string[] recordIds = new string[0];
    static int recordIndex = -1;
    static int recordIndexTmp = -1;

    public static VoiceService voiceService = VoiceService.OSX;

    public enum Tab
    {
        Phonemes,
        Words
    }

    public Tab tab = Tab.Phonemes;
    
    PhonemeClip phonemeClip = new PhonemeClip();
    WordsClip wordsClip = new WordsClip();

    string GetClipAssetPath() => GetClipAssetPathEx(LangSettings.instance.guid, dictionaries[dictionaryIndex], (Lang.LanguageCode)Enum.Parse(typeof(Lang.LanguageCode), fileLanguageNames[fileLanguageIndex]), recordIds[recordIndex]);
    string GetPhonemesAssetPath() => GetPhonemesAssetPathEx(LangSettings.instance.guid, dictionaries[dictionaryIndex], (Lang.LanguageCode)Enum.Parse(typeof(Lang.LanguageCode), fileLanguageNames[fileLanguageIndex]), recordIds[recordIndex]);
    string GetWordsAssetPath() => GetWordsAssetPathEx(LangSettings.instance.guid, dictionaries[dictionaryIndex], (Lang.LanguageCode)Enum.Parse(typeof(Lang.LanguageCode), fileLanguageNames[fileLanguageIndex]), recordIds[recordIndex]);

    public static string RemoveActorTag(string text) => System.Text.RegularExpressions.Regex.Replace(text, @" ?\[.*?\]", string.Empty).Trim();
    public static string RemoveInterjections(string text) => System.Text.RegularExpressions.Regex.Replace(text, @" ?\{.*?\}", string.Empty).Trim();
    public LangVoiceGenerator()
    {
        EditorApplication.update += EveryFrameUpdate;
    }

    
    private void EveryFrameUpdate()
    {
        switch (tab)
        {
            case Tab.Phonemes:
                phonemeClip?.EveryFrameUpdate();
                break;
            case Tab.Words:
                wordsClip?.EveryFrameUpdate();
                break;
        }
    }

    ~LangVoiceGenerator()
    {
        EditorApplication.update -= EveryFrameUpdate;
    }

    public class Indexer{
        public List<Voice> voices = new List<Voice>();

        public Voice voice{
            get{
                return voices.Where(l => l.name.Equals(name) && l.languageCodeRegion.Equals(language)).FirstOrDefault();
            }
        }

        public string name{
            get{
                if (voiceNameIndex >= 0 && voiceNameIndex < voiceNames.Length)
                {
                    return voiceNames[voiceNameIndex];
                }
                return "";
            }
        }

        public string language{
            get{
               
                if (voiceLanguageIndex >= 0 && voiceLanguageIndex < voiceLanguages.Length)
                {
                    return voiceLanguages[voiceLanguageIndex];
                }
                return "";
            }
        }

        public string[] voiceNames = new string[0];
        public int voiceNameIndex = -1;
        public int voiceNameIndexTmp = -1;

        public string[] voiceLanguages = new string[0];
        public int voiceLanguageIndex = -1;
        public int voiceLanguageIndexTmp = -1;

        public void UpdateLanguageRegions(string languageCode = ""){

            voiceLanguages = new string[0];
            voiceLanguageIndex = voiceLanguageIndexTmp = -1;

            voiceNames = new string[0];
            voiceNameIndex = voiceNameIndexTmp = -1;

            if (string.IsNullOrEmpty(languageCode))
            {
                if (fileLanguageIndex >= 0 && fileLanguageIndex < fileLanguageNames.Length)
                {
                    languageCode = fileLanguageNames[fileLanguageIndex];
                }
            }

            voiceLanguages = (from v in voices
                              where v.languageCodeRegion.Length >= 2 && v.languageCodeRegion.ToUpper().Substring(0, 2).Contains(languageCode)
                              select v.languageCodeRegion).Distinct().OrderBy(l => l).ToArray();

        }

        public void UpdateVoicesInLanguageRegion(string languageCodeRegion = ""){
            voiceNames = new string[0];
            voiceNameIndex = voiceNameIndexTmp = -1;

            if(string.IsNullOrEmpty(languageCodeRegion)){
                if(voiceLanguageIndex >= 0 && voiceLanguageIndex < voiceLanguages.Length){
                    languageCodeRegion = voiceLanguages[voiceLanguageIndex];
                }
            }

            voiceNames = (from v in voices
                          where v.languageCodeRegion.Length >= 2 && v.languageCodeRegion.ToUpper().Contains(languageCodeRegion.ToUpper())
                         select v.name).Distinct().OrderBy(l => l).ToArray();

        }

        public void Reset(){
            voiceNames = new string[0];
            voiceNameIndex = -1;
            voiceNameIndexTmp = -1;

            voiceLanguages = new string[0];
            voiceLanguageIndex = -1;
            voiceLanguageIndexTmp = -1;
        }
    }

    public static Indexer indexer{
        get{
            switch (voiceService)
            {
                case VoiceService.OSX: return OSX.indexer;
                case VoiceService.WINDOWS: return WINDOWS.indexer;
                case VoiceService.GOOGLE: return GOOGLE.indexer;
                case VoiceService.AMAZON: return AMAZON.indexer;
                case VoiceService.IBM: return AMAZON.indexer;
                case VoiceService.ALEXA: return AMAZON.indexer;
            }
            return null;
        }
        set
        {
            switch (voiceService)
            {
                case VoiceService.OSX: OSX.indexer = value; return;
                case VoiceService.WINDOWS: WINDOWS.indexer = value; return;
                case VoiceService.GOOGLE: GOOGLE.indexer = value; return;
                case VoiceService.AMAZON: AMAZON.indexer = value; return;
                case VoiceService.IBM: AMAZON.indexer = value; return;
                case VoiceService.ALEXA: AMAZON.indexer = value; return;
            }
        }
    }

    public static void GenerateVoice(VoiceService service, Voice voice, string path, string text = "")
    {
        switch (service)
        {
            case VoiceService.OSX: OSX.GenerateVoice(voice, path, text); return;
            case VoiceService.WINDOWS: WINDOWS.GenerateVoice(voice, path, text); return;
            case VoiceService.GOOGLE: GOOGLE.GenerateVoice(voice, path, text); return;
            case VoiceService.AMAZON: AMAZON.GenerateVoice(voice, path, text); return;
            case VoiceService.IBM: IBM.GenerateVoice(voice, path, text); return;
            case VoiceService.ALEXA: ALEXA.GenerateVoice(voice, path, text); return;
        }

    }

    public static void TestVoice(VoiceService service, Voice voice, string text = "")
    {
        switch (service)
        {
            case VoiceService.OSX: OSX.TestVoice(voice, text); return;
            case VoiceService.WINDOWS: WINDOWS.TestVoice(voice, text); return;
            case VoiceService.GOOGLE: GOOGLE.TestVoice(voice, text); return;
            case VoiceService.AMAZON: AMAZON.TestVoice(voice, text); return;
            case VoiceService.IBM: IBM.TestVoice(voice, text); return;
            case VoiceService.ALEXA: ALEXA.TestVoice(voice, text); return;
        }
    }

    public static List<Voice> GetVoices(VoiceService service, bool force = false)
    {
        switch (service)
        {
            case VoiceService.OSX: return OSX.GetVoices(force);
            case VoiceService.WINDOWS: return WINDOWS.GetVoices(force);
            case VoiceService.GOOGLE: return GOOGLE.GetVoices(force);
            case VoiceService.AMAZON: return AMAZON.GetVoices(force);
            case VoiceService.IBM: return IBM.GetVoices(force);
            case VoiceService.ALEXA: return ALEXA.GetVoices(force);
        }
        return null;
    }

    void OnEnable()
    {
        RefreshAll();
    }

    void OnDisable()
    {

    }

    void RefreshAll(){
        LangSettings.Init();
        dictionaries = Lang.dictionaries.Select(d => d.aliasName).ToArray();
        LangVoiceGenerator.GetVoices(voiceService, true);
    }

    Color backgroundColor = Color.white;
    bool generate = false;

    void OnGUI()
    {
        DrawSeparator();

        DrawToolbar();

        DrawSeparator();

        DrawLanguageFile();

        EditorGUI.BeginDisabledGroup(dictionaryIndex < 0);
        DrawLanguage();

        EditorGUI.BeginDisabledGroup(fileLanguageIndex < 0);
        DrawIDFilter();
        DrawRecordFilter();

        DrawSeparator();

        EditorGUI.EndDisabledGroup();
        EditorGUI.EndDisabledGroup();

        if (indexer.voiceLanguageIndex != (indexer.voiceLanguageIndexTmp = EditorGUILayout.Popup("Voice Region", indexer.voiceLanguageIndexTmp, indexer.voiceLanguages)))
        {
            indexer.voiceLanguageIndex = indexer.voiceLanguageIndexTmp;
            indexer.UpdateVoicesInLanguageRegion();
        }

        EditorGUILayout.BeginHorizontal();

        if (indexer.voiceNameIndex != (indexer.voiceNameIndexTmp = EditorGUILayout.Popup("Voice Name", indexer.voiceNameIndexTmp, indexer.voiceNames)))
        {
            indexer.voiceNameIndex = indexer.voiceNameIndexTmp;
        }

        EditorGUI.BeginDisabledGroup(recordIndex < 0 || fileLanguageIndex < 0 || indexer.voiceLanguageIndex < 0 || indexer.voiceNameIndex < 0);
        
        if (GUILayout.Button("Test", GUILayout.Width(50)))
        {
            selectedDictionary.InitDictionary();
            string text = selectedDictionary.GetText(Lang.selectedLanguage, recordIds[recordIndex], "", true);
            if (!string.IsNullOrEmpty(text))
            {
                TestVoice(voiceService, indexer.voice, text);
            }
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();

        switch (voiceService)
        {
            case VoiceService.OSX:
                generate = OSX.OnGUI();
                break;
            case VoiceService.WINDOWS:
                generate = WINDOWS.OnGUI();
                break;
            case VoiceService.GOOGLE:
                generate = GOOGLE.OnGUI();
                break;
            case VoiceService.AMAZON:
                generate = AMAZON.OnGUI();
                break;
        }

        DrawSeparator();

        DrawGenerate();

        DrawSeparator();

        EditorGUILayout.BeginHorizontal();
        {
            foreach (var t in System.Enum.GetValues(typeof(Tab)))
            {
                var t2 = (Tab)t;
                if (t2 == tab) GUI.backgroundColor = Color.green;
                if (GUILayout.Button(t2.ToString(), EditorStyles.toolbarButton))
                {
                    tab = t2;
                }
                GUI.backgroundColor = backgroundColor;
            }
        }
        EditorGUILayout.EndHorizontal();

        switch (tab)
        {
            case Tab.Phonemes:
                phonemeClip?.OnGUI();
                break;
            case Tab.Words:
                wordsClip?.OnGUI();
                break;
        }
    }

    static void DrawSeparator(int height = 2, int width = -1){
        if (width > 0)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height), GUILayout.Width(width));
        }
        else
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
        }
    }

    void DrawToolbar()
    {
        backgroundColor = GUI.backgroundColor;

        EditorGUILayout.BeginHorizontal();

        foreach (VoiceService serviceTab in System.Enum.GetValues(typeof(VoiceService)))
        {
            if (serviceTab == voiceService)
            {
                GUI.backgroundColor = Color.green;
            }

            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX && serviceTab == VoiceService.WINDOWS) GUI.enabled = false;
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows && serviceTab == VoiceService.OSX) GUI.enabled = false;

            if (GUILayout.Button(serviceTab.ToString(), EditorStyles.toolbarButton))
            {
                voiceService = serviceTab;
                LangVoiceGenerator.GetVoices(voiceService);
                indexer.UpdateLanguageRegions();
            }

            GUI.enabled = true;

            GUI.backgroundColor = backgroundColor;
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("REFRESH", EditorStyles.toolbarButton))
        {
            RefreshAll();
            dictionaryIndex = dictionaryIndexTmp = -1;
            fileLanguageIndex = fileLanguageIndexTmp = -1;
            recordIndex = recordIndexTmp = -1;
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawLanguageFile()
    {
        if (dictionaryIndex != (dictionaryIndexTmp = EditorGUILayout.Popup("Language File", dictionaryIndexTmp, dictionaries)))
        {
            dictionaryIndex = dictionaryIndexTmp;

            fileLanguageIndex = fileLanguageIndexTmp = -1;
            recordIndex = recordIndexTmp = -1;

            selectedDictionary = Lang.GetDictionary(dictionaries[dictionaryIndex]);
            selectedDictionary.InitDictionary();

            fileLanguageNames = new string[selectedDictionary.languages.Count];
            for (int i = 0; i < fileLanguageNames.Length; i++)
            {
                fileLanguageNames[i] = selectedDictionary.languages[i].ToString();
            }

            indexer.UpdateLanguageRegions();
        }
    }

    static void DrawLanguage()
    {
        if (fileLanguageIndex != (fileLanguageIndexTmp = EditorGUILayout.Popup("Language", fileLanguageIndexTmp, fileLanguageNames)))
        {
            fileLanguageIndex = fileLanguageIndexTmp;
                       
            recordIndex = recordIndexTmp = -1;

            Lang.selectedLanguage = Lang.GetLanguageCodeFromTwoLetterISOLanguageName(fileLanguageNames[fileLanguageIndex]);
                       
            selectedDictionary.InitDictionary();

            GetRecords();

            indexer.UpdateLanguageRegions();
        }
    }

    static void DrawIDFilter()
    {
        if (idFilter != (idFilterTmp = EditorGUILayout.TextField("ID Filter", idFilterTmp)))
        {
            idFilter = idFilterTmp;
            GetRecords();
        }
    }

   static void DrawRecordFilter()
    {
        if (recordFilter != (recordFilterTmp = EditorGUILayout.TextField("Record Filter", recordFilterTmp)))
        {
            recordFilter = recordFilterTmp;
            GetRecords();
        }
    }

    static void GetRecords()
    {
        recordIds =
                (from v in selectedDictionary.dictionary[Lang.selectedLanguage]
                 where
                     (idFilter.Trim() == string.Empty ? true : v.Key.Contains(idFilter)) &&
                     (recordFilter.Trim() == string.Empty ? true : v.Value.Contains(recordFilter))
                 select v.Key
        ).ToArray();

        records =
            (from v in selectedDictionary.dictionary[Lang.selectedLanguage]
             where
                 (idFilter.Trim() == string.Empty ? true : v.Key.Contains(idFilter)) &&
                 (recordFilter.Trim() == string.Empty ? true : v.Value.Contains(recordFilter))
             select v.Key + " " + v.Value
        ).ToArray();
    }

    void DrawGenerate()
    {
        EditorGUI.BeginDisabledGroup(fileLanguageIndex < 0);

        EditorGUILayout.BeginHorizontal();

        if (recordIndex != (recordIndexTmp = EditorGUILayout.Popup("Record", recordIndexTmp, records)))
        {
            recordIndex = recordIndexTmp;
            wordsClip.clip = phonemeClip.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(GetClipAssetPath());
            phonemeClip.phonemes = AssetDatabase.LoadAssetAtPath<TextAsset>(GetPhonemesAssetPath());
            wordsClip.words = AssetDatabase.LoadAssetAtPath<TextAsset>(GetWordsAssetPath());
        }

        EditorGUI.BeginDisabledGroup(recordIndex < 0);

        if (GUILayout.Button("Voice", GUILayout.Width(80)))
        {
            selectedDictionary.InitDictionary();
            string text = selectedDictionary.GetText(Lang.selectedLanguage, recordIds[recordIndex], "", true);
            string path = GetClipAssetPath();
            path = path.GetFullPathFromAssetPath();

            if (!string.IsNullOrEmpty(text))
            {
                string directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);                

                Debug.Log("Generating : " + text);
                GenerateVoice(voiceService, indexer.voice, path, text);

                AssetDatabase.ImportAsset(GetClipAssetPath());

                phonemeClip.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(GetClipAssetPath());
            }
        }

        if (GUILayout.Button("P",GUILayout.Width(25))){
            EditorGUIUtility.PingObject(phonemeClip.clip);
        }

        if (GUILayout.Button("Phonemes", GUILayout.Width(80)))
        {
            selectedDictionary.InitDictionary();
            string text = selectedDictionary.GetText(Lang.selectedLanguage, recordIds[recordIndex], "", true);
            string path = GetClipAssetPath();
            path = path.GetFullPathFromAssetPath();

            if (!string.IsNullOrEmpty(text))
            {
                GeneratePhonemes(path, text);

                AssetDatabase.ImportAsset(GetPhonemesAssetPath());

                phonemeClip.phonemes = AssetDatabase.LoadAssetAtPath<TextAsset>(GetPhonemesAssetPath());
            }
        }

        if (GUILayout.Button("P", GUILayout.Width(25)))
        {
            EditorGUIUtility.PingObject(phonemeClip.phonemes);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(fileLanguageIndex < 0);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("GENERATE ALL VOICES"))
        {
            EditorCoroutineStart.StartCoroutineWithUI(
                GenerateVoicesCoroutine(selectedDictionary, Lang.selectedLanguage, idFilter, recordFilter, voiceService, indexer.voice),
                "Generating voices...", true
                );
        }

        if (GUILayout.Button("GENERATE ALL PHONEMES"))
        {
            EditorCoroutineStart.StartCoroutineWithUI(GeneratePhonemesCoroutine(), "Generating phonemes...", true);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }

    public static IEnumerator GenerateVoicesCoroutine(Lang.LangDictionary langDictionary,Lang.LanguageCode langCode, string idFilter, string recordFilter, VoiceService voiceService, Voice voice)
    {
        GOOGLE.Init();
        
        List<Thread> threadPool = new List<Thread>();

        langDictionary.InitDictionary();

        Dictionary<string, string> kvs = null;

        kvs = (from kv in langDictionary.dictionary[Lang.selectedLanguage]
               where
                   (idFilter.Trim() == string.Empty ? true : kv.Key.Contains(idFilter)) &&
                   (recordFilter.Trim() == string.Empty ? true : kv.Value.Contains(recordFilter))
               select kv).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

        int count = 0;
        foreach (KeyValuePair<string, string> kv in kvs)
        {
            while (threadPool.Count >= SystemInfo.processorCount)
            {
                yield return null;
                for (int i = threadPool.Count - 1; i >= 0; i--) if (!threadPool[i].IsAlive) threadPool.RemoveAt(i);
            }

            //string voiceName = voiceNames[voiceIndex];
            string path = "Assets/Resources Localization/" + LangSettings.instance.guid + "/LanguageAudios/" + langDictionary.aliasName + "/" + langCode + "/" + kv.Key + ".ogg";
            path = path.GetFullPathFromAssetPath();

            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            string text = kv.Value;

            count++;

            EditorCoroutineStart.UpdateUIProgressBar((float)count / (float)kvs.Count);
            EditorCoroutineStart.UpdateUILabel(Path.GetFileName(path));

            if (!string.IsNullOrEmpty(text)) {

                var thread = EditorDispatcher.StartThread((h) =>
                {
                    GenerateVoice(h.VOICESERVICE, h.VOICE, h.PATH, h.TEXT);
                }, new
                {
                    VOICESERVICE = voiceService,
                    VOICE = voice,
                    PATH = path,
                    TEXT = text,                    
                });

                threadPool.Add(thread);
            } 
        }

        while (threadPool.Count > 0)
        {
            yield return null;
            for (int i = threadPool.Count - 1; i >= 0; i--) if (!threadPool[i].IsAlive) threadPool.RemoveAt(i);
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log("ALL VOICES GENERATED");
    }

    IEnumerator GeneratePhonemesCoroutine()
    {
        List<Thread> threadPool = new List<Thread>();

        selectedDictionary.InitDictionary();

        Dictionary<string, string> kvs = null;

        kvs = (from kv in selectedDictionary.dictionary[Lang.selectedLanguage]
               where
                   (idFilter.Trim() == string.Empty ? true : kv.Key.Contains(idFilter)) &&
                   (recordFilter.Trim() == string.Empty ? true : kv.Value.Contains(recordFilter))
               select kv).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

        int count = 0;
        foreach (KeyValuePair<string, string> kv in kvs)
        {
            while(threadPool.Count >= SystemInfo.processorCount)
            {
                yield return null;
                for(int i = threadPool.Count - 1; i>=0; i--) if (!threadPool[i].IsAlive) threadPool.RemoveAt(i);
            } 

            //string voiceName = voiceNames[voiceIndex];
            string path = "Assets/Resources Localization/" + LangSettings.instance.guid + "/LanguageAudios/" + dictionaries[dictionaryIndex] + "/" + fileLanguageNames[fileLanguageIndex] + "/" + kv.Key + ".ogg";
            path = path.GetFullPathFromAssetPath();

            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            string text = kv.Value;

            count++;
            
            EditorCoroutineStart.UpdateUIProgressBar((float)count / (float)kvs.Count);
            EditorCoroutineStart.UpdateUILabel(Path.GetFileName(path));
            
            if (!string.IsNullOrEmpty(text))
            {
                var thread = EditorDispatcher.StartThread((h) =>
                {
                    GeneratePhonemes(h.PATH, h.TEXT);
                }, new
                {
                    PATH = path,
                    TEXT = text,
                });

                threadPool.Add(thread);
            }
        }

        while (threadPool.Count > 0)
        {
            yield return null;
            for (int i = threadPool.Count - 1; i >= 0; i--) if (!threadPool[i].IsAlive) threadPool.RemoveAt(i);
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log("ALL PHONEMES GENERATED");
    }
}