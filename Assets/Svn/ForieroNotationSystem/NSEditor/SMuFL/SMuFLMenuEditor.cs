/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.IO;
using ForieroEditor.CommandLine;
using Newtonsoft.Json.Linq;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.SMuFL.Extensions;
using UnityEditor;
using UnityEngine;

public static class SmuflMenuEditor
{
    private static void DownloadBravuraFont()
    {
        var bravura = Directory.GetFiles(Application.dataPath, "Bravura.otf", SearchOption.AllDirectories);
        Debug.Log(bravura[0]);
        GitHub.GetRepositoryFile("steinbergmedia", "bravura","master/redist/otf/Bravura.otf", bravura[0]);

        var bravuraText = Directory.GetFiles(Application.dataPath, "BravuraText.otf", SearchOption.AllDirectories);
        Debug.Log(bravuraText[0]);
        GitHub.GetRepositoryFile("steinbergmedia", "bravura","master/redist/otf/BravuraText.otf", bravuraText[0]);

        var bravuraMetadata = Directory.GetFiles(Application.dataPath, "bravura_metadata.json", SearchOption.AllDirectories);
        Debug.Log(bravuraMetadata[0]);
        GitHub.GetRepositoryFile("steinbergmedia", "bravura","master/redist/bravura_metadata.json", bravuraMetadata[0]);
    }

    static void DownloadPetalumaFont()
    {
        var petaluma = Directory.GetFiles(Application.dataPath, "Petaluma.otf", SearchOption.AllDirectories);
        Debug.Log(petaluma[0]);
        GitHub.GetRepositoryFile("steinbergmedia", "petaluma","master/redist/otf/Petaluma.otf", petaluma[0]);

        var petalumaText = Directory.GetFiles(Application.dataPath, "PetalumaText.otf", SearchOption.AllDirectories);
        Debug.Log(petalumaText[0]);
        GitHub.GetRepositoryFile("steinbergmedia", "petaluma","master/redist/otf/PetalumaText.otf", petalumaText[0]);
        
        var petalumaMetadata = Directory.GetFiles(Application.dataPath, "petaluma_metadata.json", SearchOption.AllDirectories);
        Debug.Log(petalumaMetadata[0]);
        GitHub.GetRepositoryFile("steinbergmedia", "petaluma","master/redist/petaluma_metadata.json", petalumaMetadata[0]);
    }

    private static void DownloadDefinitions()
    {
        var glyphnames = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
        Debug.Log(glyphnames[0]);
        GitHub.GetRepositoryFile("w3c", "smufl","gh-pages/metadata/glyphnames.json", glyphnames[0]);
        AssetDatabase.Refresh();

        var classes = Directory.GetFiles(Application.dataPath, "classes.json", SearchOption.AllDirectories);
        Debug.Log(classes[0]);
        GitHub.GetRepositoryFile("w3c", "smufl","gh-pages/metadata/classes.json", classes[0]);
        AssetDatabase.Refresh();

        var ranges = Directory.GetFiles(Application.dataPath, "ranges.json", SearchOption.AllDirectories);
        Debug.Log(ranges[0]);
        GitHub.GetRepositoryFile("w3c", "smufl","gh-pages/metadata/ranges.json", ranges[0]);
        AssetDatabase.Refresh();
    }

    [MenuItem("Foriero/NS/SMuFL/Download Latest Font Definition")]
    public static void DownloadLatestFontDefinition()
    {
        DownloadBravuraFont();
        DownloadPetalumaFont();
        DownloadDefinitions();
    }

