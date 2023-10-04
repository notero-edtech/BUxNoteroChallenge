using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using ForieroEditor.Extensions;
using System.Threading.Tasks;
using System;

public class LineEndingsEditor : EditorWindow
{
    public static LineEndingsEditor window;

    [MenuItem("Foriero/Tools/Line Endings", false, -2000)]
    static void LineEndingsEditorMenu()
    {
        window = EditorWindow.GetWindow(typeof(LineEndingsEditor)) as LineEndingsEditor;
        window.titleContent = new GUIContent("LineEndings");
        window.Show();
    }

    [MenuItem("Assets/LineEndings/Mac")]
    static void ConvertToMac()
    {
        foreach(var o in Selection.objects)
        {
            var p = AssetDatabase.GetAssetPath(o);
            foreach(var f in fileTypes)
            {
                var ext = f.Replace("*", "");
                if (p.EndsWith(ext, comparisonType))
                {
                    Consolidate(p.GetFullPathFromAssetPath(), LineEndingsEnum.Linux);
                }
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/LineEndings/Windows")]
    static void ConvertToWindows()
    {
        foreach (var o in Selection.objects)
        {
            var p = AssetDatabase.GetAssetPath(o);
            foreach (var f in fileTypes)
            {
                var ext = f.Replace("*", "");
                if (p.EndsWith(ext, System.StringComparison.Ordinal))
                {
                    Consolidate(p.GetFullPathFromAssetPath(), LineEndingsEnum.Windows);
                }
            }
        }

        AssetDatabase.Refresh();
    }

    static string[] fileTypes = new string[]{
                "*.meta",
		        "*.h",
                "*.txt",
                "*.cs",
                "*.js",
                "*.boo",
                "*.compute",
                "*.shader",
                "*.cginc",
                "*.glsl",
                "*.xml",
                "*.xaml",
                "*.json",
                "*.inc",
                "*.css",
                "*.htm",
                "*.html",
    };

    static Regex regex = new Regex(@"(?<!\r)\n");
    static System.StringComparison comparisonType = System.StringComparison.Ordinal;

    static readonly string windowsLE = "\r\n";
    static readonly string linuxLE = "\n";

    enum LineEndingsEnum
    {
        Windows,
        Linux
    }

    void OnEnable()
    {
        tasking = false;
    }

    List<string> inconsistentFiles = new List<string>();
    volatile bool tasking = false;
    void OnGUI()
    {
        GUI.enabled = !tasking;
        if(GUILayout.Button("Find")) Find(null);
        GUI.enabled = true;

        if (tasking)
        {
            EditorGUILayout.HelpBox("Searching for inconsistent lines ...", MessageType.Info);
            return;
        }

        bool converted = false;
        foreach(string file in inconsistentFiles){
            GUILayout.BeginHorizontal();
            GUILayout.Label(file);
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Windows")){
                converted = true;
                Consolidate(file, LineEndingsEnum.Windows);
            }

            if(GUILayout.Button("Linux")){
                converted = true;
                Consolidate(file, LineEndingsEnum.Linux);
            }
            GUILayout.EndHorizontal();
        }

        if (converted)
        {
            Find(inconsistentFiles);
            converted = false;
        }
    }

    void Find(List<string> files){
        string dataPath = Application.dataPath;
        Task.Run(() =>
        {
            tasking = true;
            List<string> list = new List<string>();
            try
            {
                int totalFileCount = 0;
                var changedFiles = new List<string>();

                foreach (string fileType in fileTypes)
                {
                    string[] filenames = null;
                    if (files != null)
                    {
                        filenames = files.ToArray();
                        files.Clear();
                    }
                    else
                    {
                        filenames = Directory.GetFiles(dataPath, fileType, SearchOption.AllDirectories);
                    }

                    totalFileCount += filenames.Length;

                    foreach (string filename in filenames)
                    {
                        if (!Consistent(filename)) list.Add(filename);
                    }
                }

                var specialList = new List<string>
            {
            Path.Combine(Directory.GetCurrentDirectory(), ".collabignore"),
            Path.Combine(Directory.GetCurrentDirectory(), ".gitattributes"),
            Path.Combine(Directory.GetCurrentDirectory(), ".gitignore")
            };

                foreach (string filename in specialList)
                {
                    if (!Consistent(filename)) list.Add(filename);
                }
            }
            catch (Exception e) { Debug.LogError(e.Message); }
            finally
            {
                tasking = false;
            }
            inconsistentFiles = list;
        });             
    }

    bool Consistent(string filename){
        if (!File.Exists(filename)) return true;
        string originalText = File.ReadAllText(filename);

        int linuxMathces = Regex.Matches(originalText, linuxLE).Count;
        int windowsMatches = Regex.Matches(originalText, windowsLE).Count;

        if (linuxMathces > 0 && windowsMatches > 0)
        {
            if (linuxMathces != windowsMatches)
            {
                return false;
            }
        }

        return true;
    }

    static void Consolidate(string filename, LineEndingsEnum lineEnding ){
        string originalText = File.ReadAllText(filename);

        int linuxMathces = Regex.Matches(originalText, linuxLE).Count;
        int windowsMatches = Regex.Matches(originalText, windowsLE).Count;

        if (linuxMathces > 0 && windowsMatches > 0)
        {
            if (linuxMathces != windowsMatches)
            {
                string changedText = "";

                changedText = regex.Replace(originalText, windowsLE);

                if (lineEnding == LineEndingsEnum.Linux)
                {
                    changedText = changedText.Replace(windowsLE, linuxLE);
                }

                bool isTextIdentical = string.Equals(changedText, originalText, comparisonType);

                if (!isTextIdentical) File.WriteAllText(filename, changedText, System.Text.Encoding.UTF8);
            }
        }
    }
}
