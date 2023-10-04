using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LangSettings))]
public partial class LangSettingsEditor : Editor
{
    ReorderableList supportedLanguagesList;
    ReorderableList googleDocsList;
    ReorderableList dictionariesList;
   
    protected SerializedProperty m_Script;
    protected SerializedProperty debug;
    protected SerializedProperty test;
    protected SerializedProperty guid;
    protected SerializedProperty defaultLanguage;
    protected SerializedProperty forceLanguage;
    protected SerializedProperty forcedLanguage;

    protected SerializedProperty voURLBase;
    protected SerializedProperty voProjectName;
    protected SerializedProperty voVersion;

    protected SerializedProperty supportedLanguages;
    protected SerializedProperty googleDocs;
    protected SerializedProperty dictionaries;
    protected SerializedProperty actors;

    LangSettings o;

    protected virtual void OnEnable()
    {
        o = target as LangSettings;

        m_Script = serializedObject.FindProperty("m_Script");
        debug = serializedObject.FindProperty("debug");
        test = serializedObject.FindProperty("test");
        guid = serializedObject.FindProperty("guid");
        defaultLanguage = serializedObject.FindProperty("defaultLanguage");
        forceLanguage = serializedObject.FindProperty("forceLanguage");
        forcedLanguage = serializedObject.FindProperty("forcedLanguage");

        voURLBase = serializedObject.FindProperty("voURLBase");

        voProjectName = serializedObject.FindProperty("voProjectName");
        voVersion = serializedObject.FindProperty("voVersion");

        supportedLanguages = serializedObject.FindProperty("supportedLanguages");
        googleDocs = serializedObject.FindProperty("googleDocs");
        dictionaries = serializedObject.FindProperty("dictionaries");
        actors = serializedObject.FindProperty("actors");

        supportedLanguagesList = new ReorderableList(serializedObject, supportedLanguages, true, true, true, true);
        googleDocsList = new ReorderableList(serializedObject, googleDocs, true, true, true, true);
        dictionariesList = new ReorderableList(serializedObject, dictionaries, true, true, true, true);
        actorsList = new ReorderableList(serializedObject, actors, true, true, true, true);

        InitSupportedLanguagesList();
        InitGoogleDocsList();
        InitActorsList();
        InitDictionariesList();
    }
       
    bool updateDictionaries = false;
    bool isDirty = false;

    enum Tab
    {
        Languages,
        GoogleDocs,
        Actors,
        Dictionaries,
        Addressables
    }

    Tab tab = Tab.Languages;

    Color backgroundColor;

    public override void OnInspectorGUI()
    {
        backgroundColor = GUI.backgroundColor;

        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginChangeCheck();
        {
            //EditorGUILayout.PropertyField (m_Script);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(guid);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Generate"))
            {
                if (EditorUtility.DisplayDialog("Really?", "Do you really want to generate a new guid?", "Yes", "No"))
                {
                    guid.stringValue = System.Guid.NewGuid().ToString();
                }
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(test);
            EditorGUILayout.PropertyField(defaultLanguage);
            EditorGUILayout.PropertyField(forceLanguage);
            EditorGUILayout.PropertyField(forcedLanguage);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(voURLBase);
            if (GUILayout.Button("Default", GUILayout.Width(60)))
            {
                voURLBase.stringValue = "http://backend.foriero.com/unity/voice_over/";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(voProjectName);
            EditorGUILayout.PropertyField(voVersion);

            GUILayout.BeginHorizontal();
            foreach (Tab t in System.Enum.GetValues(typeof(Tab)))
            {
                GUI.backgroundColor = t == tab ? Color.green : Color.grey;

                if (GUILayout.Button(t.ToString()))
                {
                    tab = t;
                }

                GUI.backgroundColor = backgroundColor;
            }
            GUILayout.EndHorizontal();

            switch (tab)
            {
                case Tab.Languages:
                    supportedLanguagesList?.DoLayoutList();
                    break;
                case Tab.GoogleDocs:
                    googleDocsList?.DoLayoutList();
                    break;
                case Tab.Actors:
                    actorsList?.DoLayoutList();
                    actorVoicesList?.DoLayoutList();
                    DrawVoice(selectedActorVoice);
                    actorDictionariesList?.DoLayoutList();
                    actorRecordsList?.DoLayoutList();
                    break;
                case Tab.Dictionaries:
                    dictionariesList?.DoLayoutList();
                    DrawDictionaryItem();
                    break;
                case Tab.Addressables:
                    DrawAddresables();
                    break;
            }
        }

        if (updateDictionaries)
        {
            XSLXUpdateDictionaries();
            updateDictionaries = false;
            isDirty = true;
        }

        if (EditorGUI.EndChangeCheck() || isDirty)
        {
            serializedObject.ApplyModifiedProperties();
            isDirty = false;
        }
    }
             
    class UpdateDictionaryItem
    {
        public LangSettings.LangDictionary.Storage stored = LangSettings.LangDictionary.Storage.InBuild;         
    }
}
