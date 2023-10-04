using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ForieroEditor.Extensions;

public partial class LangVoice2Word : EditorWindow
{
    public static LangVoice2Word window;

    [MenuItem("Foriero/Lang/Language Voice2Word")]
    static void Init()
    {
        window = EditorWindow.GetWindow(typeof(LangVoice2Word)) as LangVoice2Word;
        window.titleContent = new GUIContent("Voice2Word");
    }

    public string text = "This is some example text to test the functionality.";

    static int dictionaryIndex = -1;
    static int dictionaryIndexTmp = -1;
    static string[] dictionaries = new string[0];

    static int fileLanguageIndex = -1;
    static int fileLanguageIndexTmp = -1;
    static string[] fileLanguageNames = new string[0];

    static Lang.LangDictionary selectedDictionary = null;

    static string[] recordIds = new string[0];
    static string[] records = new string[0];

    static int recordIndex = -1;
    static int recordIndexTmp = -1;

    static List<Voice> voices = new List<Voice>();

    static string[] voiceNames = new string[0];
    static int voiceIndex = -1;
    static int voiceIndexTmp = -1;

    static string[] voiceLanguages = new string[0];
    static int voiceLanguageIndex = -1;
    static int voiceLanguageIndexTmp = -1;

    static int bitRate = 22050;


    static AudioClip audioClip;
    static string audioDescription;
    static float audioTime = 0;
    static Color timeColor = Color.white;

    List<Texture2D> waveforms = new List<Texture2D>();

    static Vector2 waveformScroll = Vector2.zero;
    static int waveformPixelsPerSecond = 300;
    static float waveformTop = 20f;
    static float waveformBottom = 20f;
    static float waveformHeight = 150;
    static int waveformBorder = 10;

    static float renderWidth = 0;

    static bool focused = false;

    public LangVoice2Word()
    {
        EditorApplication.update += EveryFrameUpdate;
    }

    ~LangVoice2Word()
    {
        EditorApplication.update -= EveryFrameUpdate;
    }

    void OnFocus()
    {
        focused = true;
    }

    void OnLostFocus()
    {
        focused = false;
    }

    bool playWord = false;
    float playWordFinishTime = 0f;

    void EveryFrameUpdate()
    {
        if (!focused) return;

        if (audioClip)
        {

            audioTime = audioClip.GetClipPosition();
            if (playWord && audioTime >= playWordFinishTime)
            {
                audioClip?.StopClip();
                playWord = false;
            }

            if (m_Data.Count > 0)
            {
                DragObject closestDragObject = m_Data[0];
                foreach (DragObject d in m_Data)
                {
                    d.highlighted = false;
                    if (d.time < audioTime && d.time > closestDragObject.time)
                    {
                        closestDragObject = d;
                    }
                }
                closestDragObject.highlighted = true;
            }

            Repaint();

            if (!audioClip.IsClipPlaying())
            {
                foreach (DragObject d in m_Data)
                {
                    d.highlighted = false;
                }
            }
        }
    }

    void RefreshDictionaries()
    {
        dictionaryIndex = dictionaryIndexTmp = -1;
        fileLanguageIndex = fileLanguageIndexTmp = -1;
        recordIndex = recordIndexTmp = -1;

        voiceLanguageIndex = voiceLanguageIndexTmp = -1;
        voiceIndex = voiceIndexTmp = -1;

        dictionaries = new string[0];
        fileLanguageNames = new string[0];

        recordIds = new string[0];
        records = new string[0];

        voices = LangVoiceGenerator.GetVoices(VoiceService.OSX);

        waveforms.Clear();

        LangSettings.Init();

        dictionaries = Lang.dictionaries.Select(d => d.aliasName).ToArray();
    }

    void OnEnable()
    {
        RefreshDictionaries();
    }

    void OnDisable()
    {

    }

    void OnSelectionChange()
    {

    }

    public void Update()
    {
        if (doRepaint)
        {
            Repaint();
        }
    }

    GUIStyle box;



