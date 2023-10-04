using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class LangCharsEditor : EditorWindow
{

    [MenuItem("Foriero/Lang/Language Chars")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(LangCharsEditor));
    }

    void Update()
    {

    }

    List<string> dictionaries = new List<string>();
    bool[] bools = new bool[0];
    Lang.LanguageCode langCode = Lang.LanguageCode.EN;
    string result = "";

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            Refresh();
        }

        langCode = (Lang.LanguageCode)EditorGUILayout.EnumPopup(langCode, GUILayout.Width(80));

        if (GUILayout.Button("Generate", GUILayout.Width(80)))
        {
            List<string> d = new List<string>();

            for (int i = 0; i < bools.Length; i++)
            {
                if (bools[i])
                    d.Add(dictionaries[i]);
            }

            result = Generate(d, langCode);
            GUI.FocusControl("");
        }

        EditorGUILayout.EndHorizontal();

        GUI.SetNextControlName("Result");
        GUILayout.TextField(result);

        if (GUI.GetNameOfFocusedControl() == string.Empty)
        {
            GUI.FocusControl("Result");
        }


        for (int i = 0; i < bools.Length; i++)
        {
            bools[i] = GUILayout.Toggle(bools[i], dictionaries[i]);
        }
    }

    void OnEnable()
    {

    }


    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }

    void Refresh()
    {
        LangSettings.Init();
        dictionaries = Lang.dictionaries.Select(t => t.aliasName).ToList();

        bools = new bool[dictionaries.Count];
    }

    public static string Generate(List<string> dictionaries, Lang.LanguageCode langCode)
    {
        Lang.selectedLanguage = langCode;
        SortedDictionary<char, char> resultDictionary = new SortedDictionary<char, char>();
        string result = "";
        for (int i = 0; i < dictionaries.Count; i++)
        {
            Lang.LangDictionary dictionary = Lang.GetDictionary(dictionaries[i]);
            dictionary.InitDictionary();
            foreach (string key in dictionary.dictionary[langCode].Keys)
            {
                string item = Lang.GetText(dictionaries[i], key, "NOT FOUND");
                foreach (char ch in item)
                {
                    if (!resultDictionary.ContainsKey(ch))
                        resultDictionary.Add(ch, ch);
                }
            }
        }

        foreach (KeyValuePair<char, char> pair in resultDictionary)
        {
            result += pair.Value;
        }

        Debug.Log(result);

        return result;
    }
}