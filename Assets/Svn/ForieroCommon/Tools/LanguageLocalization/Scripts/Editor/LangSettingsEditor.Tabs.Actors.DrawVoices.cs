using System.Collections.Generic;
using System.Linq;
using ForieroEditor.Coroutines;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public partial class LangSettingsEditor : Editor
{
    public void DrawVoice(SerializedProperty actorVoice)
    {
        if (actorVoice == null) return;
        
        EditorGUILayout.HelpBox("Language Voice", UnityEditor.MessageType.None, true);
        EditorGUILayout.PropertyField(actorVoice.FindPropertyRelative("voiceService"));

        EditorGUILayout.PropertyField(actorVoice.FindPropertyRelative("languageCode"));

        EditorGUILayout.HelpBox("Voice", UnityEditor.MessageType.None, true);
        var voice = actorVoice.FindPropertyRelative("voice");

        var service = (VoiceService)actorVoice.FindPropertyRelative("voiceService").enumValueIndex;
        var languageCode = (Lang.LanguageCode)actorVoice.FindPropertyRelative("languageCode").enumValueIndex;

        GUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(voice.FindPropertyRelative("languageCodeRegion"));
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(false);
            {
                if (GUILayout.Button("Set", GUILayout.Width(40)))
                {
                    var voices = LangVoiceGenerator.GetVoices(service);
                    var voiceLanguages = (from v in voices
                                            where v.languageCodeRegion.Length >= 2 && v.languageCodeRegion.ToUpper().Substring(0, 2).Contains(languageCode.ToString())
                                            select v.languageCodeRegion).Distinct().OrderBy(l => l).ToArray();

                    var langMenu = new GenericMenu();

                    void OnLangMenuItem(object o){
                        voice.FindPropertyRelative("languageCodeRegion").stringValue = o as string;
                        serializedObject.ApplyModifiedProperties();
                    }

                    foreach (var l in voiceLanguages)
                    {
                        langMenu.AddItem(new GUIContent(l), false, OnLangMenuItem, l );
                    }
                    langMenu.ShowAsContext();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(voice.FindPropertyRelative("name"));
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(false);
            {
                if (GUILayout.Button("Set", GUILayout.Width(40)))
                {
                    var voices = LangVoiceGenerator.GetVoices(service);

                    var voiceNames = (from v in voices
                                  where v.languageCodeRegion.Length >= 2 && v.languageCodeRegion.ToUpper().Contains(voice.FindPropertyRelative("languageCodeRegion").stringValue.ToUpper())
                                  select v).Distinct().OrderBy(l => l.name).ToArray();

                    var nameMenu = new GenericMenu();

                    void OnNameMenuItem(object o)
                    {
                        var v = o as Voice;
                        voice.FindPropertyRelative("name").stringValue = v.name;
                        voice.FindPropertyRelative("voiceGender").enumValueIndex = (int)v.voiceGender;
                        voice.FindPropertyRelative("bitRate").intValue = v.bitRate;
                        serializedObject.ApplyModifiedProperties();
                    }

                    foreach (var v in voiceNames)
                    {
                        nameMenu.AddItem(new GUIContent(v.name), false, OnNameMenuItem, v);
                    }
                    nameMenu.ShowAsContext();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(voice.FindPropertyRelative("voiceGender"));
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(service != VoiceService.OSX);
            {
                if (GUILayout.Button("Set", GUILayout.Width(40)))
                {
                    var gendreMenu = new GenericMenu();

                    void OnGenderMenuItem(object o)
                    {
                        voice.FindPropertyRelative("voiceGender").enumValueIndex = (int)o;
                        serializedObject.ApplyModifiedProperties();
                    }

                    foreach(var e in System.Enum.GetValues(typeof(VoiceGender))){
                        gendreMenu.AddItem(new GUIContent(e.ToString()), false, OnGenderMenuItem, (int)e);
                    }

                    gendreMenu.ShowAsContext();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(voice.FindPropertyRelative("bitRate"));
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(service != VoiceService.OSX);
            {
                if (GUILayout.Button("Set", GUILayout.Width(40)))
                {
                    var bitRateMenu = new GenericMenu();

                    void OnBitRateMenuItem(object o)
                    {
                        voice.FindPropertyRelative("bitRate").intValue = (int)o;
                        serializedObject.ApplyModifiedProperties();
                    }

                    bitRateMenu.AddItem(new GUIContent("44100"), false, OnBitRateMenuItem, 44100);
                    bitRateMenu.AddItem(new GUIContent("22050"), false, OnBitRateMenuItem, 22050);

                    bitRateMenu.ShowAsContext();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Test"))
        {
            var l = o.actors[actorsList.index].langActorVoices[actorVoicesList.index];
            LangVoiceGenerator.TestVoice(l.voiceService, l.voice, "Hello, how are you?");
        }
    }
}
