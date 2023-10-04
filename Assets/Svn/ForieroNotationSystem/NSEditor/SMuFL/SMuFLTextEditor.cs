/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.GlyphNames;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SMuFLText))]
public class SMuFLTextEditor : Editor
{

    static string glyphnamesJSONPath = "";
    static string rangesJSONPath = "";
    //static string classesJSONPath = "";

    SMuFLText t;

    static List<string> rangeNames = new List<string>();
    static int rangeNamesIndex = 0;
    static Dictionary<string, int> rangeGlyphNames = new Dictionary<string, int>();
    static int rangeGlyphNamesIndex = -1;

    void OnEnable()
    {
        t = target as SMuFLText;
        LoadRanges();
    }

    enum InputMode
    {
        Range,
        Class,
        All
    }

    InputMode inputMode = InputMode.Range;

    public override void OnInspectorGUI()
    {
        int removeItem = -1;

        for (int i = 0; i < t.glyphNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            t.glyphNames[i] = (GlyphNames)EditorGUILayout.EnumPopup(t.glyphNames[i]);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                removeItem = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (removeItem != -1)
        {
            t.glyphNames.RemoveAt(removeItem);
            t.Apply();
        }

        inputMode = (InputMode)EditorGUILayout.EnumPopup("Input Mode", inputMode);
        switch (inputMode)
        {
            case InputMode.Range:
                rangeNamesIndex = EditorGUILayout.Popup("Range", rangeNamesIndex, rangeNames.ToArray());
                if (GUI.changed)
                {
                    LoadRangeGlyphs();
                }
                rangeGlyphNamesIndex = EditorGUILayout.Popup("Glyphs", rangeGlyphNamesIndex, rangeGlyphNames.Keys.Select(x => x.ToString()).ToArray());
                break;
            case InputMode.Class:
                break;
            case InputMode.All:
                break;
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            t.glyphNames.Add((GlyphNames)rangeGlyphNames.Values.Select(x => (int)x).ToArray()[rangeGlyphNamesIndex]);
            t.Apply();
            EditorUtility.SetDirty(t);
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Apply"))
        {
            t.Apply();
            EditorUtility.SetDirty(t);
        }
    }

    void LoadRanges()
    {
        rangeNames = new List<string>();

        if (string.IsNullOrEmpty(rangesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "ranges.json", SearchOption.AllDirectories);
            rangesJSONPath = files[0];
        }

        JObject rootRanges = JObject.Parse(File.ReadAllText(rangesJSONPath));

        foreach (var nodeClass in rootRanges)
        {
            rangeNames.Add(nodeClass.Key.FirstLetterToUpper());
        }
    }

    void LoadRangeGlyphs()
    {
        rangeGlyphNames = new Dictionary<string, int>();
        rangeGlyphNamesIndex = -1;

        if (string.IsNullOrEmpty(rangesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "ranges.json", SearchOption.AllDirectories);
            rangesJSONPath = files[0];
        }

        JObject rootRanges = JObject.Parse(File.ReadAllText(rangesJSONPath));

        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        JObject rootGlyphNames = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));

        foreach (var nodeClass in rootRanges)
        {
            if (nodeClass.Key.FirstLetterToUpper() == rangeNames[rangeNamesIndex])
            {
                foreach (var nodeEnum in nodeClass.Value["glyphs"])
                {
                    foreach (var nodeGlyph in rootGlyphNames)
                    {
                        if (nodeGlyph.Key == nodeEnum.Value<string>())
                            rangeGlyphNames.Add(nodeGlyph.Key, int.Parse(nodeGlyph.Value["codepoint"].Value<string>().Substring(2), System.Globalization.NumberStyles.HexNumber));
                    }
                }
            }
        }
    }
}
