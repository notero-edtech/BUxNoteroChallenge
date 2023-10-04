using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

//this limits the editor to running on object that have type CameraLocationHolder
[CustomEditor(typeof(LangActorDefinition))]
public class LangActorDefinitionEditor : Editor
{
    private LangActorDefinition o;

    string[] files = new string[0];
    string[] fileNames = new string[0];

    void OnEnable()
    {
        o = target as LangActorDefinition;

        files = Directory.GetFiles(Application.dataPath + "/Resources/Dictionaries", "*.txt");

        fileNames = new string[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            fileNames[i] = Path.GetFileNameWithoutExtension(files[i]);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Actor"))
        {
            o.actors.Add(new LangActorDefinition.ActorDefinition());
        }
        if (GUILayout.Button("Generate"))
        {
            foreach (LangActorDefinition.ActorDefinition actor in o.actors)
            {
                GenerateActorVoice(actor);
            }
        }
        EditorGUILayout.EndHorizontal();


        foreach (LangActorDefinition.ActorDefinition actor in o.actors)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            actor.actor = EditorGUILayout.TextField("Actor", actor.actor);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Lang"))
            {
                actor.languages.Add(new LangActorDefinition.ActorLangDefinition());
            }
            if (GUILayout.Button("Generate"))
            {
                GenerateActorVoice(actor);
            }
            EditorGUILayout.EndHorizontal();

            foreach (LangActorDefinition.ActorLangDefinition lang in actor.languages)
            {
                EditorGUILayout.BeginHorizontal();
                lang.generate = EditorGUILayout.Toggle(lang.generate, GUILayout.Width(20));
                //lang.voice = EditorGUILayout.TextField(lang.voice);
                //lang.pitchShift = EditorGUILayout.Slider(lang.pitchShift, -3, 3);
                lang.langCode = (Lang.LanguageCode)EditorGUILayout.EnumPopup(lang.langCode, GUILayout.Width(35));

                //string[] voiceArray = voices.Where(item => item.language.Substring(0, 2).ToUpper() == lang.langCode.ToString()).Select(item => item.name).ToArray();
                //if (lang.indexVoice != (lang.indexVoiceTmp = EditorGUILayout.Popup(lang.indexVoiceTmp, voiceArray, GUILayout.Width(50))))
                //{
                //    lang.indexVoice = lang.indexVoiceTmp;
                //    lang.voice = voiceArray[lang.indexVoice];
                //}

                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(o);
    }

    void GenerateActorVoice(LangActorDefinition.ActorDefinition actor)
    {
        LangSettings.Init();

        foreach (LangActorDefinition.ActorLangDefinition lang in actor.languages)
        {
            if (!lang.generate)
                continue;

            foreach (Lang.LangDictionary dictionary in Lang.dictionaries)
            {
                dictionary.InitDictionary(lang.langCode);
                if (dictionary.dictionary.ContainsKey(lang.langCode))
                {
                    //Dictionary<string, string> kvs = (from kv in dictionary.dictionary[lang.langCode]
                    //                                  where
                    //                                      kv.Value.Contains(actor.actor)
                    //                                  select kv).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

                    //int count = 0;

                    //foreach (KeyValuePair<string, string> kv in kvs)
                    //{
                    //    string path = Application.dataPath + "/Resources/LanguageAudios/" + fileNames[fileIndex] + "/" + lang.langCode.ToString() + "/" + kv.Key + ".mp3";
                    //    string directory = Path.GetDirectoryName(path);
                    //    if (!Directory.Exists(directory))
                    //        Directory.CreateDirectory(directory);
                    //    string text = kv.Value;
                    //    if (EditorUtility.DisplayCancelableProgressBar("Generating files", kv.Key, (float)count / (float)kvs.Count))
                    //        break;
                    //    if (!string.IsNullOrEmpty(text))
                    //        LangVoiceGenerator.GenerateVoice(lang.voice, path, 22050, text);
                    //    count++;
                    //}

                    AssetDatabase.Refresh();
                    EditorUtility.ClearProgressBar();
                }
            }
        }
    }
}