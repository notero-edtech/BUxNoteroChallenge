using System.Collections.Generic;
using System.Linq;
using ForieroEditor.Coroutines;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public partial class LangSettingsEditor : Editor
{
    private ReorderableList actorsList;
    private ReorderableList actorVoicesList;
    private ReorderableList actorDictionariesList;
    private ReorderableList actorRecordsList;

    private List<string> dicts = new List<string>();

    private class Record
    {
        public string id = "";
        public string record = "";
    }

    private List<Record> records = new List<Record>();

    private SerializedProperty selectedActor = null;
    private SerializedProperty selectedActorVoice = null;
    private string selectedActorDictionary = null;

    void InitActorsList()
    {
        actorsList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Actors (Name)"); };

        actorsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = actorsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("name"), GUIContent.none);
        };

        actorsList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
                actorVoicesList = null;
            }
        };

        actorsList.onChangedCallback = (ReorderableList l) => { };

        actorsList.onReorderCallback = (ReorderableList l) => { };

        actorsList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("name").stringValue = "";
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("langActorVoices")
                .ClearArray();
        };

        actorsList.onSelectCallback = (ReorderableList l) =>
        {
            LangSettings.Init();
            dicts = new List<string>();

            selectedActor = null;
            selectedActorVoice = null;
            selectedActorDictionary = null;

            actorVoicesList = null;
            actorDictionariesList = null;
            actorRecordsList = null;

            if (l.index >= 0 && l.index < l.count)
            {
                selectedActor = l.serializedProperty.GetArrayElementAtIndex(l.index);
                var actorName = selectedActor.FindPropertyRelative("name").stringValue;
                foreach (var d in Lang.dictionaries)
                {
                    d.InitDictionary();
                    var any = d.dictionary[Lang.selectedLanguage].Any(
                        v => actorName.Trim() == string.Empty || v.Value.Contains(actorName)
                    );

                    if (any) dicts.Add(d.aliasName);
                }

                InitActorVoicesList(l);
                InitActorDictionariesList(dicts);
            }
        };
    }

    void InitActorVoicesList(ReorderableList list)
    {
        selectedActorVoice = null;

        actorVoicesList = new ReorderableList(serializedObject,
            actors.GetArrayElementAtIndex(list.index).FindPropertyRelative("langActorVoices"));

        actorVoicesList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Actor's voices (Lang, Service, Accent, VoiceName, Gender, Sampling)");
        };

        actorVoicesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = actorVoicesList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float tw = 0;
            float w = 40;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, w, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("languageCode"), GUIContent.none);
            tw += w;
            w = 70;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("voiceService"), GUIContent.none);

            EditorGUI.BeginDisabledGroup(true);
            tw += w;
            w = 60;
            EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("voice").FindPropertyRelative("languageCodeRegion").stringValue);

            tw += w;
            w = 120;
            EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("voice").FindPropertyRelative("name").stringValue);

            tw += w;
            w = 60;
            VoiceGender voiceGender = (VoiceGender) element.FindPropertyRelative("voice")
                .FindPropertyRelative("voiceGender").enumValueIndex;
            EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight),
                voiceGender.ToString());

            tw += w;
            w = rect.width - tw - 60;
            EditorGUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("voice").FindPropertyRelative("bitRate").intValue.ToString());
            EditorGUI.EndDisabledGroup();

            tw += w;
            w = 60;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Generate"))
            {
                foreach (var d in dicts)
                {
                    var dictionary = Lang.GetDictionary(d);
                    var l = o.actors[actorsList.index].langActorVoices[actorVoicesList.index];
                    var name = o.actors[actorsList.index].name;

                    EditorCoroutineStart.StartCoroutineWithUI(
                        LangVoiceGenerator.GenerateVoicesCoroutine(dictionary, l.languageCode, "", name, l.voiceService,
                            l.voice),
                        "Generating voices...", true
                    );
                }
            }
        };

        actorVoicesList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
            }
        };

        actorVoicesList.onChangedCallback = (ReorderableList l) => { };

        actorVoicesList.onReorderCallback = (ReorderableList l) => { };

        actorVoicesList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
        };

        actorVoicesList.onSelectCallback = (ReorderableList l) =>
        {
            selectedActorVoice = null;

            if (l.index >= 0 && l.index < l.count)
            {
                selectedActorVoice = actorVoicesList.serializedProperty.GetArrayElementAtIndex(l.index);
            }
        };
    }

    void InitActorDictionariesList(List<string> list)
    {
        actorRecordsList = null;
        actorDictionariesList = new ReorderableList(list, typeof(string), true, true, false, false);

        actorDictionariesList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Actor's dictionaries (Dictionary, Generate)");
        };

        actorDictionariesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = actorDictionariesList.list[index] as string;
            rect.y += 2;
            EditorGUI.BeginDisabledGroup(selectedActorVoice == null);
            {
                float tw = 0;
                float w = rect.width - 60;
                GUI.Label(new Rect(rect.x, rect.y, w, EditorGUIUtility.singleLineHeight), element);
                tw += w;
                w = 60;
                if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Generate",
                    EditorStyles.toolbarButton))
                {
                    var dictionary = Lang.GetDictionary(element);
                    var actorVoice = o.actors[actorsList.index].langActorVoices[actorVoicesList.index];
                    var actorName = o.actors[actorsList.index].name;

                    EditorCoroutineStart.StartCoroutineWithUI(
                        LangVoiceGenerator.GenerateVoicesCoroutine(dictionary, actorVoice.languageCode, "", actorName, actorVoice.voiceService,
                            actorVoice.voice),
                        "Generating voices...", true
                    );
                }
            }
            EditorGUI.EndDisabledGroup();
        };

        actorDictionariesList.onSelectCallback = (ReorderableList l) =>
        {
            selectedActorDictionary = null;

            if (selectedActorVoice == null) return;
            
            if (l.index >= 0 && l.index < l.count)
            {
                selectedActorDictionary = dicts[l.index];
                
                LangSettings.Init();
                records = new List<Record>();
                var actorVoice = o.actors[actorsList.index].langActorVoices[actorVoicesList.index];
                var actorName = o.actors[actorsList.index].name;
                var d = Lang.GetDictionary(selectedActorDictionary);
                records =  d.dictionary[actorVoice.languageCode]
                    .Where(v => actorName.Trim() == string.Empty || v.Value.Contains(actorName))
                    .Select(v => new Record() { id = v.Key, record = v.Value.RemoveActorTag()}).ToList();
                InitActorRecordsList(records);
            }
        };
    }

    void InitActorRecordsList(List<Record> list)
    {
        actorRecordsList = new ReorderableList(list, typeof(string), true, true, false, false);

        actorRecordsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Actor's Record (Id, Value, Generate)");
        };

        actorRecordsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = actorRecordsList.list[index] as Record;
            rect.y += 2;
            EditorGUI.BeginDisabledGroup(selectedActorVoice == null);
            {
                float tw = 0;
                float w = 150;
                GUI.TextField(new Rect(rect.x, rect.y, w, EditorGUIUtility.singleLineHeight), element.id);
                tw += w;
                w = rect.width - 250;
                GUI.TextField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), element.record);
                tw += w;
                w = 40;
                if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Log", EditorStyles.toolbarButton))
                {
                    var dictionary = Lang.GetDictionary(dicts[actorDictionariesList.index]);
                    var actorVoice = o.actors[actorsList.index].langActorVoices[actorVoicesList.index];
                    Debug.Log(dictionary.GetText(actorVoice.languageCode,element.id));
                }

                tw += w;
                w = 60;
                if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Generate", EditorStyles.toolbarButton))
                {
                    var dictionary = Lang.GetDictionary(dicts[actorDictionariesList.index]);
                    var actorVoice = o.actors[actorsList.index].langActorVoices[actorVoicesList.index];
                    var actorName = o.actors[actorsList.index].name;

                    EditorCoroutineStart.StartCoroutineWithUI(
                        LangVoiceGenerator.GenerateVoicesCoroutine(dictionary, actorVoice.languageCode, element.id, actorName,
                            actorVoice.voiceService, actorVoice.voice),
                        "Generating voices...", true
                    );
                }
            }
            EditorGUI.EndDisabledGroup();
        };
    }
}