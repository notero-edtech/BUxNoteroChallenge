/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using ForieroEngine.Music.SMuFL.Extensions;
using UnityEditor;
using UnityEngine;

public class SMuFLEditor : EditorWindow
{

    static string glyphnamesJSONPath = "";
    static string rangesJSONPath = "";
    //static string classesJSONPath = "";

    static Vector2 scrollView = Vector2.zero;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Foriero/NS/SMuFL/SMuFL Tool")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SMuFLEditor window = (SMuFLEditor)EditorWindow.GetWindow(typeof(SMuFLEditor));

        window.titleContent = new GUIContent("SMuFL");

        window.Show();
    }

    static List<string> rangeNames = new List<string>();
    static int rangeNamesIndex = -1;

    public string text = "";
    public Font bravuraFont = null;
    public GUIStyle bravuraFontGUIStyle = null;
    public GUIStyle bravuraButtonGUIStyle = null;
    public string fileName = "smufl_symbols.txt";

    public string rangeSymbols = "";

    void OnGUI()
    {
        if (bravuraFontGUIStyle == null)
        {
            bravuraFontGUIStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
        }

        if (bravuraButtonGUIStyle == null)
        {
            bravuraButtonGUIStyle = new GUIStyle(GUI.skin.box);
        }

        if (bravuraFont == null)
        {
            string[] guids = AssetDatabase.FindAssets("Bravura t:Font");
            if (guids.Length > 0)
            {
                bravuraFont = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            if (bravuraFont)
            {
                bravuraFontGUIStyle.font = bravuraFont;
                bravuraButtonGUIStyle.font = bravuraFont;
            }
        }

        if (rangeNames.Count == 0)
        {
            GUILayout.Label("File not found : ranges.json");
            return;
        }

        GUILayout.BeginHorizontal();

        int lastRangeNamesIndex = rangeNamesIndex;
        rangeNamesIndex = EditorGUILayout.Popup("Range", rangeNamesIndex, rangeNames.ToArray());
        if (lastRangeNamesIndex != rangeNamesIndex)
        {
            rangeSymbols = GetRangeSymbols();
        }

        if (GUILayout.Button("Add All Symbols"))
        {
            text += GetRangeSymbols();
        }

        GUILayout.EndHorizontal();

        scrollView = GUILayout.BeginScrollView(scrollView);

        int objectsCount = 30;
        int bhCount = 0;
        int ehCount = 0;

        for (int i = 0; i < rangeSymbols.Length; i++)
        {
            if (i % objectsCount == 0)
            {
                GUILayout.BeginHorizontal();
                bhCount++;
            }

            if (GUILayout.Button(rangeSymbols[i].ToString(), bravuraButtonGUIStyle))
            {
                text += rangeSymbols[i].ToString();
            }

            if (i % objectsCount == objectsCount - 1)
            {
                GUILayout.EndHorizontal();
                ehCount++;
            }
        }

        if (bhCount != ehCount)
        {
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        text = GUILayout.TextArea(text, bravuraFontGUIStyle);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy to Clipboard"))
        {
            TextEditor te = new TextEditor();
            te.text = text;
            te.SelectAll();
            te.Copy();
        }

        if (GUILayout.Button("Clear"))
        {
            text = "";
        }

        if (GUILayout.Button("Save"))
        {
            string f = EditorUtility.SaveFilePanel("Save symbols", "", fileName, "txt");
            if (!string.IsNullOrEmpty(f))
            {
                File.WriteAllText(f, text);
                AssetDatabase.Refresh();
                fileName = Path.GetFileName(f);
            }
        }
        GUILayout.EndHorizontal();
    }

    void OnEnable()
    {
        LoadRanges();
    }

    void OnDisable()
    {
    }

    void LoadRanges()
    {
        rangeNames = new List<string>();

        if (string.IsNullOrEmpty(rangesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "ranges.json", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                Debug.LogError("File not found : ranges.json");
                return;
            }

            rangesJSONPath = files[0];
        }

        JObject rootRanges = JObject.Parse(File.ReadAllText(rangesJSONPath));

        foreach (var nodeClass in rootRanges)
        {
            rangeNames.Add(nodeClass.Key.FirstLetterToUpper());
        }
    }

    string GetRangeSymbols()
    {
        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        if (string.IsNullOrEmpty(rangesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "ranges.json", SearchOption.AllDirectories);
            rangesJSONPath = files[0];
        }

        JObject rootGlyphNames = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));
        JObject rootRanges = JObject.Parse(File.ReadAllText(rangesJSONPath));

        string codepoint = "";
        int unicode = 0;
        string output = "";

        foreach (var nodeClass in rootRanges)
        {
            if (rangeNames[rangeNamesIndex] != (nodeClass.Key.FirstLetterToUpper()))
                continue;

            foreach (var nodeEnum in nodeClass.Value["glyphs"])
            {
                foreach (var nodeGlyph in rootGlyphNames)
                {
                    if (nodeGlyph.Key == nodeEnum.Value<string>())
                    {
                        codepoint = nodeGlyph.Value["codepoint"].Value<string>();
                        unicode = int.Parse(codepoint.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        output += ((char)unicode).ToString();
                    }
                }
            }
        }

        return output;
    }
}