    [MenuItem("Foriero/NS/SMuFL/Generate All Characters TextFile")]
    public static void CreateTextFileForTextMeshPRO()
    {
        string glyphnamesJSONPath = "";

        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        JObject root = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));

        string codepoint = "";
        int unicode = 0;
        string text = "";

        foreach (var node in root)
        {
            codepoint = node.Value["codepoint"].Value<string>();
            unicode = int.Parse(codepoint.Substring(2), System.Globalization.NumberStyles.HexNumber);
            if (unicode == 58138 || unicode == 58139 || unicode == 58334 || unicode == 58335)
            {
                // missing characters in bravura font //
                continue;
            }
            text += ((char)unicode).ToString();
        }

        string textFile = Path.Combine(Path.GetDirectoryName(glyphnamesJSONPath), "Scripts/Generated/smufl_textmeshpro_all.txt");

        File.WriteAllText(textFile, text);
        AssetDatabase.Refresh();
    }

    [MenuItem("Foriero/NS/SMuFL/Generate Enums")]
    public static void CreateSMuFLEnums()
    {
        GenerateSMuFLClasses();
        GenerateSMuFLRanges();
        GenerateSMuFLGlyphNames();
        AssetDatabase.Refresh();
    }

    [MenuItem("Foriero/NS/SMuFL/Generate Metadata")]
    public static void CreateMetadata()
    {
        GenerateMetadata("Bravura", "bravura_metadata.json");
        GenerateMetadata("Petaluma", "petaluma_metadata.json");
        AssetDatabase.Refresh();
    }

    static void GenerateSMuFLClasses()
    {
        string glyphnamesJSONPath = "";

        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        string classesJSONPath = "";

        if (string.IsNullOrEmpty(classesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "classes.json", SearchOption.AllDirectories);
            classesJSONPath = files[0];
        }

        JObject rootGlyphNames = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));
        JObject rootClasses = JObject.Parse(File.ReadAllText(classesJSONPath));

        string csharp = "";
        string newLine = System.Environment.NewLine;
        int tabLevel = 0;
        csharp += "namespace ForieroEngine.Music.SMuFL.Classes{" + System.Environment.NewLine;
        tabLevel++;
        foreach (var nodeClass in rootClasses)
        {
            csharp += Tabs(tabLevel) + "public enum " + nodeClass.Key.FirstLetterToUpper() + "{" + newLine;
            tabLevel++;
            foreach (var nodeEnum in nodeClass.Value)
            {
                string codepoint = "";
                string unicode = "";

                foreach (var nodeGlyph in rootGlyphNames)
                {
                    if (nodeGlyph.Key == nodeEnum.Value<string>())
                    {
                        codepoint = nodeGlyph.Value["codepoint"].Value<string>();
                        unicode = codepoint.Substring(2);
                    }
                }

                string value = nodeEnum.Value<string>().FirstLetterToUpper();
                if (char.IsDigit(value[0]))
                    value = "_" + value;

                csharp += Tabs(tabLevel) + value + " = 0x" + unicode + "," + newLine;
            }
            tabLevel--;
            csharp += Tabs(tabLevel) + "}" + newLine;
        }
        tabLevel--;
        csharp += "}";

        string csharpPath = Path.Combine(Path.GetDirectoryName(glyphnamesJSONPath), "Scripts/Generated/smufl_classes.cs");

        File.WriteAllText(csharpPath, csharp);
    }

    static void GenerateSMuFLRanges()
    {
        string glyphnamesJSONPath = "";

        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        string rangesJSONPath = "";

        if (string.IsNullOrEmpty(rangesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "ranges.json", SearchOption.AllDirectories);
            rangesJSONPath = files[0];
        }

        JObject rootGlyphNames = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));
        JObject rootRanges = JObject.Parse(File.ReadAllText(rangesJSONPath));

        string csharp = "";
        string newLine = System.Environment.NewLine;
        int tabLevel = 0;
        csharp += "namespace ForieroEngine.Music.SMuFL.Ranges{" + System.Environment.NewLine;
        tabLevel++;
        foreach (var nodeClass in rootRanges)
        {
            csharp += Tabs(tabLevel) + "public enum " + nodeClass.Key.FirstLetterToUpper() + "{" + newLine;
            tabLevel++;
            foreach (var nodeEnum in nodeClass.Value["glyphs"])
            {
                string codepoint = "";
                string unicode = "";

                foreach (var nodeGlyph in rootGlyphNames)
                {
                    if (nodeGlyph.Key == nodeEnum.Value<string>())
                    {
                        codepoint = nodeGlyph.Value["codepoint"].Value<string>();
                        unicode = codepoint.Substring(2);
                    }
                }

                string value = nodeEnum.Value<string>().FirstLetterToUpper();
                if (char.IsDigit(value[0]))
                    value = "_" + value;

                csharp += Tabs(tabLevel) + value + " = 0x" + unicode + "," + newLine;
            }
            tabLevel--;
            csharp += Tabs(tabLevel) + "}" + newLine;
        }
        tabLevel--;
        csharp += "}";

        string csharpPath = Path.Combine(Path.GetDirectoryName(glyphnamesJSONPath), "Scripts/Generated/smufl_ranges.cs");

        File.WriteAllText(csharpPath, csharp);
    }

    static void GenerateSMuFLGlyphNames()
    {
        string glyphnamesJSONPath = "";

        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        JObject rootGlyphNames = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));

        string csharp = "";
        string newLine = System.Environment.NewLine;
        int tabLevel = 0;
        csharp += "namespace ForieroEngine.Music.SMuFL.GlyphNames{" + System.Environment.NewLine;
        tabLevel++;
        csharp += Tabs(tabLevel) + "public enum GlyphNames{" + newLine;
        tabLevel++;
        foreach (var node in rootGlyphNames)
        {
            string codepoint = "";
            string unicode = "";

            codepoint = node.Value["codepoint"].Value<string>();
            unicode = codepoint.Substring(2);

            string value = node.Key.FirstLetterToUpper();
            if (char.IsDigit(value[0]))
                value = "_" + value;

            csharp += Tabs(tabLevel) + value + " = 0x" + unicode + "," + newLine;
        }
        tabLevel--;
        csharp += "}" + newLine;
        tabLevel--;
        csharp += "}";

        string csharpPath = Path.Combine(Path.GetDirectoryName(glyphnamesJSONPath), "Scripts/Generated/smufl_glyphnames.cs");

        File.WriteAllText(csharpPath, csharp);
    }

    static void GenerateMetadata(string className, string metadata)
    {
        string metadataJSONPath = "";

        if (string.IsNullOrEmpty(metadataJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, metadata, SearchOption.AllDirectories);
            metadataJSONPath = files[0];
        }

        string glyphnamesJSONPath = "";

        if (string.IsNullOrEmpty(glyphnamesJSONPath))
        {
            string[] files = Directory.GetFiles(Application.dataPath, "glyphnames.json", SearchOption.AllDirectories);
            glyphnamesJSONPath = files[0];
        }

        JObject rootMetadata = JObject.Parse(File.ReadAllText(metadataJSONPath));
        JObject rootGlyphNames = JObject.Parse(File.ReadAllText(glyphnamesJSONPath));

        string csharp = "";
        string newLine = System.Environment.NewLine;
        int tabLevel = 0;

        csharp += "using System.Collections.Generic;" + newLine + newLine;
        csharp += "namespace ForieroEngine.Music.SMuFL{" + newLine;
        tabLevel++;
        csharp += Tabs(tabLevel) + "public static partial class Metadata{" + newLine;

        tabLevel++;

        csharp += Tabs(tabLevel) + "public static partial class " + className + "{" + newLine;

        tabLevel++;

        csharp += Tabs(tabLevel) + "public static readonly string FontName = \"" + rootMetadata["fontName"].Value<string>() + "\";" + newLine;

        csharp += Tabs(tabLevel) + "public static readonly string FontVersion = \"" + rootMetadata["fontVersion"].Value<string>() + "\";" + newLine + newLine;

        var rootEngravingDefaults = rootMetadata["engravingDefaults"].Value<JObject>();

        csharp += Tabs(tabLevel) + "public static class EngravingDefaults {" + newLine;
        tabLevel++;

        csharp += Tabs(tabLevel) + "public static readonly float arrowShaftThickness = " + rootEngravingDefaults["arrowShaftThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float barlineSeparation = " + rootEngravingDefaults["barlineSeparation"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float beamSpacing = " + rootEngravingDefaults["beamSpacing"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float beamThickness = " + rootEngravingDefaults["beamThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float bracketThickness = " + rootEngravingDefaults["bracketThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float dashedBarlineDashLength = " + rootEngravingDefaults["dashedBarlineDashLength"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float dashedBarlineGapLength = " + rootEngravingDefaults["dashedBarlineGapLength"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float dashedBarlineThickness = " + rootEngravingDefaults["dashedBarlineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float hairpinThickness = " + rootEngravingDefaults["hairpinThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float legerLineExtension = " + rootEngravingDefaults["legerLineExtension"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float legerLineThickness = " + rootEngravingDefaults["legerLineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float lyricLineThickness = " + rootEngravingDefaults["lyricLineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float octaveLineThickness = " + rootEngravingDefaults["octaveLineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float pedalLineThickness = " + rootEngravingDefaults["pedalLineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float repeatBarlineDotSeparation = " + rootEngravingDefaults["repeatBarlineDotSeparation"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float repeatEndingLineThickness = " + rootEngravingDefaults["repeatEndingLineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float slurEndpointThickness = " + rootEngravingDefaults["slurEndpointThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float slurMidpointThickness = " + rootEngravingDefaults["slurMidpointThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float staffLineThickness = " + rootEngravingDefaults["staffLineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float stemThickness = " + rootEngravingDefaults["stemThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float subBracketThickness = " + rootEngravingDefaults["subBracketThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float textEnclosureThickness = " + rootEngravingDefaults["textEnclosureThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float thickBarlineThickness = " + rootEngravingDefaults["thickBarlineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float thinBarlineThickness = " + rootEngravingDefaults["thinBarlineThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float tieEndpointThickness = " + rootEngravingDefaults["tieEndpointThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float tieMidpointThickness = " + rootEngravingDefaults["tieMidpointThickness"].Value<float>() + "f;" + newLine;
        csharp += Tabs(tabLevel) + "public static readonly float tupletBracketThickness = " + rootEngravingDefaults["tupletBracketThickness"].Value<float>() + "f;" + newLine;

        tabLevel--;
        csharp += Tabs(tabLevel) + "}" + newLine + newLine;

        csharp += Tabs(tabLevel) + "public static Dictionary<int, GlyphBoundingBox> glyphBoundingBoxes = new Dictionary<int, GlyphBoundingBox>()" + newLine;
        csharp += Tabs(tabLevel) + "{" + newLine;
        tabLevel++;

        foreach (var nodeGlyphBoundingBox in rootMetadata["glyphBBoxes"].Value<JObject>())
        {
            bool found = false;
            string codepoint = "";
            string unicode = "";
            float top = 0f;
            float left = 0f;
            float right = 0f;
            float bottom = 0f;

            foreach (var nodeGlyphName in rootGlyphNames)
            {
                if (nodeGlyphName.Key == nodeGlyphBoundingBox.Key)
                {
                    codepoint = nodeGlyphName.Value["codepoint"].Value<string>();
                    unicode = codepoint.Substring(2);

                    right = nodeGlyphBoundingBox.Value["bBoxNE"][0].Value<float>();
                    top = nodeGlyphBoundingBox.Value["bBoxNE"][1].Value<float>();
                    left = nodeGlyphBoundingBox.Value["bBoxSW"][0].Value<float>();
                    bottom = nodeGlyphBoundingBox.Value["bBoxSW"][1].Value<float>();

                    found = true;
                    break;
                }
            }

            if (found)
            {
                csharp += Tabs(tabLevel)
                    + "{ 0x" + unicode + ", new GlyphBoundingBox(){ unicode = 0x" + unicode + ", topEm = " + top.ToString() + "f, leftEm = " + left.ToString() + "f, bottomEm = " + bottom.ToString() + "f, rightEm = " + right.ToString() + "f } },"
                    + newLine;
            }
            else
            {
                Debug.LogError("Not found : " + nodeGlyphBoundingBox.Key);
            }
        }

        tabLevel--;
        csharp += Tabs(tabLevel) + "};" + newLine;


        tabLevel--;
        csharp += Tabs(tabLevel) + "}" + newLine;
        tabLevel--;
        csharp += "}";
        tabLevel--;
        csharp += "}";

        string csharpPath = Path.Combine(Path.GetDirectoryName(metadataJSONPath), "Scripts/Generated/smufl_metadata_" + className.ToLower() + ".cs");
        Debug.Log(csharpPath);
        File.WriteAllText(csharpPath, csharp);
    }


    static string Tabs(int level)
    {
        string result = "";
        for (int i = 0; i < level; i++)
        {
            result += "\t";
        }
        return result;
    }
}