    void OnGUI()
    {

        //This style is for box rendered behind waveforms//
        box = GUI.skin.GetStyle("Box");

        #region Dictionary Selection

        EditorGUILayout.BeginHorizontal();

        if (dictionaryIndex != (dictionaryIndexTmp = EditorGUILayout.Popup(dictionaryIndexTmp, dictionaries)))
        {
            dictionaryIndex = dictionaryIndexTmp;

            fileLanguageIndex = fileLanguageIndexTmp = -1;

            recordIndex = -1;

            voiceLanguageIndex = voiceLanguageIndexTmp = -1;
            voiceIndex = voiceIndexTmp = -1;

            recordIds = new string[0];
            records = new string[0];

            selectedDictionary = Lang.GetDictionary(dictionaries[dictionaryIndex]);
            selectedDictionary.InitDictionary();

            fileLanguageNames = new string[selectedDictionary.languages.Count];
            for (int i = 0; i < fileLanguageNames.Length; i++)
            {
                fileLanguageNames[i] = selectedDictionary.languages[i].ToString();
            }
        }

        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            RefreshDictionaries();
        }

        EditorGUILayout.EndHorizontal();
        #endregion

        #region Language
        if (fileLanguageIndex != (fileLanguageIndexTmp = EditorGUILayout.Popup(fileLanguageIndexTmp, fileLanguageNames)))
        {
            fileLanguageIndex = fileLanguageIndexTmp;

            Lang.selectedLanguage = Lang.GetLanguageCodeFromTwoLetterISOLanguageName(fileLanguageNames[fileLanguageIndex]);

            selectedDictionary.InitDictionary();

            recordIds = selectedDictionary.dictionary[Lang.selectedLanguage].Keys.ToArray();
            records = selectedDictionary.dictionary[Lang.selectedLanguage].Values.ToArray();
            recordIndex = -1;

            voiceLanguageIndex = voiceLanguageIndexTmp = -1;
            voiceIndex = voiceIndexTmp = -1;

            voiceLanguages = (

                (from v in voices
                 where v.languageCodeRegion.ToUpper().Contains(fileLanguageNames[fileLanguageIndex])
                 select v.languageCodeRegion).Distinct().OrderBy(l => l)

                ).ToArray();
        }
        #endregion

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("<", GUILayout.Width(25)))
        {
            if (records.Length > 0 && recordIndexTmp > 0)
            {
                recordIndex = --recordIndexTmp;
                Load(selectedDictionary, Lang.selectedLanguage, recordIds[recordIndex]);
            }
        }

        if (recordIndex != (recordIndexTmp = EditorGUILayout.Popup(recordIndexTmp, recordIds, GUILayout.Width(150))))
        {
            recordIndex = recordIndexTmp;
            Load(selectedDictionary, Lang.selectedLanguage, recordIds[recordIndex]);
        }

        if (GUILayout.Button(">", GUILayout.Width(25)))
        {
            if (records.Length > 0 && recordIndexTmp < records.Length - 1)
            {
                recordIndex = ++recordIndexTmp;
                Load(selectedDictionary, Lang.selectedLanguage, recordIds[recordIndex]);
            }
        }

        if (recordIndex >= 0 && records.Length > 0) EditorGUILayout.TextField(records[recordIndex], GUILayout.Height(GUI.skin.GetStyle("Label").CalcSize(new GUIContent(records[recordIndex])).y));

        EditorGUILayout.EndHorizontal();

        if (waveforms.Count > 0)
        {

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play", GUILayout.Width(80)))
            {
                audioClip?.PlayClip();
            }

            if (GUILayout.Button("Stop", GUILayout.Width(80)))
            {
                audioClip?.StopClip();
                playWord = false;
            }

            if (GUILayout.Button("Pause", GUILayout.Width(80)))
            {
                audioClip?.PauseClip();
            }

            if (GUILayout.Button("Resume", GUILayout.Width(80)))
            {
                audioClip?.ResumeClip();
            }

            waveformPixelsPerSecond = EditorGUILayout.IntField("Pixels/Second", waveformPixelsPerSecond);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                Load(selectedDictionary, Lang.selectedLanguage, recordIds[recordIndex]);
            }

            if (GUILayout.Button("Save", GUILayout.Width(80)))
            {
                Save(selectedDictionary, Lang.selectedLanguage, recordIds[recordIndex]);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Time : ", audioTime.ToString());

            EditorGUILayout.EndHorizontal();

            waveformScroll = GUILayout.BeginScrollView(waveformScroll, GUIStyle.none, GUIStyle.none);

            int waveformIndex = 0;



            foreach (Texture2D waveform in waveforms)
            {
                if (waveform == null) continue;
                Rect waveformRect = new Rect(
                    waveformBorder / 2f,
                    (waveformTop + waveformHeight + waveformBottom + box.border.top + box.border.bottom) * (waveformIndex) + waveformTop + box.border.top,
                    waveform.width,
                    waveformHeight);

                EditorGUI.DrawTextureAlpha(waveformRect, waveform as Texture);

                GUILayout.Box("", GUILayout.Width(waveformRect.width + box.border.right), GUILayout.Height(waveformTop + waveformHeight + waveformBottom));

                waveformIndex++;
            }

            OnTimeBar();

            OnGUIWords();

            GUILayout.EndScrollView();
        }
        else
        {
            if (records.Length > 0 && recordIndex >= 0)
            {
                if (voiceLanguageIndex != (voiceLanguageIndexTmp = EditorGUILayout.Popup(voiceLanguageIndexTmp, voiceLanguages)))
                {
                    voiceLanguageIndex = voiceLanguageIndexTmp;
                    voiceIndex = voiceIndexTmp = -1;
                    voiceNames = (

                        (from v in voices
                         where v.languageCodeRegion.Contains(voiceLanguages[voiceLanguageIndex])
                         select v.name).Distinct().OrderBy(l => l)
                        ).ToArray();
                }

                if (voiceIndex != (voiceIndexTmp = EditorGUILayout.Popup(voiceIndexTmp, voiceNames)))
                {
                    voiceIndex = voiceIndexTmp;
                }

                bitRate = EditorGUILayout.IntField("BitRate", bitRate);

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = voiceIndex == -1 ? false : true;
                if (GUILayout.Button("Generate"))
                {
                    string voiceName = voiceNames[voiceIndex];
                    string path = Application.dataPath + "/Resources/LanguageAudios/" + dictionaries[dictionaryIndex] + "/" + fileLanguageNames[fileLanguageIndex] + "/" + recordIds[recordIndex] + ".wav";
                    string directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                    string text = records[recordIndex];
                    LangVoiceGenerator.GenerateVoice(VoiceService.OSX, null, path, text);
                    AssetDatabase.Refresh();
                }
                GUI.enabled = true;
                EditorGUILayout.LabelField("No audio clip exists");
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    float WidthToTime(float width)
    {
        return width / waveformPixelsPerSecond;
    }

    float TimeToWidth(float time)
    {
        return Mathf.Floor(time * waveformPixelsPerSecond);
    }

    float TimeToWidth(float time, int waveformIndex)
    {
        return Mathf.Floor(time * waveformPixelsPerSecond) - (waveformIndex * (renderWidth - waveformBorder));
    }

    void PlayWord(DragObject aDragObject)
    {
        float closestTime = audioClip.length;
        foreach (DragObject d in m_Data)
        {
            if (d.time > aDragObject.time && closestTime > d.time) closestTime = d.time;
        }
        playWord = true;
        playWordFinishTime = closestTime;
        Debug.Log(aDragObject.time);
        audioClip.PlayClip(aDragObject.time);
    }

    int GetWaveformIndexFormTime(float time)
    {
        int result = -1;
        do
        {
            result++;
            time -= WidthToTime(renderWidth - waveformBorder);

        } while (time > 0);
        return result;
    }
}