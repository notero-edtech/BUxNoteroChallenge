/* Copyright © Marek Ledvina, Foriero s.r.o. */

using System.Collections.Generic;
using ForieroEditor.Extensions;
using MoreLinq;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NSSettings))]
[CanEditMultipleObjects()]
public class NSSettingsInspector : OdinEditor
{
    NSSettings o;
   public void OnEnable() { o = target as NSSettings; }

    public enum Tab { General, License, Display, Session, Playback, Feedback, Instruments, Systems, Pool, Debug}

    private Vector2 _releases;
    private Vector2 _readme;

    private void DrawUrl(string name, string tooltip, string url)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(name, EditorStyles.toolbarButton, GUILayout.Width(130))) { Application.OpenURL(url); }
        EditorGUILayout.LabelField(url, EditorStyles.linkLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();;
    }

    public static void DrawLicense()
    {
        if (!NSDebugSettings.instance.copyrightEditorLabel) return;
        GUILayout.Label("", GUILayout.Height(25));
        GUILayout.Box("Marek Ledvina © Foriero s.r.o. 2022, The Commercial License", GUILayout.ExpandWidth(true));
    }

    private static readonly Dictionary<string, List<(string name, string url)>> Links = new()
    {
        { 
            "Links - W#C Standards", new()
            {
                new ("MusicXML", "https://www.musicxml.com/"),
                new ("MusicXML W3C", "https://w3c.github.io/musicxml/"),
                new ("MusicXML GitHub", "https://github.com/w3c/musicxml"),
                new ("SMuFL", "http://www.smufl.org/"),
                new ("SMuFL W3C", "https://w3c.github.io/smufl/latest/index.html"),
                new ("SMuFL GitHub", "https://github.com/w3c/smufl"),
            }
            
        },
        {
            "Links - Unofficial MusicXML Test Suite", new()
            {
                new ("TestSuite", "https://web.mit.edu/music21/doc/developerReference/musicxmlTest.html"),
                new ("TestSuite GitHub", "https://github.com/cuthbertLab/musicxmlTestSuite"),
            }
        },
        {
            "Links - Notation Software", new()
            {
                new ("Sibelius", "https://www.avid.com/sibelius-ultimate"),
                new ("Finale", "https://www.finalemusic.com/"),
                new ("Dorico", "https://www.steinberg.net/dorico/"),
                new ("Testflight", "https://www.noteflight.com/"),
                new ("Musescore", "https://musescore.org/en"),
            }
        },
        {
            "Links - Notation Software - Plugins", new()
            {
                new ("Dolet 8 or greater", "https://www.musicxml.com/dolet-plugin/dolet-plugin-for-sibelius/"),
            }
        },
        {
            "Links - Books", new()
            {
                new ("Music Notation", "https://berkleepress.com/arranging-composing/music-notation/"),
                new ("Behind Bars", "https://www.alfred.com/behind-bars/"),
                new ("Music Notation 20th", "https://www.amazon.com/Music-Notation-Twentieth-Century-Practical/dp/0393950530"),
            }
        },
        {
            "Links - Fonts", new()
            {
                new ("SMuFL", "https://www.smufl.org/fonts/"),
                new ("Notation Central", "https://www.notationcentral.com/product-category/fonts/music-fonts/"),
            }
        },
        {
            "Links - Libraries", new()
            {
                new ("Manufaktura", "http://manufaktura-programow.pl/"),
            }
        }
    };
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawTabs(Tab.General);
        DrawDefaultInspector();
        
        GUILayout.Box("Releases", GUILayout.ExpandWidth(true));
        GUI.enabled = false;
        _releases = EditorGUILayout.BeginScrollView(_releases, GUILayout.Height(100));
        EditorGUILayout.TextArea(NSSettings.Releases.text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUI.enabled = true;
        GUILayout.Box("Read Me", GUILayout.ExpandWidth(true));
        GUI.enabled = false;
        _readme = EditorGUILayout.BeginScrollView(_readme, GUILayout.Height(100));
        EditorGUILayout.TextArea(NSSettings.ReadMe.text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUI.enabled = true;
       
        if (GUILayout.Button("Copy Links To Clipboard"))
        {
            var s = "";
            Links.ForEach(kv =>
            {
                s += kv.Key + System.Environment.NewLine;
                kv.Value.ForEach(v => s += v.name + " - " + v.url + System.Environment.NewLine);
                s += System.Environment.NewLine;
            });
            s.CopyToClipboard();
        }
        
        Links.ForEach(kv =>
        {
            GUILayout.Box(kv.Key, GUILayout.ExpandWidth(true));
            kv.Value.ForEach(v => DrawUrl(v.name, "", v.url));
        });

        DrawLicense();
        
        EditorGUI.BeginChangeCheck();
        if (!GUI.changed && !EditorGUI.EndChangeCheck()) return;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(o);
    }
    
    public static void DrawTabs(Tab tab)
    {
        GUILayout.BeginHorizontal();

        var c = GUI.backgroundColor;

        if(tab == Tab.General) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("General", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSSettings.instance);
            Selection.objects = new Object[1] { NSSettings.instance };
            Selection.activeObject = NSSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if(tab == Tab.License) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("License", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSLicenseSettings.instance);
            Selection.objects = new Object[1] { NSLicenseSettings.instance };
            Selection.activeObject = NSLicenseSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if(tab == Tab.Session) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Session", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSSessionSettings.instance);
            Selection.objects = new Object[1] { NSSessionSettings.instance };
            Selection.activeObject = NSSessionSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if(tab == Tab.Display) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Display", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSDisplaySettings.instance);
            Selection.objects = new Object[1] { NSDisplaySettings.instance };
            Selection.activeObject = NSDisplaySettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Playback) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Playback", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSPlaybackSettings.instance);
            Selection.objects = new Object[1] { NSPlaybackSettings.instance };
            Selection.activeObject = NSPlaybackSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Feedback) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Feedback", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSFeedbackSettings.instance);
            Selection.objects = new Object[1] { NSFeedbackSettings.instance };
            Selection.activeObject = NSFeedbackSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Instruments) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Instruments", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSInstrumentsSettings.instance);
            Selection.objects = new Object[1] { NSInstrumentsSettings.instance };
            Selection.activeObject = NSInstrumentsSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Systems) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Systems", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSSystemsSettings.instance);
            Selection.objects = new Object[1] { NSSystemsSettings.instance };
            Selection.activeObject = NSSystemsSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Pool) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Pool", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSPoolSettings.instance);
            Selection.objects = new Object[1] { NSPoolSettings.instance };
            Selection.activeObject = NSPoolSettings.instance;
        }
        GUI.backgroundColor = c;
        
        if (tab == Tab.Debug) GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Debug", EditorStyles.toolbarButton))
        {
            EditorGUIUtility.PingObject(NSDebugSettings.instance);
            Selection.objects = new Object[1] { NSDebugSettings.instance };
            Selection.activeObject = NSDebugSettings.instance;
        }
        GUI.backgroundColor = c;
        
        GUILayout.EndHorizontal();
    }
}