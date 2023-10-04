using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[CustomPropertyDrawer(typeof(TTLangRecord))]
class TTooltipEditor : PropertyDrawer
{

    float rowSpace = 1f;
    float rowHeigth = 16;
    // Draw the property inside the given rect
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty dictionary = prop.FindPropertyRelative("dictionary");
        SerializedProperty recordID = prop.FindPropertyRelative("id");
        SerializedProperty defaultText = prop.FindPropertyRelative("defaultText");

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width, rowHeigth), dictionary, new GUIContent("Dictionary"));
        EditorGUI.PropertyField(new Rect(pos.x, pos.y + rowHeigth, pos.width, rowHeigth), recordID, new GUIContent("Record ID"));
        EditorGUI.PropertyField(new Rect(pos.x, pos.y + rowHeigth * 2f, pos.width, rowHeigth), defaultText, new GUIContent("Default Text"));
        EditorGUI.indentLevel = indent;

        if (GUI.Button(new Rect(pos.x, pos.y + rowHeigth * 3f + rowSpace, 60, rowHeigth), "Refresh"))
        {
            Refresh();
        }

        selectedLanguage = (Lang.LanguageCode)EditorGUI.EnumPopup(new Rect(pos.x + 60, pos.y + rowHeigth * 3f + rowSpace, 40, rowHeigth), selectedLanguage);

        if (dictionaryIndex != (dictionaryIndexTmp = EditorGUI.Popup(new Rect(pos.x + 60 + 40, pos.y + rowHeigth * 3f + rowSpace, pos.width - 60 - 40, rowHeigth), dictionaryIndexTmp, dictionaries)))
        {
            dictionaryIndex = dictionaryIndexTmp;

            records = GetRecords();
        }

        if (filter != (filterTmp = EditorGUI.TextField(new Rect(pos.x + 60, pos.y + rowHeigth * 4f + rowSpace * 2f, pos.width - 60, rowHeigth), filterTmp)))
        {
            filter = filterTmp;
            records = GetRecords();
        }

        if (GUI.Button(new Rect(pos.x, pos.y + rowHeigth * 4f + rowSpace * 2f, 60, rowHeigth), "Sort"))
        {
            System.Array.Sort(records);
        }

        if (recordIndex != (recordIndexTmp = EditorGUI.Popup(new Rect(pos.x + 60, pos.y + rowHeigth * 5f + rowSpace * 3f, pos.width - 60, rowHeigth), recordIndexTmp, records)))
        {
            recordIndex = recordIndexTmp;
        }

        if (GUI.Button(new Rect(pos.x, pos.y + rowHeigth * 5f + rowSpace * 3f, 60, 15), "Set"))
        {
            Lang.selectedLanguage = selectedLanguage;

            dictionary.stringValue = dictionaries[dictionaryIndex];

            recordID.stringValue = records[recordIndex].Split(' ')[0];

            defaultText.stringValue = TTooltipEditor.dictionary.GetText(recordID.stringValue, "");
        }
    }

    //override this function to add space below the new property drawer
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        return 100;
    }

    static Lang.LanguageCode selectedLanguage = Lang.LanguageCode.EN;
    static string[] dictionaries = new string[0];
    static string[] records = new string[0];

    static int dictionaryIndex = -1;
    static int dictionaryIndexTmp = 0;
    static int recordIndex = -1;
    static int recordIndexTmp = 0;

    static string filter;
    static string filterTmp;

    static Lang.LangDictionary dictionary;

    public void OnEnable()
    {
        if (dictionaries.Length == 0) Refresh();
    }

    private void Refresh()
    {
        LangSettings.Init();
        dictionaries = Lang.dictionaries.Select(d => d.aliasName).ToArray();
        records = new string[0];
        dictionaryIndex = -1;
    }

    private string[] GetDictionaries()
    {
        string[] files = System.IO.Directory.GetFiles(Path.Combine(Application.dataPath, "Resources/Dictionaries"), "*.txt");
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileNameWithoutExtension(files[i]);
        }
        return files;
    }

    public string[] GetRecords()
    {
        string[] records = new string[0];

        if (dictionaryIndex >= 0 && dictionaryIndex < dictionaries.Length)
        {
            dictionary = Lang.GetDictionary(dictionaries[dictionaryIndex]);
            dictionary.InitDictionary(selectedLanguage);

            if (string.IsNullOrEmpty(filter))
            {
                records = (from kv in dictionary.dictionary[selectedLanguage]
                           select kv.Key + " " + kv.Value).ToArray();
            }
            else
            {
                records = (from kv in dictionary.dictionary[selectedLanguage]
                           where kv.Key.Contains(filter)
                           select kv.Key + " " + kv.Value).ToArray();
            }
        }
        return records;
    }
}